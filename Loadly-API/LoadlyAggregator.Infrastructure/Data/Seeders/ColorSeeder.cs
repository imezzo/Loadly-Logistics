using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Infrastructure.Data.Seeders;

public class ColorSeeder : BaseLookupSeeder<Color>
{
    public ColorSeeder(ApplicationDbContext context) : base(context, "colors.json") { }
    public override async Task SeedAsync()
    {
        try
        {
            var seedData = JsonSerializer.Deserialize<List<Color>>(GetJsonContent()) ?? new List<Color>();
            if (!seedData.Any()) { LogSkipping("Colors (no data)"); return; }
            
            var existingColorDict = (await Context.Colors.ToListAsync()).ToDictionary(c => $"{c.NameEn}|{c.NameAr}", c => c);
            var colorsToAdd = new List<Color>();
            var colorsToUpdate = new List<Color>();
            foreach (var color in seedData)
            {
                var key = $"{color.NameEn}|{color.NameAr}";
                if (existingColorDict.TryGetValue(key, out var existing))
                {
                    existing.ColorHash = color.ColorHash;
                    existing.Status = UserStatus.ACTIVE;
                    colorsToUpdate.Add(existing);
                }
                else
                {
                    colorsToAdd.Add(
                        new Color { NameEn = color.NameEn, NameAr = color.NameAr, ColorHash = color.ColorHash, Status = UserStatus.ACTIVE }
                        );
                }
            }
            if (colorsToAdd.Any()) { await Context.Colors.AddRangeAsync(colorsToAdd); LogSeeding(colorsToAdd.Count, "colors (added)"); }
            if (colorsToUpdate.Any()) { Context.Colors.UpdateRange(colorsToUpdate); LogSeeding(colorsToUpdate.Count, "colors (updated)"); }
            if (colorsToAdd.Any() || colorsToUpdate.Any()) await Context.SaveChangesAsync();
        }
        catch (Exception ex) { LogError($"Error seeding colors: {ex.Message}"); throw; }
    }
}
