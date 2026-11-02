using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class ContractBillingSnapshotNameFormatter
{
    public const string DefaultPrefix = "يوميات عقد";
    public const int MaxLength = 500;

    public static string Resolve(Contract contract, string? requestedName)
    {
        if (!string.IsNullOrWhiteSpace(requestedName))
            return TrimToMax(requestedName.Trim());

        return BuildDefault(contract);
    }

    public static string BuildDefault(Contract contract)
    {
        var companyName = contract.Company?.CompanyDetail?.NameAr;
        if (string.IsNullOrWhiteSpace(companyName))
            companyName = contract.Company?.CompanyDetail?.NameEn;

        var reference = contract.ReferenceNumber?.Trim() ?? string.Empty;
        var parts = new List<string> { DefaultPrefix };
        if (!string.IsNullOrWhiteSpace(companyName))
            parts.Add(companyName.Trim());
        if (!string.IsNullOrWhiteSpace(reference))
            parts.Add(reference);

        return TrimToMax(string.Join(' ', parts));
    }

    private static string TrimToMax(string value) =>
        value.Length <= MaxLength ? value : value[..MaxLength];
}
