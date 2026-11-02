using System.Text.Json;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using LoadlyAggregator.Domain.Common.Constants;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Infrastructure.Services.Notification;
using LoadlyAggregator.Shared.Api;
using LoadlyAggregator.Shared.DTOs.Queue;

namespace LoadlyAggregator.Infrastructure.Services.Queue;

public class PushNotificationQueueProcessor : IQueueProcessor
{
    private readonly ILogger<PushNotificationQueueProcessor> _logger;
    private readonly IConfiguration _configuration;
    private readonly LogsDbContext _logsDbContext;
    private readonly IGeneralSettingRepository _generalSettingRepository;
    private readonly SemaphoreSlim _firebaseInitLock = new(1, 1);
    private FirebaseMessaging? _firebaseMessaging;

    public PushNotificationQueueProcessor(
        ILogger<PushNotificationQueueProcessor> logger,
        IConfiguration configuration,
        LogsDbContext logsDbContext,
        IGeneralSettingRepository generalSettingRepository)
    {
        _logger = logger;
        _configuration = configuration;
        _logsDbContext = logsDbContext;
        _generalSettingRepository = generalSettingRepository;
        _firebaseMessaging = FirebaseApp.DefaultInstance != null ? FirebaseMessaging.DefaultInstance : null;
    }

    private async Task<FirebaseMessaging?> GetOrCreateFirebaseMessagingAsync(CancellationToken cancellationToken = default)
    {
        if (_firebaseMessaging != null)
        {
            return _firebaseMessaging;
        }

        await _firebaseInitLock.WaitAsync(cancellationToken);
        try
        {
            if (_firebaseMessaging != null)
            {
                return _firebaseMessaging;
            }

            _firebaseMessaging = await CreateFirebaseAppAsync(cancellationToken);
            return _firebaseMessaging;
        }
        finally
        {
            _firebaseInitLock.Release();
        }
    }

    private async Task<FirebaseMessaging?> CreateFirebaseAppAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // 1. Try from DB (general_settings table) first - works well in Docker
            var credentialsJsonFromDb = await _generalSettingRepository.GetValueByKeyAsync(GeneralSettingKeys.FirebaseCredentialsJson);
            if (!string.IsNullOrEmpty(credentialsJsonFromDb))
            {
                return CreateFirebaseAppFromJson(credentialsJsonFromDb);
            }

            // 2. Fall back to configuration (appsettings / environment)
            var credentialsJson = _configuration["Firebase:CredentialsJson"];
            var credentialsPath = _configuration["Firebase:CredentialsPath"];

            if (!string.IsNullOrEmpty(credentialsJson))
            {
                return CreateFirebaseAppFromJson(credentialsJson);
            }

            if (!string.IsNullOrEmpty(credentialsPath))
            {
                return CreateFirebaseAppFromFile(credentialsPath);
            }

