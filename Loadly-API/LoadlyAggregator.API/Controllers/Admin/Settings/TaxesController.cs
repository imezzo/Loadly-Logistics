using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Settings.Taxes;
using LoadlyAggregator.API.Resources.Admin.Settings;
using LoadlyAggregator.Domain.Admin.Actions.Taxes;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin.Settings;

[ApiController]
[Route("api/admin/settings/taxes")]
[Authorize]
public class TaxesController : ControllerBase
{
    private readonly CreateTaxAction _createTaxAction;
    private readonly GetTaxAction _getTaxAction;
    private readonly ListTaxesAction _listTaxesAction;
    private readonly UpdateTaxAction _updateTaxAction;
    private readonly DeleteTaxAction _deleteTaxAction;

    public TaxesController(
        CreateTaxAction createTaxAction,
        GetTaxAction getTaxAction,
        ListTaxesAction listTaxesAction,
        UpdateTaxAction updateTaxAction,
        DeleteTaxAction deleteTaxAction)
    {
        _createTaxAction = createTaxAction;
        _getTaxAction = getTaxAction;
        _listTaxesAction = listTaxesAction;
        _updateTaxAction = updateTaxAction;
        _deleteTaxAction = deleteTaxAction;
    }

    [HttpGet]
    [HasPermission(ListTaxesRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListTaxesRequest request) =>
        Ok((await _listTaxesAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<TaxDto, TaxResource>(dto => TaxResource.FromDto(dto)));

    [HttpPost]
    [HasPermission(CreateTaxRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateTaxRequest request)
    {
        await _createTaxAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "taxes.created_successfully" });
    }

    [HttpGet("{id}")]
    [HasPermission(GetTaxRequest.Permission)]
    public async Task<ActionResult<TaxResource>> Get(long id) =>
        Ok(TaxResource.FromDto(await _getTaxAction.ExecuteAsync(new GetTaxRequest { Id = id }.FromRequest())));

    [HttpPut("{id}")]
    [HasPermission(UpdateTaxRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateTaxRequest request)
    {
        request.Id = id;
        await _updateTaxAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "taxes.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteTaxRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        await _deleteTaxAction.ExecuteAsync(new DeleteTaxRequest { Id = id }.FromRequest());
        return Ok(new { message = "taxes.deleted_successfully" });
    }
}
