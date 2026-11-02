using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Collector.Documents;
using LoadlyAggregator.API.Resources.Admin.Documents;
using LoadlyAggregator.Domain.Collector.Actions.Documents;
using LoadlyAggregator.Domain.Common.DTOs.Documents;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Collector.Documents;

[ApiController]
[Route("api/collector/documents")]
[Authorize]
public class CollectorDocumentsController : ControllerBase
{
    private readonly CollectorGetDocumentAction _getDocumentAction;
    private readonly CollectorListDocumentsAction _listDocumentsAction;

    public CollectorDocumentsController(
        CollectorGetDocumentAction getDocumentAction,
        CollectorListDocumentsAction listDocumentsAction)
    {
        _getDocumentAction = getDocumentAction;
        _listDocumentsAction = listDocumentsAction;
    }

    [HttpGet("{id}")]
    [HasPermission(CollectorGetDocumentRequest.Permission)]
    public async Task<ActionResult<DocumentResource>> Get(long id) =>
        Ok(DocumentResource.FromDto(await _getDocumentAction.ExecuteAsync(new CollectorGetDocumentRequest { Id = id }.FromRequest())));

    [HttpGet]
    [HasPermission(CollectorListDocumentsRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] CollectorListDocumentsRequest request) =>
        Ok((await _listDocumentsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<DocumentDto, DocumentResource>(dto => DocumentResource.FromDto(dto)));
}
