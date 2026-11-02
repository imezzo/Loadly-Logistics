using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Settings.DocumentStatuses;
using LoadlyAggregator.API.Resources.Admin.Settings;
using LoadlyAggregator.Domain.Admin.Actions.DocumentStatuses;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin.Settings;

[ApiController]
[Route("api/admin/settings/document-statuses")]
[Authorize]
public class DocumentStatusesController : ControllerBase
{
    private readonly CreateDocumentStatusAction _createDocumentStatusAction;
    private readonly GetDocumentStatusAction _getDocumentStatusAction;
    private readonly ListDocumentStatusesAction _listDocumentStatusesAction;
    private readonly UpdateDocumentStatusAction _updateDocumentStatusAction;
    private readonly DeleteDocumentStatusAction _deleteDocumentStatusAction;

    public DocumentStatusesController(
        CreateDocumentStatusAction createDocumentStatusAction,
        GetDocumentStatusAction getDocumentStatusAction,
        ListDocumentStatusesAction listDocumentStatusesAction,
        UpdateDocumentStatusAction updateDocumentStatusAction,
        DeleteDocumentStatusAction deleteDocumentStatusAction)
    {
        _createDocumentStatusAction = createDocumentStatusAction;
        _getDocumentStatusAction = getDocumentStatusAction;
        _listDocumentStatusesAction = listDocumentStatusesAction;
        _updateDocumentStatusAction = updateDocumentStatusAction;
        _deleteDocumentStatusAction = deleteDocumentStatusAction;
    }

    [HttpGet]
    [HasPermission(ListDocumentStatusesRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListDocumentStatusesRequest request) =>
        Ok((await _listDocumentStatusesAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<DocumentStatusDto, DocumentStatusResource>(dto => DocumentStatusResource.FromDto(dto)));

    [HttpPost]
    [HasPermission(CreateDocumentStatusRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateDocumentStatusRequest request)
    {
        await _createDocumentStatusAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "document_statuses.created_successfully" });
    }

    [HttpGet("{id}")]
    [HasPermission(GetDocumentStatusRequest.Permission)]
    public async Task<ActionResult<DocumentStatusResource>> Get(long id) =>
        Ok(DocumentStatusResource.FromDto(await _getDocumentStatusAction.ExecuteAsync(new GetDocumentStatusRequest { Id = id }.FromRequest())));

    [HttpPut("{id}")]
    [HasPermission(UpdateDocumentStatusRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateDocumentStatusRequest request)
    {
        request.Id = id;
        await _updateDocumentStatusAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "document_statuses.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteDocumentStatusRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        await _deleteDocumentStatusAction.ExecuteAsync(new DeleteDocumentStatusRequest { Id = id }.FromRequest());
        return Ok(new { message = "document_statuses.deleted_successfully" });
    }
}
