using Microsoft.Extensions.Configuration;
using LoadlyAggregator.Infrastructure.Services.Auth;

namespace LoadlyAggregator.Infrastructure.Data.Seeders;

public class DataSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IConfiguration _configuration;

    public DataSeeder(
        ApplicationDbContext context,
        IPasswordHashingService passwordHashingService,
        IConfiguration configuration)
    {
        _context = context;
        _passwordHashingService = passwordHashingService;
        _configuration = configuration;
    }

    public async Task SeedAsync()
    {
        Console.WriteLine("🌱 Starting database seeding...\n");
        try
        {
            await new GovernorateSeeder(_context).SeedAsync();
            await new CitySeeder(_context).SeedAsync();
            await new DistrictSeeder(_context).SeedAsync();
            await new IndustryTypeSeeder(_context).SeedAsync();
            await new SectorSeeder(_context).SeedAsync();
            await new CommunicationMethodSeeder(_context).SeedAsync();
            await new ServiceRequirementSeeder(_context).SeedAsync();
            await new ColorSeeder(_context).SeedAsync();
            await new DocumentStatusSeeder(_context).SeedAsync();
            await new DocumentSeeder(_context).SeedAsync();
            await new DriverLicenseTypeSeeder(_context).SeedAsync();
            await new AdminSeeder(_context, _passwordHashingService).SeedAsync();
            await new CompanySeeder(_context, _passwordHashingService).SeedAsync();
            await new DriverSeeder(_context, _passwordHashingService).SeedAsync();
            await new GeneralSettingsSeeder(_context, _configuration).SeedAsync();
            Console.WriteLine("\n✅ Database seeding completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Database seeding failed: {ex.Message}");
            throw;
        }
    }
}
