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
    public AdminSeeder(ApplicationDbContext context, IPasswordHashingService passwordHashingService)
    {
        _context = context;
        _passwordHashingService = passwordHashingService;
    }
    public async Task SeedAsync()
    {
        var existingAdmin = await _context.Admins
            .FirstOrDefaultAsync(a => a.Email == "super@sw-ag.com" ||
                                     a.UserName == "super-admin" ||
                                     a.Phone == "01000665931");
        if (existingAdmin != null)
        {
            Console.WriteLine("⏭️  Super admin already exists. Skipping.");
            return;
        }
        var superAdmin = new Admin
        {
            FullName = "Super Administrator",
            UserName = "super-admin",
            Email = "super@sw-ag.com",
            Phone = "01000665931",
            Password = _passwordHashingService.HashPassword("password@sw-ag"),
            Status = UserStatus.ACTIVE,
            EmailVerified = true,
            PhoneVerified = true,
            MfaChannel = CommunicationChannel.None,
        };
        _context.Admins.Add(superAdmin);
        await _context.SaveChangesAsync();
        Console.WriteLine("✅ Super admin seeded successfully!");
        Console.WriteLine($"   Email: {superAdmin.Email}");
        Console.WriteLine($"   Username: {superAdmin.UserName}");
        Console.WriteLine($"   Phone: {superAdmin.Phone}");
    }
}
