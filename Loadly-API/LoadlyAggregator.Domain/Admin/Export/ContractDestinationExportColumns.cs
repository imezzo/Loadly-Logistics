using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Export;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Domain.Admin.Export;

public static class ContractDestinationExportColumns
{
    private const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss";

    public static IReadOnlyList<ExportColumn<ContractDestination>> All { get; } =
    [
        new("id", "Id", "المعرف", (destination, _) => destination.Id),
        new("nameEn", "Name En", "الاسم بالإنجليزية", (destination, _) => destination.NameEn),
        new("nameAr", "Name Ar", "الاسم بالعربية", (destination, _) => destination.NameAr),
        new("expectedKms", "Expected Kms", "الكيلومترات المتوقعة", (destination, _) => destination.ExpectedKms),
        new("returnExpectedKms", "Return Expected Kms", "كيلومترات العودة المتوقعة", (destination, _) => destination.ReturnExpectedKms),
        new("numberOfDays", "Number Of Days", "عدد الأيام", (destination, _) => destination.NumberOfDays),
        new("pricePerTon", "Price Per Ton", "السعر للطن", (destination, _) => destination.PricePerTon),
        new("fixedTripPrice", "Fixed Trip Price", "سعر الرحلة الثابت", (destination, _) => destination.FixedTripPrice),
        new("createdAt", "Created At", "تاريخ الإنشاء", (destination, _) => EgyptTime.Format(destination.CreatedAt, DateTimeFormat)),
        new("updatedAt", "Updated At", "تاريخ التحديث", (destination, _) => EgyptTime.Format(destination.UpdatedAt, DateTimeFormat))
    ];
}
