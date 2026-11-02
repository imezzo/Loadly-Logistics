using LoadlyAggregator.Domain.Common.Export;

namespace LoadlyAggregator.Domain.Admin.Export;

public sealed record DriverExportRow(
    string UserName,
    string FullName,
    string NationalId,
    string Phone,
    string Email,
    string City,
    string LicenseNo);

public static class DriverExportColumns
{
    public static IReadOnlyList<ExportColumn<DriverExportRow>> All { get; } =
    [
        new("userName", "User Name", "اسم المستخدم", (row, _) => row.UserName),
        new("fullName", "Full Name", "الاسم الكامل", (row, _) => row.FullName),
        new("nationalId", "National Id", "الرقم القومي", (row, _) => row.NationalId),
        new("phone", "Phone", "الهاتف", (row, _) => row.Phone),
        new("email", "Email", "البريد الإلكتروني", (row, _) => row.Email),
        new("city", "City", "المدينة", (row, _) => row.City),
        new("licenseNo", "License No", "رقم الرخصة", (row, _) => row.LicenseNo)
    ];
}
