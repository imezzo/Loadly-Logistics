using System.Security.Claims;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Helpers;

public static class CompanyContextHelper
{
    public static async Task<long> GetCurrentCompanyIdAsync(ClaimsPrincipal user, ICompanyRepository companyRepository)
    {
        var currentUser = await companyRepository.GetByIdOrThrowAsync(ClaimsHelper.GetUserId(user));
        return currentUser.CompanyId ?? currentUser.Id;
    }
}
