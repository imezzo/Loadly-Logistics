using System.Text;
using System.Text.Json.Serialization;
using AutoMapper.Internal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Rollbar;
using Rollbar.NetCore.AspNet;
using Serilog;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.ModelBinders;
using LoadlyAggregator.Infrastructure.Data.Seeders;
using LoadlyAggregator.Infrastructure.Extensions;
using LoadlyAggregator.Shared.Api;

try
{
    var builder = WebApplication.CreateBuilder(args);


    builder.Host.UseSerilogWithExporters(builder.Configuration, builder.Environment);

    var isCommandLineOperation = args.Length > 0 &&
        (args.Contains("--seed") ||
         (args.Length >= 3 && args[0] == "--load" && args[1] == "permission") ||
         args.Contains("--design-time"));


    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


    if (!isCommandLineOperation)
    {
        ConfigureRollbarInfrastructure(builder);


        builder.Services.AddRollbarLogger(loggerOptions =>
    {
        loggerOptions.Filter = (loggerName, loglevel) =>
        {

            if (loggerName != null && loggerName.Contains("Microsoft.EntityFrameworkCore.Query"))
            {
                return false;
            }


            if (loggerName != null && loggerName.StartsWith("Microsoft.EntityFrameworkCore"))
            {
                return loglevel >= LogLevel.Warning;
            }


            return loglevel >= LogLevel.Debug;
        };
    });
    }


    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<LoadlyAggregator.API.Filters.AsyncFluentValidationFilter>();
        options.Filters.Add<LoadlyAggregator.API.Filters.ApiResponseWrapperFilter>();
        options.ModelBinderProviders.Insert(0, new FileUploadModelBinderProvider());
    })
        .AddJsonOptions(options =>
        {
            // DateTime: see ApiDateTimeJsonSerializerOptions / ApiDateTimeContract (UTC on the wire, Z in JSON).
            ApiDateTimeJsonSerializerOptions.ApplyTo(options.JsonSerializerOptions);
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        })
        .AddApplicationPart(typeof(Program).Assembly);
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.CustomSchemaIds(type => type.FullName);
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Loadly Aggregator API",
            Version = "v1",
            Description = ApiDateTimeContract.JsonDescription
        });


        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
        });
    });


    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };


        options.Events = LoadlyAggregator.API.Authentication.JwtBearerEventsHandler.CreateEvents();
    });

    builder.Services.AddAuthorization(options =>
    {

        options.DefaultPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireAssertion(context =>
            {

                var mfaPending = context.User.FindFirst("mfa_pending")?.Value;
                return mfaPending != "true";
            })
            .Build();



        options.AddPolicy("MfaPending", policy =>
            policy.RequireAuthenticatedUser()
                  .RequireClaim("mfa_pending", "true")
                  .RequireClaim("scope", "mfa_only"));


        options.AddPolicy("CanManageUsers", policy =>
            policy.Requirements.Add(new LoadlyAggregator.Infrastructure.Authorization.PermissionRequirement("users.manage")));

        options.AddPolicy("CanViewReports", policy =>
            policy.Requirements.Add(new LoadlyAggregator.Infrastructure.Authorization.PermissionRequirement("reports.view")));

        options.AddPolicy("SuperAdmin", policy =>
            policy.Requirements.Add(new LoadlyAggregator.Infrastructure.Authorization.PermissionRequirement("*")));
    });




    builder.Services.AddInfrastructure(builder.Configuration);


    builder.Services.AddApiServices();

    // AutoMapper
    // MaxDepth(64) guards against CVE-2026-32933: unbounded recursion on cyclic graphs -> StackOverflow DoS.
    builder.Services.AddAutoMapper(cfg =>
    {
        cfg.Internal().ForAllMaps((typeMap, expr) =>
        {
            if (typeMap.MaxDepth == 0)
            {
                expr.MaxDepth(64);
            }
        });
    }, typeof(Program).Assembly);


    builder.Services.AddCors(options =>
    {
        static string[] GetAllowedOrigins(IConfiguration config)
        {
            var section = config.GetSection("Cors:AllowedOrigins");
            var value = section.Get<string>();
            return string.IsNullOrWhiteSpace(value)
                ? (section.Get<string[]>() ?? [])
                : value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        }

        options.AddPolicy("AllowAll", policy =>
        {
            if (builder.Environment.IsDevelopment())
                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            else
            {
                var origins = GetAllowedOrigins(builder.Configuration);
                if (origins.Length > 0)
                    policy.WithOrigins(origins).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
            }
        });
    });



    if (Environment.GetEnvironmentVariable("EF_DESIGN_TIME") == "true" ||
        args.Contains("--design-time"))
    {
        return;
    }

    WebApplication app;
    try
    {
        app = builder.Build();
    }
    catch (Microsoft.Extensions.Hosting.HostAbortedException)
    {

        return;
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }


    try
    {
        app.UseRollbarMiddleware();
    }
    catch (Exception)
    {

    }

    app.UseCorrelationId();
    app.UseSecurityHeaders();
    app.UseSerilogRequestLogging();
    app.UseCustomExceptionHandler();
    app.UseLanguageMiddleware();



    if (!app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("ForceHttpsRedirection"))
    {
        app.UseHttpsRedirection();
    }


    try
    {
        var cdnMainFolder = builder.Configuration["Media:CdnMainFolder"] ?? "loadly-images";
        var uploadsPath = builder.Configuration["Media:StoragePath"]
            ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");


        var absoluteUploadsPath = Path.IsPathRooted(uploadsPath)
            ? uploadsPath
            : Path.Combine(Directory.GetCurrentDirectory(), uploadsPath);
        var staticFilesPath = Path.Combine(absoluteUploadsPath, cdnMainFolder);


        if (!Directory.Exists(staticFilesPath))
        {
            Directory.CreateDirectory(staticFilesPath);
        }

        app.UseStaticFiles();
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(staticFilesPath),
            RequestPath = $"/{cdnMainFolder}"
        });
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Failed to configure static file serving. Application will continue without it.");
    }

    app.UseCors("AllowAll");
    app.UseAuthentication();
    app.UseUserTypeRoute();
    app.UseAuthorization();
    app.UseDriverLocation();
    app.MapControllers();
    app.MapHealthChecks("/health").AllowAnonymous();


    if (args.Length > 0)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {

            if (args.Contains("--seed"))
            {
                logger.LogInformation("Starting database seeding...");
                var seeder = services.GetRequiredService<DataSeeder>();
                await seeder.SeedAsync();
                logger.LogInformation("Database seeding completed successfully.");
                Environment.Exit(0);
            }


            if (args.Length >= 3 && args[0] == "--load" && args[1] == "permission")
            {
                var guardName = args[2].ToLower();

                if (!new[] { "admin", "company", "driver", "contractor", "collector" }.Contains(guardName))
                {
                    Console.WriteLine($"❌ Invalid guard: {guardName}");
                    Console.WriteLine("Valid guards: admin, company, driver, contractor, collector");
                    Environment.Exit(1);
                }

                logger.LogInformation($"Loading permissions for guard: {guardName}...");
                var permissionLoader = services.GetRequiredService<PermissionLoader>();

                if (guardName == "all")
                {
                    await permissionLoader.LoadAllPermissionsAsync();
                }
                else
                {
                    await permissionLoader.LoadPermissionsAsync(guardName);
                }

                logger.LogInformation("Permission loading completed successfully.");
                Environment.Exit(0);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while processing command.");
            Console.WriteLine($"❌ Error: {ex.Message}");
            Environment.Exit(1);
        }
    }

    app.Run();
}
catch (Exception ex)
{

    Console.WriteLine($"❌ Application failed to start: {ex.Message}");
    Console.WriteLine($"Details: {ex}");
    Environment.Exit(1);
}


