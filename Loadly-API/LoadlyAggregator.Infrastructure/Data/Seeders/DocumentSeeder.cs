using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Infrastructure.Data.Seeders;

public class DocumentSeeder : BaseLookupSeeder<Document>
{
    public DocumentSeeder(ApplicationDbContext context) : base(context, null) { }

    public override async Task SeedAsync()
    {
        try
        {
            var seedData = GetDocumentSeedData();
            if (!seedData.Any()) { LogSkipping("Documents (no data)"); return; }
            var existingDocumentDict = (await Context.Documents.ToListAsync()).ToDictionary(d => (d.NameEn, d.EntityId), d => d);
            var documentsToAdd = new List<Document>();
            var documentsToUpdate = new List<Document>();
            var processedKeys = new HashSet<(string, DocumentEntityType)>();
            foreach (var doc in seedData)
            {
                var key = (doc.NameEn, doc.EntityId);
                if (processedKeys.Contains(key)) continue;
                if (existingDocumentDict.TryGetValue(key, out var existing))
                {
                    existing.NameEn = doc.NameEn;
                    existing.NameAr = doc.NameAr;
                    existing.Required = doc.Required;
                    existing.Status = DocumentStatus.ACTIVE;
                    documentsToUpdate.Add(existing);
                }
                else
                {
                    documentsToAdd.Add(doc);
                }
                processedKeys.Add(key);
            }
            if (documentsToAdd.Any()) { await Context.Documents.AddRangeAsync(documentsToAdd); LogSeeding(documentsToAdd.Count, "documents (added)"); }
            if (documentsToUpdate.Any()) { Context.Documents.UpdateRange(documentsToUpdate); LogSeeding(documentsToUpdate.Count, "documents (updated)"); }
            if (documentsToAdd.Any() || documentsToUpdate.Any()) await Context.SaveChangesAsync();
        }
        catch (Exception ex) { LogError($"Error seeding documents: {ex.Message}"); throw; }
    }

