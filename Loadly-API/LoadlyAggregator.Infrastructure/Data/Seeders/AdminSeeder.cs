using LoadlyAggregator.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Infrastructure.Services.Auth;
namespace LoadlyAggregator.Infrastructure.Data.Seeders;

public class AdminSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHashingService _passwordHashingService;

    // shared login for dashboards / ops
    public const string SuperAdminEmail = "admin@loadly.com";
    public const string SuperAdminUserName = "admin";
    public const string SuperAdminPhone = "01000000001";
    public const string SuperAdminPassword = "Loadly@Admin123";

    public AdminSeeder(ApplicationDbContext context, IPasswordHashingService passwordHashingService)
    {
        _context = context;
        _passwordHashingService = passwordHashingService;
    }

    public async Task SeedAsync()
    {
        await SeedAdminAsync(
            fullName: "Loadly Super Admin",
            userName: SuperAdminUserName,
            email: SuperAdminEmail,
            phone: SuperAdminPhone,
            password: SuperAdminPassword);

        await SeedAdminAsync(
            fullName: "Loadly Ops Admin",
            userName: "ops-admin",
            email: "ops@loadly.com",
            phone: "01000000002",
            password: "Loadly@Ops123");
    }

    private async Task SeedAdminAsync(
        string fullName,
        string userName,
        string email,
        string phone,
        string password)
    {
        var existing = await _context.Admins
            .FirstOrDefaultAsync(a =>
                a.Email == email ||
                a.UserName == userName ||
                a.Phone == phone);

        if (existing != null)
        {
            // keep password/login usable if row already there from older seed
            existing.FullName = fullName;
            existing.UserName = userName;
            existing.Email = email;
            existing.Phone = phone;
            existing.Password = _passwordHashingService.HashPassword(password);
            existing.Status = UserStatus.ACTIVE;
            existing.EmailVerified = true;
            existing.PhoneVerified = true;
            existing.MfaChannel = CommunicationChannel.None;
            await _context.SaveChangesAsync();
            Console.WriteLine($"⏭️  Admin {userName} updated (password reset).");
            return;
        }

        var admin = new Admin
        {
            FullName = fullName,
            UserName = userName,
            Email = email,
            Phone = phone,
            Password = _passwordHashingService.HashPassword(password),
            Status = UserStatus.ACTIVE,
            EmailVerified = true,
            PhoneVerified = true,
            MfaChannel = CommunicationChannel.None,
        };
        _context.Admins.Add(admin);
        await _context.SaveChangesAsync();
        Console.WriteLine($"✅ Admin seeded: {email} / {userName}");
    }
}