void ConfigureRollbarInfrastructure(WebApplicationBuilder builder)
{
    var rollbarAccessToken = builder.Configuration["Rollbar:AccessToken"];
    var rollbarEnvironment = builder.Configuration["Rollbar:Environment"] ?? builder.Environment.EnvironmentName.ToLower();

    if (string.IsNullOrWhiteSpace(rollbarAccessToken))
    {
        // Rollbar is disabled when AccessToken is not set; no events will be sent.
        Serilog.Log.Debug("Rollbar skipped: Rollbar:AccessToken is not configured.");
        return;
    }

    try
    {
        RollbarInfrastructureConfig config = new RollbarInfrastructureConfig(
            rollbarAccessToken,
            rollbarEnvironment
        );

        RollbarDataSecurityOptions dataSecurityOptions = new RollbarDataSecurityOptions();
        dataSecurityOptions.ScrubFields = new string[]
        {
            "url",
            "method",
        };

        config.RollbarLoggerConfig.RollbarDataSecurityOptions.Reconfigure(dataSecurityOptions);

        RollbarInfrastructure.Instance.Init(config);

        // Send a one-time test message so you can verify Rollbar in the dashboard
        RollbarLocator.RollbarInstance.AsBlockingLogger(TimeSpan.FromSeconds(3))
            .Info("Loadly API started – Rollbar test message");
    }
    catch (Exception ex)
    {
        Serilog.Log.Warning(ex, "Rollbar initialization failed; error reporting to Rollbar will be disabled.");
    }
}
