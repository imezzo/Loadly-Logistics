namespace LoadlyAggregator.Infrastructure.Data.ACL;

public static class ContractorAcl
{
    public static Dictionary<string, List<string>> Permissions => new()
    {
        ["profile"] = new List<string>
        {
            "view_profile",
            "edit_profile",
        },

        ["trips"] = new List<string>
        {
            "view_trip",
        },

        ["documents"] = new List<string>
        {
            "view_document",
        },

        ["drivers"] = new List<string>
        {
            "view_driver",
        },
    };
}
