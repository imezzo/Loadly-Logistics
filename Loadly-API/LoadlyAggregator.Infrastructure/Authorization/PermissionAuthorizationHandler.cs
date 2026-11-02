using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Infrastructure.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            context.Fail();
            return;
        }

        using var scope = _serviceScopeFactory.CreateScope();
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
        var adminRepository = scope.ServiceProvider.GetRequiredService<IAdminRepository>();
        var companyRepository = scope.ServiceProvider.GetRequiredService<ICompanyRepository>();
        var driverRepository = scope.ServiceProvider.GetRequiredService<IDriverRepository>();
        var contractorRepository = scope.ServiceProvider.GetRequiredService<IContractorRepository>();
        var collectorRepository = scope.ServiceProvider.GetRequiredService<ICollectorRepository>();

        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userTypeClaim = context.User.FindFirst("userType")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userTypeClaim))
        {
            context.Fail();
            return;
        }

        if (!long.TryParse(userIdClaim, out var userId))
        {
            context.Fail();
            return;
        }

        bool hasPermission = userTypeClaim.ToLower() switch
        {
            "admin" => await CheckAdminPermission(permissionService, adminRepository, userId, requirement.Permission),
            "company" => await CheckCompanyPermission(permissionService, companyRepository, userId, requirement.Permission),
            "driver" => await CheckDriverPermission(permissionService, driverRepository, userId, requirement.Permission),
            "contractor" => await CheckContractorPermission(permissionService, contractorRepository, userId, requirement.Permission),
            "collector" => await CheckCollectorPermission(permissionService, collectorRepository, userId, requirement.Permission),
            _ => false
        };

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }

    private async Task<bool> CheckAdminPermission(
        IPermissionService permissionService,
        IAdminRepository adminRepository,
        long userId,
        string permission)
    {
        var user = await adminRepository.GetByIdAsync(userId);
        return user != null && await permissionService.CanAsync(user, permission);
    }

    private async Task<bool> CheckCompanyPermission(
        IPermissionService permissionService,
        ICompanyRepository companyRepository,
        long userId,
        string permission)
    {
        var user = await companyRepository.GetByIdAsync(userId);
        return user != null && await permissionService.CanAsync(user, permission);
    }

    private async Task<bool> CheckDriverPermission(
        IPermissionService permissionService,
        IDriverRepository driverRepository,
        long userId,
        string permission)
    {
        var user = await driverRepository.GetByIdAsync(userId);
        return user != null && await permissionService.CanAsync(user, permission);
    }

    private async Task<bool> CheckContractorPermission(
        IPermissionService permissionService,
        IContractorRepository contractorRepository,
        long userId,
        string permission)
    {
        var user = await contractorRepository.GetByIdAsync(userId);
        return user != null && await permissionService.CanAsync(user, permission);
    }

    private async Task<bool> CheckCollectorPermission(
        IPermissionService permissionService,
        ICollectorRepository collectorRepository,
        long userId,
        string permission)
    {
        var user = await collectorRepository.GetByIdAsync(userId);
        return user != null && await permissionService.CanAsync(user, permission);
    }
}

