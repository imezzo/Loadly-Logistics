using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using LoadlyAggregator.Domain.Common.Constants;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Infrastructure.Data;

namespace LoadlyAggregator.Infrastructure.Data.Seeders;

public class GeneralSettingsSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public GeneralSettingsSeeder(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task SeedAsync()
    {
        await SeedFirebaseCredentialsAsync();
        await SeedSmsSettingsAsync();
    }

    private async Task SeedFirebaseCredentialsAsync()
    {
        var key = GeneralSettingKeys.FirebaseCredentialsJson;
        var exists = await _context.GeneralSettings.AnyAsync(gs => gs.Key == key);
        if (exists)
        {
            Console.WriteLine("⏭️  General setting Firebase:CredentialsJson already exists. Skipping.");
            return;
        }

        var value = _configuration["Firebase:CredentialsJson"] ?? string.Empty;

        _context.GeneralSettings.Add(new GeneralSetting
        {
            Key = key,
            Value = value,
        });
        await _context.SaveChangesAsync();
        Console.WriteLine("✅ General setting Firebase:CredentialsJson seeded successfully!");
        if (string.IsNullOrEmpty(value))
        {
            Console.WriteLine("   (Value empty – set via API or appsettings and re-seed later.)");
        }
    }

    private async Task SeedSmsSettingsAsync()
    {
        var smsKeys = new[]
        {
            GeneralSettingKeys.SmsProvider,
            GeneralSettingKeys.SmsVictoryLinkUsername,
            GeneralSettingKeys.SmsVictoryLinkPassword,
            GeneralSettingKeys.SmsVictoryLinkSenderName,
            GeneralSettingKeys.SmsVictoryLinkApiUrl,
            GeneralSettingKeys.SmsUltraMsgToken,
            GeneralSettingKeys.SmsUltraMsgInstanceId,
            GeneralSettingKeys.SmsAkedlyApiKey,
            GeneralSettingKeys.SmsAkedlyPipelineId,
            GeneralSettingKeys.SmsAkedlyApiUrl
        };

        var configKeys = new Dictionary<string, string>
        {
            [GeneralSettingKeys.SmsProvider] = "Sms:Provider",
            [GeneralSettingKeys.SmsVictoryLinkUsername] = "Sms:VictoryLink:Username",
            [GeneralSettingKeys.SmsVictoryLinkPassword] = "Sms:VictoryLink:Password",
            [GeneralSettingKeys.SmsVictoryLinkSenderName] = "Sms:VictoryLink:SenderName",
            [GeneralSettingKeys.SmsVictoryLinkApiUrl] = "Sms:VictoryLink:ApiUrl",
            [GeneralSettingKeys.SmsUltraMsgToken] = "Sms:UltraMsg:Token",
            [GeneralSettingKeys.SmsUltraMsgInstanceId] = "Sms:UltraMsg:InstanceId",
            [GeneralSettingKeys.SmsAkedlyApiKey] = "Sms:Akedly:ApiKey",
            [GeneralSettingKeys.SmsAkedlyPipelineId] = "Sms:Akedly:PipelineId",
            [GeneralSettingKeys.SmsAkedlyApiUrl] = "Sms:Akedly:ApiUrl"
        };

        var existingKeys = await _context.GeneralSettings
            .Where(gs => smsKeys.Contains(gs.Key))
            .Select(gs => gs.Key)
            .ToListAsync();

        var added = 0;
        foreach (var key in smsKeys)
        {
            if (existingKeys.Contains(key))
            {
                continue;
            }

            var configKey = configKeys[key];
            var value = _configuration[configKey] ?? string.Empty;

            _context.GeneralSettings.Add(new GeneralSetting
            {
                Key = key,
                Value = value,
            });
            added++;
        }

        if (added > 0)
        {
            await _context.SaveChangesAsync();
            Console.WriteLine($"✅ Seeded {added} SMS general setting(s).");
        }
        else
        {
            Console.WriteLine("⏭️  SMS general settings already exist. Skipping.");
        }
    }
}
