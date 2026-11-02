namespace LoadlyAggregator.Shared.Constants;

public static class BankTransferPayerTypes
{
    public const string Company = "Company";
    public const string Lead = "Lead";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        Company,
        Lead
    };
}
