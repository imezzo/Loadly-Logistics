using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
namespace LoadlyAggregator.Infrastructure.Data.Seeders;

public class CommunicationMethodSeeder : BaseLookupSeeder<CommunicationMethod>
{
    public CommunicationMethodSeeder(ApplicationDbContext context) : base(context, null) { }
    public override async Task SeedAsync()
    {
        try
        {
            var seedData = GetCommunicationMethodSeedData();
            if (!seedData.Any()) { LogSkipping("Communication methods (no data)"); return; }
            var existingMethodDict = (await Context.CommunicationMethods.ToListAsync()).ToDictionary(c => (c.NameEn, c.NameAr), c => c);
            var methodsToAdd = new List<CommunicationMethod>();
            var methodsToUpdate = new List<CommunicationMethod>();
            var processedKeys = new HashSet<(string, string)>();
            foreach (var communicationMethod in seedData)
            {
                var key = (communicationMethod.NameEn, communicationMethod.NameAr);
                if (processedKeys.Contains(key)) continue;
                if (existingMethodDict.TryGetValue(key, out var existing))
                {
                    existing.NameEn = communicationMethod.NameEn;
                    existing.NameAr = communicationMethod.NameAr;
                    methodsToUpdate.Add(existing);
                }
                else
                {
                    methodsToAdd.Add(communicationMethod);
                }
                processedKeys.Add(key);
            }
            if (methodsToAdd.Any()) { await Context.CommunicationMethods.AddRangeAsync(methodsToAdd); LogSeeding(methodsToAdd.Count, "communication methods (added)"); }
            if (methodsToUpdate.Any()) { Context.CommunicationMethods.UpdateRange(methodsToUpdate); LogSeeding(methodsToUpdate.Count, "communication methods (updated)"); }
            if (methodsToAdd.Any() || methodsToUpdate.Any()) await Context.SaveChangesAsync();
        }
        catch (Exception ex) { LogError($"Error seeding communication methods: {ex.Message}"); throw; }
    }
    private static List<CommunicationMethod> GetCommunicationMethodSeedData() => new()
    {
        new() { NameEn = "Email", NameAr = "البريد الإلكتروني" },
        new() { NameEn = "Phone Call", NameAr = "مكالمة هاتفية" },
        new() { NameEn = "SMS", NameAr = "رسالة نصية" },
        new() { NameEn = "WhatsApp", NameAr = "واتساب" },
        new() { NameEn = "In-Person Meeting", NameAr = "لقاء شخصي" },
        new() { NameEn = "Video Call", NameAr = "مكالمة فيديو" }
    };
}
