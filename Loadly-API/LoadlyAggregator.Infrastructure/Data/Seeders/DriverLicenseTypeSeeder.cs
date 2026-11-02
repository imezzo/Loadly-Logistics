using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Infrastructure.Data.Seeders;

public class DriverLicenseTypeSeeder : BaseLookupSeeder<DriverLicenseType>
{
    public DriverLicenseTypeSeeder(ApplicationDbContext context) : base(context, null) { }
    public override async Task SeedAsync()
    {
        try
        {
            var seedData = GetDriverLicenseTypeSeedData();
            if (!seedData.Any()) { LogSkipping("Driver license types (no data)"); return; }
            var existingTypeDict = (await Context.DriverLicenseTypes.ToListAsync()).ToDictionary(d => (d.NameEn, d.NameAr), d => d);
            var typesToAdd = new List<DriverLicenseType>();
            var typesToUpdate = new List<DriverLicenseType>();
            var processedKeys = new HashSet<(string, string)>();
            foreach (var driverLicenseType in seedData)
            {
                var key = (driverLicenseType.NameEn, driverLicenseType.NameAr);
                if (processedKeys.Contains(key)) continue;
                if (existingTypeDict.TryGetValue(key, out var existing))
                {
                    existing.NameEn = driverLicenseType.NameEn;
                    existing.NameAr = driverLicenseType.NameAr;
                    typesToUpdate.Add(existing);
                }
                else
                {
                    typesToAdd.Add(driverLicenseType);
                }
                processedKeys.Add(key);
            }
            if (typesToAdd.Any()) { await Context.DriverLicenseTypes.AddRangeAsync(typesToAdd); LogSeeding(typesToAdd.Count, "driver license types (added)"); }
            if (typesToUpdate.Any()) { Context.DriverLicenseTypes.UpdateRange(typesToUpdate); LogSeeding(typesToUpdate.Count, "driver license types (updated)"); }
            if (typesToAdd.Any() || typesToUpdate.Any()) await Context.SaveChangesAsync();
        }
        catch (Exception ex) { LogError($"Error seeding driver license types: {ex.Message}"); throw; }
    }
    private static List<DriverLicenseType> GetDriverLicenseTypeSeedData() => new()
    {
        new() { NameEn = "Professional", NameAr = "القيادة المهنية" },
        new() { NameEn = "Commercial", NameAr = "القيادة التجارية" }
    };
}
