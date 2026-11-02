using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Infrastructure.Authorization;





[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class HasPermissionAttribute : TypeFilterAttribute
{
    public HasPermissionAttribute(string permission) : base(typeof(PermissionFilter))
    {
        Arguments = new object[] { permission };
    }

    private class PermissionFilter : IAsyncAuthorizationFilter
    {
        private readonly string _permission;

        public PermissionFilter(string permission)
        {
            _permission = permission;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {

            if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var permissionService = context.HttpContext.RequestServices
                .GetRequiredService<IPermissionService>();
            var dbContext = context.HttpContext.RequestServices
                .GetRequiredService<ApplicationDbContext>();


            var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userTypeClaim = context.HttpContext.User.FindFirst("userType")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userTypeClaim))
            {
                context.Result = new ForbidResult();
                return;
            }

            if (!long.TryParse(userIdClaim, out var userId))
            {
                context.Result = new ForbidResult();
                return;
            }


            bool hasPermission = userTypeClaim.ToLower() switch
            {
                "admin" => await CheckPermissionForAdmin(permissionService, dbContext, userId, _permission),
                "company" => await CheckPermissionForCompany(permissionService, dbContext, userId, _permission),
                "driver" => await CheckPermissionForDriver(permissionService, dbContext, userId, _permission),
                "collector" => await CheckPermissionForCollector(permissionService, dbContext, userId, _permission),
                _ => false
            };

            if (!hasPermission)
            {
                var response = ApiResponse<object>.Error("Forbidden: Insufficient permissions", null);
                context.Result = new ObjectResult(response) { StatusCode = 403 };
            }
        }

        private async Task<bool> CheckPermissionForAdmin(
            IPermissionService permissionService,
            ApplicationDbContext dbContext,
            long userId,
            string permission)
        {
            var user = await dbContext.Admins.FindAsync(userId);
            if (user == null) return false;
            return await permissionService.CanAsync(user, permission);
        }

        private async Task<bool> CheckPermissionForCompany(
            IPermissionService permissionService,
            ApplicationDbContext dbContext,
            long userId,
            string permission)
        {
            var user = await dbContext.Companies.FindAsync(userId);
            if (user == null) return false;
            return await permissionService.CanAsync(user, permission);
        }

        private async Task<bool> CheckPermissionForDriver(
            IPermissionService permissionService,
            ApplicationDbContext dbContext,
            long userId,
            string permission)
        {
            var user = await dbContext.Drivers.FindAsync(userId);
            if (user == null) return false;
            return await permissionService.CanAsync(user, permission);
        }

        private async Task<bool> CheckPermissionForCollector(
            IPermissionService permissionService,
            ApplicationDbContext dbContext,
            long userId,
            string permission)
        {
            var user = await dbContext.Collectors.FindAsync(userId);
            if (user == null) return false;
            return await permissionService.CanAsync(user, permission);
        }
    }
}

