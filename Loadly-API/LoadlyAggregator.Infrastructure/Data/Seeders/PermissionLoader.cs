using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Infrastructure.Data.ACL;

namespace LoadlyAggregator.Infrastructure.Data.Seeders;




public class PermissionLoader
{
    private readonly ApplicationDbContext _context;
    private readonly IPermissionService _permissionService;

    public PermissionLoader(ApplicationDbContext context, IPermissionService permissionService)
    {
        _context = context;
        _permissionService = permissionService;
    }




    public async Task LoadPermissionsAsync(string guardName)
    {
        guardName = guardName.ToLower();

        Dictionary<string, List<string>> permissions;

        switch (guardName)
        {
            case "admin":
                permissions = AdminAcl.Permissions;
                break;
            case "company":
                permissions = CompanyAcl.Permissions;
                break;
            case "driver":
                permissions = DriverAcl.Permissions;
                break;
            case "collector":
                permissions = CollectorAcl.Permissions;
                break;
            case "contractor":
                permissions = ContractorAcl.Permissions;
                break;
            default:
                throw new ArgumentException($"Unknown guard: {guardName}. Valid guards: admin, company, driver, collector");
        }

        Console.WriteLine($"\n🔐 Loading permissions for guard: {guardName}");
        Console.WriteLine($"📋 Found {permissions.Count} permission groups\n");

        var allPermissionNames = new List<string>();


        foreach (var group in permissions)
        {
            var groupName = group.Key;
            var groupPermissions = group.Value;

            Console.WriteLine($"  📁 Group: {groupName}");


            var permissionGroup = await _context.PermissionGroups
                .FirstOrDefaultAsync(pg => pg.Name == groupName && pg.Type == guardName);

            if (permissionGroup == null)
            {
                permissionGroup = new PermissionGroup
                {
                    Name = groupName,
                    NameAr = groupName,
                    Type = guardName,
                    Status = PermissionStatus.ENABLED,
                };
                _context.PermissionGroups.Add(permissionGroup);
                await _context.SaveChangesAsync();
                Console.WriteLine($"    ✅ Created permission group: {groupName}");
            }
            else
            {
                Console.WriteLine($"    ℹ️  Permission group already exists: {groupName}");
            }


            foreach (var permissionName in groupPermissions)
            {
                var existingPermission = await _context.Permissions
                    .FirstOrDefaultAsync(p => p.Name == permissionName && p.GuardName == guardName);

                if (existingPermission == null)
                {
                    try
                    {
                        var permission = await _permissionService.CreatePermissionAsync(
                            permissionName,
                            guardName,
                            permissionGroup.Id
                        );
                        allPermissionNames.Add(permissionName);
                        Console.WriteLine($"      ✅ Created permission: {permissionName}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"      ❌ Failed to create permission {permissionName}: {ex.Message}");
                    }
                }
                else
                {
                    allPermissionNames.Add(permissionName);
                    Console.WriteLine($"      ℹ️  Permission already exists: {permissionName}");
                }
            }
        }


        if (guardName == "admin")
        {
            await AssignPermissionsToSuperAdminAsync(allPermissionNames);
        }

        if (guardName == "company")
        {
            await AssignPermissionsToCompaniesSuperAdminsAsync();
        }

        Console.WriteLine($"\n✅ Successfully loaded {allPermissionNames.Count} permissions for guard: {guardName}\n");
    }




    private async Task AssignPermissionsToSuperAdminAsync(List<string> permissionNames)
    {
        var superAdmin = await _context.Admins
            .FirstOrDefaultAsync(a => a.Email == "super@sw-ag.com" ||
                                     a.UserName == "super-admin" ||
                                     a.Phone == "01000665931");

        if (superAdmin == null)
        {
            Console.WriteLine("⚠️  Super admin not found. Please seed super admin first using: dotnet run -- --seed");
            return;
        }

        Console.WriteLine($"\n👤 Assigning permissions to super admin: {superAdmin.UserName}");

        try
        {
            await _permissionService.GivePermissionsToAsync(superAdmin, permissionNames);
            Console.WriteLine($"✅ Successfully assigned {permissionNames.Count} permissions to super admin\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to assign permissions: {ex.Message}\n");
        }
    }

    private async Task AssignPermissionsToCompaniesSuperAdminsAsync()
    {
        var companiesSuperAdmins = await _context.Companies.Where(c => c.CompanyId == null && c.DeletedAt == null).ToListAsync();

        if (companiesSuperAdmins.Count == 0)
        {
            Console.WriteLine("⚠️  No parent companies (CompanyId == null) found to assign company permissions.");
            return;
        }

        Console.WriteLine($"\n🏢 Assigning all company permissions to {companiesSuperAdmins.Count} parent company account(s)");

        try
        {
            foreach (var company in companiesSuperAdmins)
            {
                await _permissionService.GiveAllGuardPermissionsAsync(company);
            }

            Console.WriteLine($"✅ Successfully assigned permissions to {companiesSuperAdmins.Count} parent company account(s)\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to assign permissions to parent companies: {ex.Message}\n");
        }
    }

    public async Task LoadAllPermissionsAsync()
    {
        await LoadPermissionsAsync("admin");
        await LoadPermissionsAsync("company");
        await LoadPermissionsAsync("driver");
        await LoadPermissionsAsync("contractor");
        await LoadPermissionsAsync("collector");
    }
}

