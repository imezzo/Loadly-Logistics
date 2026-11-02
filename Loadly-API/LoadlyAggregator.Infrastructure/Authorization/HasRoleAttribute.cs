using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Infrastructure.Authorization;





[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class HasRoleAttribute : TypeFilterAttribute
{
    public HasRoleAttribute(string role) : base(typeof(RoleFilter))
    {
        Arguments = new object[] { role };
    }

    private class RoleFilter : IAsyncAuthorizationFilter
    {
        private readonly string _role;

        public RoleFilter(string role)
        {
            _role = role;
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

            bool hasRole = userTypeClaim.ToLower() switch
            {
                "admin" => await CheckRoleForAdmin(permissionService, dbContext, userId, _role),
                "company" => await CheckRoleForCompany(permissionService, dbContext, userId, _role),
                "driver" => await CheckRoleForDriver(permissionService, dbContext, userId, _role),
                "collector" => await CheckRoleForCollector(permissionService, dbContext, userId, _role),
                _ => false
            };

            if (!hasRole)
            {
                var response = ApiResponse<object>.Error("Forbidden: Insufficient role", null);
                context.Result = new ObjectResult(response) { StatusCode = 403 };
            }
        }

        private async Task<bool> CheckRoleForAdmin(
            IPermissionService permissionService,
            ApplicationDbContext dbContext,
            long userId,
            string role)
        {
            var user = await dbContext.Admins.FindAsync(userId);
            if (user == null) return false;
            return await permissionService.HasRoleAsync(user, role);
        }

        private async Task<bool> CheckRoleForCompany(
            IPermissionService permissionService,
            ApplicationDbContext dbContext,
            long userId,
            string role)
        {
            var user = await dbContext.Companies.FindAsync(userId);
            if (user == null) return false;
            return await permissionService.HasRoleAsync(user, role);
        }

        private async Task<bool> CheckRoleForDriver(
            IPermissionService permissionService,
            ApplicationDbContext dbContext,
            long userId,
            string role)
        {
            var user = await dbContext.Drivers.FindAsync(userId);
            if (user == null) return false;
            return await permissionService.HasRoleAsync(user, role);
        }

        private async Task<bool> CheckRoleForCollector(
            IPermissionService permissionService,
            ApplicationDbContext dbContext,
            long userId,
            string role)
        {
            var user = await dbContext.Collectors.FindAsync(userId);
            if (user == null) return false;
            return await permissionService.HasRoleAsync(user, role);
        }
    }
}

