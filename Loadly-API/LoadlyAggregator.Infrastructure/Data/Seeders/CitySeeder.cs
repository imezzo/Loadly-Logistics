using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Infrastructure.Data.Seeders;

public class CitySeeder : BaseLookupSeeder<City>
{
    public CitySeeder(ApplicationDbContext context) : base(context, "cities.json") { }
    public override async Task SeedAsync()
    {
        try
        {
            var seedData = JsonSerializer.Deserialize<List<CitySeedDto>>(GetJsonContent()) ?? new List<CitySeedDto>();
            if (!seedData.Any()) { LogSkipping("Cities (no data)"); return; }
            
            var governoratesByNameEn = (await Context.Governerates.ToListAsync()).ToDictionary(g => g.NameEn, g => g);
            var existingCitiesDict = (await Context.Cities.ToListAsync()).ToDictionary(c => (c.NameEn, c.GovernerateId), c => c);
            var citiesToAdd = new List<City>();
            var citiesToUpdate = new List<City>();
            var processedKeys = new HashSet<(string, long)>();
            foreach (var dto in seedData)
            {
                if (!governoratesByNameEn.TryGetValue(dto.GovernorateNameEn, out var governorate))
                { LogError($"Governorate '{dto.GovernorateNameEn}' not found for city '{dto.NameEn}'"); continue; }
                var key = (dto.NameEn, governorate.Id);
                if (processedKeys.Contains(key)) continue;
                if (existingCitiesDict.TryGetValue(key, out var existingCity))
                {
                    existingCity.NameAr = dto.NameAr;
                    citiesToUpdate.Add(existingCity);
                }
                else
                {
                    citiesToAdd.Add(
                        new City { NameEn = dto.NameEn, NameAr = dto.NameAr, GovernerateId = governorate.Id }
                        );
                }
                processedKeys.Add(key);
            }
            if (citiesToAdd.Any()) { await Context.Cities.AddRangeAsync(citiesToAdd); LogSeeding(citiesToAdd.Count, "cities (added)"); }
            if (citiesToUpdate.Any()) { Context.Cities.UpdateRange(citiesToUpdate); LogSeeding(citiesToUpdate.Count, "cities (updated)"); }
            if (citiesToAdd.Any() || citiesToUpdate.Any()) await Context.SaveChangesAsync();
        }
        catch (Exception ex) { LogError($"Error seeding cities: {ex.Message}"); throw; }
    }
    private class CitySeedDto
    {
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string GovernorateNameEn { get; set; } = string.Empty;
    }
}
