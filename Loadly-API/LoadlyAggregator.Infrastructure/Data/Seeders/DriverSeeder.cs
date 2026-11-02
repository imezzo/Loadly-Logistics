using LoadlyAggregator.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Infrastructure.Services.Auth;

namespace LoadlyAggregator.Infrastructure.Data.Seeders;

public class DriverSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHashingService _passwordHashingService;

    public DriverSeeder(ApplicationDbContext context, IPasswordHashingService passwordHashingService)
    {
        _context = context;
        _passwordHashingService = passwordHashingService;
    }

    private const string TestDriverPhone = "01000665931";
    private const string NoPasswordDriverPhone = "01000665932";

    public async Task SeedAsync()
    {
        await SeedTestDriverAsync();
        await SeedNoPasswordDriverAsync();
    }

    private async Task SeedTestDriverAsync()
    {
        var existingDriver = await _context.Drivers
            .FirstOrDefaultAsync(d => d.Phone == TestDriverPhone);
        if (existingDriver != null)
        {
            Console.WriteLine($"⏭️  Driver with phone {TestDriverPhone} already exists. Skipping.");
            return;
        }

        var driver = new Driver
        {
            FullName = "Test Driver",
            FullNameAr = "سائق تجريبي",
            NationalId = "29901011234567",
            UserName = "driver-01000665931",
            Email = "driver-01000665931@loadly.test",
            Phone = TestDriverPhone,
            Password = _passwordHashingService.HashPassword("123456"),
            Status = UserStatus.ACTIVE,
            EmailVerified = true,
            PhoneVerified = true,
            MfaChannel = CommunicationChannel.None,
        };

        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();
        Console.WriteLine("✅ Test driver seeded successfully!");
        Console.WriteLine($"   Phone: {driver.Phone}");
        Console.WriteLine($"   Username: {driver.UserName}");
    }

    private async Task SeedNoPasswordDriverAsync()
    {
        var existingDriver = await _context.Drivers
            .FirstOrDefaultAsync(d => d.Phone == NoPasswordDriverPhone);
        if (existingDriver != null)
        {
            Console.WriteLine($"⏭️  Driver with phone {NoPasswordDriverPhone} (no password) already exists. Skipping.");
            return;
        }

        var driver = new Driver
        {
            FullName = "Driver No Password",
            FullNameAr = "سائق بدون كلمة مرور",
            NationalId = "29901011234568",
            UserName = "driver-01000665932",
            Email = "driver-01000665932@loadly.test",
            Phone = NoPasswordDriverPhone,
            Password = string.Empty,
            Status = UserStatus.ACTIVE,
            EmailVerified = true,
            PhoneVerified = true,
            MfaChannel = CommunicationChannel.None,
        };

        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();
        Console.WriteLine("✅ Driver without password seeded successfully!");
        Console.WriteLine($"   Phone: {driver.Phone}");
        Console.WriteLine($"   Username: {driver.UserName}");
        Console.WriteLine($"   Login with this phone to trigger OTP + redirect reset_password.");
    }
}
