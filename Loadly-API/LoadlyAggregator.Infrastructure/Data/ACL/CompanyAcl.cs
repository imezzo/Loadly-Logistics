namespace LoadlyAggregator.Infrastructure.Data.ACL;



public static class CompanyAcl
{
    public static Dictionary<string, List<string>> Permissions => new()
    {
        
        ["trips"] = new List<string>
        {
            "view_trip",
            "create_trip",
            "edit_trip",
            "delete_trip",
        },

        ["contracts"] = new List<string>
        {
            "view_contract",
        },

        ["invoice"] = new List<string>
        {
            "view_invoice",
        },
        
        ["dashboard"] = new List<string>
        {
            "view_dashboard",
        },

        ["users"] = new List<string>
        {
            "view_user",
            "create_user",
            "edit_user",
            "delete_user",
        },

        ["permissions"] = new List<string>
        {
            "manage_permissions",
        },
    };
}
