using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Helpers;
using LoadlyAggregator.API.Requests.Company.Dashboard;
using LoadlyAggregator.API.Resources.Company.Dashboard;
using LoadlyAggregator.API.Resources.Company.Invoices;
using LoadlyAggregator.Domain.Company.Actions.Dashboard;
using LoadlyAggregator.Domain.Company.DTOs.Dashboard;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Company;

[ApiController]
[Route("api/company/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly CompanyGetDashboardMonthlyTripsAction _companyGetDashboardMonthlyTripsAction;
    private readonly CompanyGetTripsStatisticsAction _companyGetTripsStatisticsAction;
    private readonly CompanyGetTripCostPeriodTotalsAction _companyGetTripCostPeriodTotalsAction;
    private readonly CompanyGetInvoiceStatisticsAction _companyGetInvoiceStatisticsAction;
    private readonly CompanyGetHomeDashboardAction _companyGetHomeDashboardAction;

    public DashboardController(
        CompanyGetDashboardMonthlyTripsAction companyGetDashboardMonthlyTripsAction,
        CompanyGetTripsStatisticsAction companyGetTripsStatisticsAction,
        CompanyGetTripCostPeriodTotalsAction companyGetTripCostPeriodTotalsAction,
        CompanyGetInvoiceStatisticsAction companyGetInvoiceStatisticsAction,
        CompanyGetHomeDashboardAction companyGetHomeDashboardAction)
    {
        _companyGetDashboardMonthlyTripsAction = companyGetDashboardMonthlyTripsAction;
        _companyGetTripsStatisticsAction = companyGetTripsStatisticsAction;
        _companyGetTripCostPeriodTotalsAction = companyGetTripCostPeriodTotalsAction;
        _companyGetInvoiceStatisticsAction = companyGetInvoiceStatisticsAction;
        _companyGetHomeDashboardAction = companyGetHomeDashboardAction;
    }

    [HttpGet("home")]
    //[HasPermission(GetCompanyDashboardHomeRequest.Permission)]
    public async Task<ActionResult<CompanyHomeDashboardResource>> GetHome([FromQuery] GetCompanyDashboardHomeRequest request)
    {
        var companyId = ClaimsHelper.GetCompanyId(User);
        return Ok(CompanyHomeDashboardResource.FromDto(
            await _companyGetHomeDashboardAction.ExecuteAsync(request.FromRequest(companyId))));
    }

    [HttpGet("trips")]
    //[HasPermission(GetCompanyDashboardTripsStatisticsRequest.Permission)]
    public async Task<ActionResult<CompanyTripsStatisticsResource>> GetTrips([FromQuery] GetCompanyDashboardTripsStatisticsRequest request)
    {
        var companyId = ClaimsHelper.GetCompanyId(User);
        return Ok(CompanyTripsStatisticsResource.FromDto(await _companyGetTripsStatisticsAction.ExecuteAsync(request.FromRequest(companyId))));
    }

    [HttpGet("trip-cost-period-totals")]
    //[HasPermission(GetCompanyDashboardTripCostTotalsRequest.Permission)]
    public async Task<ActionResult<CompanyTripCostPeriodTotalsResource>> GetTripCostPeriodTotals()
    {
        var companyId = ClaimsHelper.GetCompanyId(User);
        GetCompanyDashboardTripCostTotalsRequest request = new();
        return Ok(CompanyTripCostPeriodTotalsResource.FromDto(
            await _companyGetTripCostPeriodTotalsAction.ExecuteAsync(request.FromRequest(companyId))));
    }

    [HttpGet("invoice-statistics")]
    //[HasPermission(GetCompanyDashboardInvoiceStatisticsRequest.Permission)]
    public async Task<ActionResult<InvoiceStatisticsResource>> GetInvoiceStatistics()
    {
        var companyId = ClaimsHelper.GetCompanyId(User);
        GetCompanyDashboardInvoiceStatisticsRequest request = new();
        return Ok(InvoiceStatisticsResource.FromDto(
            await _companyGetInvoiceStatisticsAction.ExecuteAsync(request.FromRequest(companyId))));
    }

    [HttpGet("monthly-trips-kilometers")]
    //[HasPermission(GetCompanyDashboardMonthlyTripsRequest.Permission)]
    public async Task<ActionResult> GetMonthlyTripsKilometers([FromQuery] GetCompanyDashboardMonthlyTripsRequest request)
    {
        var companyId = ClaimsHelper.GetCompanyId(User);
        return Ok((await _companyGetDashboardMonthlyTripsAction.ExecuteAsync(request.FromRequest(companyId)))
            .ToResourceCollection<CompanyMonthlyTripKilometersDto, CompanyMonthlyTripKilometersResource>(
                dto => CompanyMonthlyTripKilometersResource.FromDto(dto)));
    }
}
