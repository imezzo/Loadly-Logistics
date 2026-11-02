using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Infrastructure.Data.Seeders;

public class ServiceRequirementSeeder : BaseLookupSeeder<ServiceRequirement>
{
    public ServiceRequirementSeeder(ApplicationDbContext context) : base(context, null) { }
    public override async Task SeedAsync()
    {
        try
        {
            var seedData = GetServiceRequirementSeedData();
            if (!seedData.Any()) { LogSkipping("Service requirements (no data)"); return; }
            var existingRequirementDict = (await Context.ServiceRequirements.ToListAsync()).ToDictionary(s => (s.NameEn, s.NameAr), s => s);
            var requirementsToAdd = new List<ServiceRequirement>();
            var requirementsToUpdate = new List<ServiceRequirement>();
            var processedKeys = new HashSet<(string, string)>();
            foreach (var serviceRequirement in seedData)
            {
                var key = (serviceRequirement.NameEn, serviceRequirement.NameAr);
                if (processedKeys.Contains(key)) continue;
                if (existingRequirementDict.TryGetValue(key, out var existing))
                {
                    existing.NameEn = serviceRequirement.NameEn;
                    existing.NameAr = serviceRequirement.NameAr;
                    requirementsToUpdate.Add(existing);
                }
                else
                {
                    requirementsToAdd.Add(serviceRequirement);
                }
                processedKeys.Add(key);
            }
            if (requirementsToAdd.Any()) { await Context.ServiceRequirements.AddRangeAsync(requirementsToAdd); LogSeeding(requirementsToAdd.Count, "service requirements (added)"); }
            if (requirementsToUpdate.Any()) { Context.ServiceRequirements.UpdateRange(requirementsToUpdate); LogSeeding(requirementsToUpdate.Count, "service requirements (updated)"); }
            if (requirementsToAdd.Any() || requirementsToUpdate.Any()) await Context.SaveChangesAsync();
        }
        catch (Exception ex) { LogError($"Error seeding service requirements: {ex.Message}"); throw; }
    }
    private static List<ServiceRequirement> GetServiceRequirementSeedData() => new()
    {
       new() { NameEn = "Regular Delivery", NameAr = "التوصيل العادي" },
       new() { NameEn = "Express Service", NameAr = "خدمة سريعة" },
       new() { NameEn = "Specialized Transport", NameAr = "النقل المتخصص" },
       new() { NameEn = "Cold Chain", NameAr = "سلسلة التبريد" },
       new() { NameEn = "Fragile Goods", NameAr = "البضائع القابلة للكسر" }
    };
}