    private static List<Document> GetDocumentSeedData() => new()
    {
        new() { EntityId = DocumentEntityType.VEHICLE, NameEn = "Vehicle Registration", NameAr = "رخصة المركبة", Required = true, Status = DocumentStatus.ACTIVE },
        new() { EntityId = DocumentEntityType.VEHICLE, NameEn = "Insurance Certificate", NameAr = "شهادة التأمين", Required = true, Status = DocumentStatus.ACTIVE },
        new() { EntityId = DocumentEntityType.VEHICLE, NameEn = "Inspection Certificate", NameAr = "شهادة الفحص", Required = true, Status = DocumentStatus.ACTIVE },

        new() { EntityId = DocumentEntityType.COMPANY, NameEn = "Commercial registry certificate", NameAr = "شهادة السجل التجاري", Required = true, Status = DocumentStatus.ACTIVE },
        new() { EntityId = DocumentEntityType.COMPANY, NameEn = "Tax registration certificate", NameAr = "شهادة التسجيل الضريبي", Required = true, Status = DocumentStatus.ACTIVE },
        new() { EntityId = DocumentEntityType.COMPANY, NameEn = "company profile", NameAr = "الملف التعريفي للشركة", Required = false, Status = DocumentStatus.ACTIVE },
        new() { EntityId = DocumentEntityType.COMPANY, NameEn = "authorized signatory list", NameAr = "قائمة المفوضين بالتوقيع", Required = false, Status = DocumentStatus.ACTIVE },
        new() { EntityId = DocumentEntityType.COMPANY, NameEn = "special handling certifications", NameAr = "شهادات المعالجة الخاصة", Required = false, Status = DocumentStatus.ACTIVE },

        new() { EntityId = DocumentEntityType.DRIVER, NameEn = "Driving license", NameAr = "رخصة القيادة", Required = false, Status = DocumentStatus.ACTIVE },
        new() { EntityId = DocumentEntityType.DRIVER, NameEn = "Professional Driving License", NameAr = "رخصة القيادة المهنية", Required = false, Status = DocumentStatus.ACTIVE },
        new() { EntityId = DocumentEntityType.DRIVER, NameEn = "National ID", NameAr = "الرقم القومي", Required = false, Status = DocumentStatus.ACTIVE },
        new() { EntityId = DocumentEntityType.DRIVER, NameEn = "Medical certificate", NameAr = "شهادة الصحية", Required = false, Status = DocumentStatus.ACTIVE },
        new() { EntityId = DocumentEntityType.DRIVER, NameEn = "Criminal record check", NameAr = "السجل الجنائي", Required = false, Status = DocumentStatus.ACTIVE },
        new() { EntityId = DocumentEntityType.DRIVER, NameEn = "Employment contract", NameAr = "عقد التوظيف", Required = false, Status = DocumentStatus.ACTIVE },
        new() { EntityId = DocumentEntityType.DRIVER, NameEn = "Emergency contact form", NameAr = "طلب الاتصال الطوارئ (فقط للسائقين الموظفين)", Required = false, Status = DocumentStatus.ACTIVE },
        new() { EntityId = DocumentEntityType.DRIVER, NameEn = "Insurance Policy", NameAr = "سياسة التأمين", Required = false, Status = DocumentStatus.ACTIVE },
        new() { EntityId = DocumentEntityType.DRIVER, NameEn = "Safety Training", NameAr = "تدريب الأمان", Required = false, Status = DocumentStatus.ACTIVE },
        new() { EntityId = DocumentEntityType.DRIVER, NameEn = "First Aid Certification", NameAr = "شهادة الإسعافات الأولية", Required = false, Status = DocumentStatus.ACTIVE },
        new() { EntityId = DocumentEntityType.DRIVER, NameEn = "Company Orientation", NameAr = "توضيح الشركة", Required = false, Status = DocumentStatus.ACTIVE },

        new() { EntityId = DocumentEntityType.CONTRACTOR_DRIVER, NameEn = "ID Card, Driving License and Vehicle License", NameAr = "البطاقة و رخصة القيادة و رخصة العربية", Required = false, Status = DocumentStatus.ACTIVE },

        new() { EntityId = DocumentEntityType.TRIP, NameEn = "Receipt Note Image", NameAr = "صورة إيصال الاستلام", Required = false, Status = DocumentStatus.ACTIVE },
        new() { EntityId = DocumentEntityType.TRIP, NameEn = "Delivery Note Image", NameAr = "صورة مذكرة التسليم", Required = false, Status = DocumentStatus.ACTIVE },
        new() { EntityId = DocumentEntityType.TRIP, NameEn = "Tollgate Images", NameAr = "بوابات وموازين", Required = false, Status = DocumentStatus.ACTIVE },
        new() { EntityId = DocumentEntityType.TRIP, NameEn = "Weighbridge Ticket - Pickup", NameAr = "تذكرة الميزان - التحميل", Required = false, Status = DocumentStatus.ACTIVE },
        new() { EntityId = DocumentEntityType.TRIP, NameEn = "Weighbridge Ticket - Delivery", NameAr = "تذكرة الميزان - التسليم", Required = false, Status = DocumentStatus.ACTIVE },
        new() { EntityId = DocumentEntityType.TRIP, NameEn = "Cargo Inspection Report", NameAr = "تقرير فحص الشحنة", Required = false, Status = DocumentStatus.ACTIVE },
        new() { EntityId = DocumentEntityType.TRIP, NameEn = "Customs Documents", NameAr = "المستندات الجمركية", Required = false, Status = DocumentStatus.ACTIVE },
        new() { EntityId = DocumentEntityType.TRIP, NameEn = "Fuel Receipts", NameAr = "إيصالات الوقود", Required = false, Status = DocumentStatus.ACTIVE },

        new() { EntityId = DocumentEntityType.CONTRACTOR, NameEn = "National ID", NameAr = "الرقم القومي", Required = true, Status = DocumentStatus.ACTIVE },
        new() { EntityId = DocumentEntityType.CONTRACTOR, NameEn = "Commercial Registration", NameAr = "السجيل التجاري", Required = false, Status = DocumentStatus.ACTIVE },
        new() { EntityId = DocumentEntityType.CONTRACTOR, NameEn = "Tax Card", NameAr = "البطاقة الضريبية", Required = false, Status = DocumentStatus.ACTIVE },

        new() { EntityId = DocumentEntityType.CONTRACT, NameEn = "Contract Conditions", NameAr = "شروط العقد", Required = false, Status = DocumentStatus.ACTIVE },

    };
}
