using System;
using System.Linq;
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





[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class HasAnyPermissionAttribute : TypeFilterAttribute
{
    public HasAnyPermissionAttribute(params string[] permissions) : base(typeof(AnyPermissionFilter))
    {
        Arguments = new object[] { permissions };
    }

    private class AnyPermissionFilter : IAsyncAuthorizationFilter
    {
        private readonly string[] _permissions;

        public AnyPermissionFilter(string[] permissions)
        {
            _permissions = permissions;
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

            bool hasAnyPermission = false;

            switch (userTypeClaim.ToLower())
            {
                case "admin":
                    var admin = await dbContext.Admins.FindAsync(userId);
                    if (admin != null)
                        hasAnyPermission = await permissionService.HasAnyPermissionAsync(admin, _permissions);
                    break;
                case "company":
                    var company = await dbContext.Companies.FindAsync(userId);
                    if (company != null)
                        hasAnyPermission = await permissionService.HasAnyPermissionAsync(company, _permissions);
                    break;
                case "driver":
                    var driver = await dbContext.Drivers.FindAsync(userId);
                    if (driver != null)
                        hasAnyPermission = await permissionService.HasAnyPermissionAsync(driver, _permissions);
                    break;
                case "collector":
                    var collector = await dbContext.Collectors.FindAsync(userId);
                    if (collector != null)
                        hasAnyPermission = await permissionService.HasAnyPermissionAsync(collector, _permissions);
                    break;
            }

            if (!hasAnyPermission)
            {
                var response = ApiResponse<object>.Error("Forbidden: Insufficient permissions", null);
                context.Result = new ObjectResult(response) { StatusCode = 403 };
            }
        }
    }
}

