using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Helpers;
using LoadlyAggregator.API.Requests.Company.Trips;
using LoadlyAggregator.API.Resources.Admin.Trips;
using LoadlyAggregator.API.Resources.Company.Trips;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.Actions.Trips;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Company.Trips;

[ApiController]
[Route("api/company/trips")]
[Authorize]
public class CompanyTripsController : ControllerBase
{
    private readonly CompanyGetTripAction _companyGetTripAction;
    private readonly CompanyListTripsAction _companyListTripsAction;
    private readonly CompanyCreateTripAction _companyCreateTripAction;
    private readonly CompanyUpdateTripAction _companyUpdateTripAction;
    private readonly CompanyDeleteTripAction _companyDeleteTripAction;
    private readonly CompanyCancelTripAction _companyCancelTripAction;
    private readonly CompanyDownloadTripDocumentAction _downloadTripDocumentAction;
    private readonly CompanyTrackTripsAction _companyTrackTripsAction;
    private readonly CompanyBulkCreateTripsAction _companyBulkCreateTripsAction;
    private readonly ILanguageService _languageService;
    private readonly IMapper _mapper;

    public CompanyTripsController(
        CompanyGetTripAction companyGetTripAction,
        CompanyListTripsAction companyListTripsAction,
        CompanyCreateTripAction companyCreateTripAction,
        CompanyUpdateTripAction companyUpdateTripAction,
        CompanyDeleteTripAction companyDeleteTripAction,
        CompanyCancelTripAction companyCancelTripAction,
        CompanyDownloadTripDocumentAction downloadTripDocumentAction,
        CompanyTrackTripsAction companyTrackTripsAction,
        CompanyBulkCreateTripsAction companyBulkCreateTripsAction,
        ILanguageService languageService,
        IMapper mapper)
    {
        _companyGetTripAction = companyGetTripAction;
        _companyListTripsAction = companyListTripsAction;
        _companyCreateTripAction = companyCreateTripAction;
        _companyUpdateTripAction = companyUpdateTripAction;
        _companyDeleteTripAction = companyDeleteTripAction;
        _companyCancelTripAction = companyCancelTripAction;
        _downloadTripDocumentAction = downloadTripDocumentAction;
        _companyTrackTripsAction = companyTrackTripsAction;
        _companyBulkCreateTripsAction = companyBulkCreateTripsAction;
        _languageService = languageService;
        _mapper = mapper;
    }

    [HttpGet("track")]
    // [HasPermission(CompanyTrackTripsRequest.Permission)]
    public async Task<ActionResult> Track([FromQuery] CompanyTrackTripsRequest request)
    {
        var companyId = ClaimsHelper.GetCompanyId(User);
        var lang = _languageService.GetCurrentLanguage();
        return Ok((await _companyTrackTripsAction.ExecuteAsync(request.FromRequest(companyId)))
            .ToResourceCollection<TripTrackItemDto, TripTrackItemResource>(dto => TripTrackItemResource.FromDto(dto, lang)));
    }

    [HttpPost]
    // [HasPermission(CompanyCreateTripRequest.Permission)]
    public async Task<IActionResult> CreateTrip([FromBody] CompanyCreateTripRequest request)
    {
        var companyId = ClaimsHelper.GetCompanyId(User);
        await _companyCreateTripAction.ExecuteAsync(request.FromRequest(companyId));
        return Ok(new { message = "trips.created_successfully" });
    }

    [HttpPost("bulk-create")]
    // [HasPermission(CompanyBulkCreateTripsRequest.Permission)]
    public async Task<ActionResult<BulkImportTripsResultResource>> BulkCreate([FromBody] CompanyBulkCreateTripsRequest request)
    {
        var companyId = ClaimsHelper.GetCompanyId(User);
        var result = await _companyBulkCreateTripsAction.ExecuteAsync(request.FromRequest(companyId));
        return Ok(BulkImportTripsResultResource.FromResult(result));
    }

    [HttpPut("{id:long}")]
    // [HasPermission(CompanyUpdateTripRequest.Permission)]
    public async Task<IActionResult> UpdateTrip(long id, [FromBody] CompanyUpdateTripRequest request)
    {
        var companyId = ClaimsHelper.GetCompanyId(User);
        await _companyUpdateTripAction.ExecuteAsync(request.FromRequest(id, companyId));
        return Ok(new { message = "trips.updated_successfully" });
    }

    [HttpPost("{id:long}/cancel")]
    // [HasPermission(CompanyCancelTripRequest.Permission)]
    public async Task<IActionResult> Cancel(long id, [FromBody] CompanyCancelTripRequest? request = null)
    {
        var companyId = ClaimsHelper.GetCompanyId(User);
        await _companyCancelTripAction.ExecuteAsync((request ?? new CompanyCancelTripRequest()).FromRequest(id, companyId));
        return Ok(new { message = "trips.cancelled_successfully" });
    }

    [HttpDelete("{id:long}")]
    // [HasPermission(CompanyDeleteTripRequest.Permission)]
    public async Task<IActionResult> Delete(long id)
    {
        var companyId = ClaimsHelper.GetCompanyId(User);
        await _companyDeleteTripAction.ExecuteAsync(new CompanyDeleteTripRequest().FromRequest(id, companyId));
        return Ok(new { message = "trips.deleted_successfully" });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CompanyTripResource>> Get(long id)
    {
        var companyId = ClaimsHelper.GetCompanyId(User);
        return Ok(_mapper.Map<CompanyTripResource>(await _companyGetTripAction.ExecuteAsync(new CompanyGetTripRequest { Id = id }.FromRequest(companyId))));
    }

    [HttpGet("{id}/documents/download")]
    public async Task<IActionResult> DownloadDocument(long id, [FromQuery] string path)
    {
        var companyId = ClaimsHelper.GetCompanyId(User);
        return Redirect(await _downloadTripDocumentAction.ExecuteAsync(new CompanyDownloadTripDocumentRequest { TripId = id, Path = path }.FromRequest(companyId)));
    }

    [HttpGet]
    public async Task<ActionResult> List([FromQuery] CompanyListTripsRequest request)
    {
        var companyId = ClaimsHelper.GetCompanyId(User);
        return Ok((await _companyListTripsAction.ExecuteAsync(request.FromRequest(companyId)))
            .ToResourceCollection<TripDto, CompanyTripResource>(dto => _mapper.Map<CompanyTripResource>(dto)));
    }
}
