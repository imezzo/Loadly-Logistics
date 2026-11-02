namespace LoadlyAggregator.Infrastructure.Data.ACL;

public static class CollectorAcl
{
    public static Dictionary<string, List<string>> Permissions => new()
    {
        ["profile"] = new List<string>
        {
            "view_profile",
            "edit_profile",
        },

        ["documents"] = new List<string>
        {
            "view_document",
        },

        ["trips"] = new List<string>
        {
            "view_trip",
            "collect_trip_documents",
            "confirm_trip_documents",
        },
    };
}
