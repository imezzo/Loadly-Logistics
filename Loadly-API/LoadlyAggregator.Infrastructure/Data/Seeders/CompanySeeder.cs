using LoadlyAggregator.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Infrastructure.Services.Auth;
namespace LoadlyAggregator.Infrastructure.Data.Seeders;

public class CompanySeeder : BaseLookupSeeder<Company>
{
    private readonly IPasswordHashingService _passwordHashingService;
    public CompanySeeder(ApplicationDbContext context, IPasswordHashingService passwordHashingService) : base(context, null)
    {
        _passwordHashingService = passwordHashingService;
    }
    public override async Task SeedAsync()
    {
        try
        {
            var seedData = GetCompanySeedData();
            if (!seedData.Any()) { LogSkipping("Companies (no data)"); return; }
            var existingCompanyDict = (await Context.Companies.ToListAsync()).ToDictionary(c => c.UserName.ToLower(), c => c);
            var companiesToAdd = new List<Company>();
            var companiesToUpdate = new List<Company>();
            var processedKeys = new HashSet<string>();
            foreach (var company in seedData)
            {
                var key = company.UserName.ToLower();
                if (processedKeys.Contains(key)) continue;
                if (existingCompanyDict.TryGetValue(key, out var existing))
                {
                    existing.FullName = company.FullName;
                    existing.Email = company.Email;
                    existing.Phone = company.Phone;
                    existing.Status = company.Status;
                    existing.EmailVerified = company.EmailVerified;
                    existing.PhoneVerified = company.PhoneVerified;
                    existing.MfaChannel = company.MfaChannel;
                    existing.Type = company.Type;
                    companiesToUpdate.Add(existing);
                }
                else
                {
                    companiesToAdd.Add(company);
                }
                processedKeys.Add(key);
            }
            if (companiesToAdd.Any()) { await Context.Companies.AddRangeAsync(companiesToAdd); LogSeeding(companiesToAdd.Count, "companies (added)"); }
            if (companiesToUpdate.Any()) { Context.Companies.UpdateRange(companiesToUpdate); LogSeeding(companiesToUpdate.Count, "companies (updated)"); }
            if (companiesToAdd.Any() || companiesToUpdate.Any()) await Context.SaveChangesAsync();
            else LogSkipping("Companies (no changes)");
        }
        catch (Exception ex) { LogError($"Error seeding companies: {ex.Message}"); throw; }
    }
    private List<Company> GetCompanySeedData() => new()
    {
        new()
        {
            FullName = "Loadly Tech Solutions", UserName = "loadly-tech",
            Email = "info@loadlytech.com", Phone = "01001234567",
            Password = _passwordHashingService.HashPassword("password123"),
            Status = UserStatus.ACTIVE, EmailVerified = true, PhoneVerified = true,
            MfaChannel = CommunicationChannel.None, Type = CompanyType.COMPANY,
            CompanyId = null
        },
        new()
        {
            FullName = "Cairo Logistics Co.", UserName = "cairo-logistics",
            Email = "contact@cairologistics.com", Phone = "01002345678",
            Password = _passwordHashingService.HashPassword("password123"),
            Status = UserStatus.ACTIVE, EmailVerified = true, PhoneVerified = true,
            MfaChannel = CommunicationChannel.None, Type = CompanyType.COMPANY,
            CompanyId = null
        },
        new()
        {
            FullName = "Alexandria Trading", UserName = "alex-trading",
            Email = "info@alextrading.com", Phone = "01003456789",
            Password = _passwordHashingService.HashPassword("password123"),
            Status = UserStatus.ACTIVE, EmailVerified = true, PhoneVerified = false,
            MfaChannel = CommunicationChannel.None, Type = CompanyType.COMPANY,
            CompanyId = null
        },
        new()
        {
            FullName = "Giza Manufacturing", UserName = "giza-manufacturing",
            Email = "sales@gizamanufacturing.com", Phone = "01004567890",
            Password = _passwordHashingService.HashPassword("password123"),
            Status = UserStatus.ACTIVE, EmailVerified = false, PhoneVerified = true,
            MfaChannel = CommunicationChannel.None, Type = CompanyType.COMPANY,
            CompanyId = null
        },
        new()
        {
            FullName = "Delta Services", UserName = "delta-services",
            Email = "hello@deltaservices.com", Phone = "01005678901",
            Password = _passwordHashingService.HashPassword("password123"),
            Status = UserStatus.SUSPENDED, EmailVerified = true, PhoneVerified = true,
            MfaChannel = CommunicationChannel.None, Type = CompanyType.COMPANY,
            CompanyId = null
        }
    };
}
