using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Contractor.Documents;
using LoadlyAggregator.API.Resources.Admin.Documents;
using LoadlyAggregator.Domain.Common.DTOs.Documents;
using LoadlyAggregator.Domain.Contractor.Actions.Documents;

namespace LoadlyAggregator.API.Controllers.Contractor.Documents;

[ApiController]
[Route("api/contractor/documents")]
public class ContractorDocumentsController : ControllerBase
{
    private readonly ContractorListDocumentsAction _listDocumentsAction;

    public ContractorDocumentsController(ContractorListDocumentsAction listDocumentsAction)
    {
        _listDocumentsAction = listDocumentsAction;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult> List([FromQuery] ContractorListDocumentsRequest request) =>
        Ok((await _listDocumentsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<DocumentDto, DocumentResource>(dto => DocumentResource.FromDto(dto)));
}
