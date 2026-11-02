using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Infrastructure.Data.Seeders;

public class IndustryTypeSeeder : BaseLookupSeeder<IndustryType>
{
    public IndustryTypeSeeder(ApplicationDbContext context) : base(context, null) { }
    public override async Task SeedAsync()
    {
        try
        {
            var seedData = GetIndustryTypeSeedData();
            if (!seedData.Any()) { LogSkipping("Industry types (no data)"); return; }
            var existingTypeDict = (await Context.IndustryTypes.ToListAsync()).ToDictionary(i => (i.NameEn, i.NameAr), i => i);
            var typesToAdd = new List<IndustryType>();
            var typesToUpdate = new List<IndustryType>();
            var processedKeys = new HashSet<(string, string)>();
            foreach (var industryType in seedData)
            {
                var key = (industryType.NameEn, industryType.NameAr);
                if (processedKeys.Contains(key)) continue;
                if (existingTypeDict.TryGetValue(key, out var existing))
                {
                    existing.NameEn = industryType.NameEn;
                    existing.NameAr = industryType.NameAr;
                    typesToUpdate.Add(existing);
                }
                else
                {
                    typesToAdd.Add(industryType);
                }
                processedKeys.Add(key);
            }
            if (typesToAdd.Any()) { await Context.IndustryTypes.AddRangeAsync(typesToAdd); LogSeeding(typesToAdd.Count, "industry types (added)"); }
            if (typesToUpdate.Any()) { Context.IndustryTypes.UpdateRange(typesToUpdate); LogSeeding(typesToUpdate.Count, "industry types (updated)"); }
            if (typesToAdd.Any() || typesToUpdate.Any()) await Context.SaveChangesAsync();
        }
        catch (Exception ex) { LogError($"Error seeding industry types: {ex.Message}"); throw; }
    }
    private static List<IndustryType> GetIndustryTypeSeedData() => new()
    {
       new() { NameEn = "Manufacturing", NameAr = "التصنيع" },
       new() { NameEn = "Retail", NameAr = "التجزئة" },
       new() { NameEn = "E-commerce", NameAr = "التجارة الإلكترونية" },
       new() { NameEn = "Healthcare", NameAr = "الرعاية الصحية" },
       new() { NameEn = "Food & Beverage", NameAr = "الطعام والشراب" },
       new() { NameEn = "Construction", NameAr = "البناء" },
       new() { NameEn = "Other", NameAr = "أخرى" }
    };
}