            return LogWarningAndReturnNull("Firebase credentials not configured. Set Firebase:CredentialsJson in general_settings or appsettings. Push notifications will fail.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Firebase");
            return null;
        }
    }

    private FirebaseMessaging? LogWarningAndReturnNull(string message)
    {
        _logger.LogWarning(message);
        return null;
    }

    private FirebaseMessaging? CreateFirebaseAppFromFile(string credentialsPath)
    {
        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromFile(credentialsPath)
        });
        _logger.LogInformation("Firebase initialized successfully from file");
        return FirebaseMessaging.DefaultInstance;
    }

    private FirebaseMessaging? CreateFirebaseAppFromJson(string credentialsJson)
    {
        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromJson(credentialsJson)
        });
        _logger.LogInformation("Firebase initialized successfully from JSON");
        return FirebaseMessaging.DefaultInstance;
    }

    public async Task ProcessMessageAsync(string messageJson, string queueName, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = JsonSerializer.Deserialize<PushNotificationQueueMessage>(
                messageJson,
                ApiDateTimeJsonSerializerOptions.SharedQueueAndRedis);

            _ = message ?? throw new InvalidOperationException($"Failed to deserialize push notification message from queue {queueName}");

            _logger.LogInformation("Processing push notification message to device {DeviceToken}", PiiRedactor.RedactToken(message.DeviceToken));

            var logEntry = CreateLogEntry(message);

            await ProcessPushNotificationAsync(message, logEntry, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Failed to deserialize push notification message from queue {QueueName}", queueName);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse push notification message JSON from queue {QueueName}", queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing push notification message from queue {QueueName}", queueName);
        }
    }

    private static NotificationsLog CreateLogEntry(PushNotificationQueueMessage message)
    {
        return new NotificationsLog
        {
            Channel = NotificationChannel.PUSH,
            Recipient = message.DeviceToken,
            Body = $"Title: {message.Title}\n\nBody: {message.Body}",
            Status = NotificationStatus.PENDING,
            EventType = message.EventType.HasValue ? (NotificationEventType)message.EventType.Value : null,
            ModelId = message.ModelId,
            ModelType = message.ModelType,
            BroadcastNotificationId = message.BroadcastNotificationId
        };
    }

    private async Task ProcessPushNotificationAsync(
        PushNotificationQueueMessage message,
        NotificationsLog logEntry,
        CancellationToken cancellationToken)
    {
        try
        {
            var firebase = await GetOrCreateFirebaseMessagingAsync(cancellationToken);
            _ = firebase ?? throw new InvalidOperationException("Firebase is not initialized. Please check Firebase configuration (general_settings table or appsettings).");

            var pushStartTime = DateTime.UtcNow;
            var response = await firebase.SendAsync(BuildFirebaseMessage(message), cancellationToken);
            var pushDuration = (DateTime.UtcNow - pushStartTime).TotalMilliseconds;

            UpdateLogEntryForSuccess(logEntry, response, pushDuration);
            _logger.LogInformation("Push notification sent successfully to device {DeviceToken} with message ID: {MessageId} in {Duration}ms", PiiRedactor.RedactToken(message.DeviceToken), response, pushDuration);

            _logsDbContext.NotificationsLogs.Add(logEntry);
            await _logsDbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            UpdateLogEntryForFailure(logEntry, ex);
            _logger.LogError(ex, "Exception occurred while sending push notification to device {DeviceToken}", PiiRedactor.RedactToken(message.DeviceToken));

            await SaveLogEntrySafely(logEntry);
        }
    }

    private static Message BuildFirebaseMessage(PushNotificationQueueMessage message)
    {
        return new Message
        {
            Token = message.DeviceToken,
            Notification = new FirebaseAdmin.Messaging.Notification
            {
                Title = message.Title,
                Body = message.Body,
                ImageUrl = message.ImageUrl
            },
            Data = BuildDataDictionary(message),
            Android = new AndroidConfig
            {
                Priority = Priority.High,

                // ClickAction is deliberately left unset: it must name an Android intent filter
                // action, and a business action such as "open_trip" matches no activity, so the tap
                // silently does nothing while the app is killed. Clients read the business action
                // from the data payload instead.
                Notification = new AndroidNotification
                {
                    Title = message.Title,
                    Body = message.Body,
                    ImageUrl = message.ImageUrl,

                    // EventTimestamp is a non-nullable DateTime, so leaving it unset still emits
                    // event_time 0001-01-01 and Android renders the notification as ~2032 years old.
                    EventTimestamp = DateTime.UtcNow
                }
            },
            Apns = new ApnsConfig
            {
                Aps = new Aps
                {
                    Alert = new ApsAlert
                    {
                        Title = message.Title,
                        Body = message.Body
                    },
                    Badge = 1,
                    Sound = "default"
                }
            }
        };
    }

    private static Dictionary<string, string> BuildDataDictionary(PushNotificationQueueMessage message)
    {
        var data = message.Data?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new Dictionary<string, string>();

        data["title"] = message.Title ?? string.Empty;
        data["body"] = message.Body ?? string.Empty;

        if (!string.IsNullOrEmpty(message.ClickAction))
        {
            data["clickAction"] = message.ClickAction;
        }

        return data;
    }

    private static void UpdateLogEntryForSuccess(NotificationsLog logEntry, string messageId, double durationMs)
    {
        logEntry.Status = NotificationStatus.SUCCESS;
        logEntry.Response = JsonSerializer.Serialize(
            new
            {
                success = true,
                messageId,
                durationMs
            },
            ApiDateTimeJsonSerializerOptions.SharedErrorResponse);
    }

    private static void UpdateLogEntryForFailure(NotificationsLog logEntry, Exception ex)
    {
        logEntry.Status = NotificationStatus.FAILED;
        logEntry.Response = JsonSerializer.Serialize(
            new { error = ex.Message, type = ex.GetType().Name },
            ApiDateTimeJsonSerializerOptions.SharedErrorResponse);
    }

    private async Task SaveLogEntrySafely(NotificationsLog logEntry)
    {
        try
        {
            _logsDbContext.NotificationsLogs.Add(logEntry);
            await _logsDbContext.SaveChangesAsync();
        }
        catch (Exception logEx)
        {
            _logger.LogError(logEx, "Failed to save notification log entry");
        }
    }
}
