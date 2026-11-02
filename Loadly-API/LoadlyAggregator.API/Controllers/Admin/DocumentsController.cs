using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Documents;
using LoadlyAggregator.API.Resources.Admin.Documents;
using LoadlyAggregator.Domain.Admin.Actions.Documents;
using LoadlyAggregator.Domain.Common.DTOs.Documents;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin;

[ApiController]
[Route("api/admin/documents")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly CreateDocumentAction _createDocumentAction;
    private readonly GetDocumentAction _getDocumentAction;
    private readonly ListDocumentsAction _listDocumentsAction;
    private readonly UpdateDocumentAction _updateDocumentAction;
    private readonly DeleteDocumentAction _deleteDocumentAction;

    public DocumentsController(
        CreateDocumentAction createDocumentAction,
        GetDocumentAction getDocumentAction,
        ListDocumentsAction listDocumentsAction,
        UpdateDocumentAction updateDocumentAction,
        DeleteDocumentAction deleteDocumentAction)
    {
        _createDocumentAction = createDocumentAction;
        _getDocumentAction = getDocumentAction;
        _listDocumentsAction = listDocumentsAction;
        _updateDocumentAction = updateDocumentAction;
        _deleteDocumentAction = deleteDocumentAction;
    }

    [HttpPost]
    [HasPermission(CreateDocumentRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateDocumentRequest request)
    {
        await _createDocumentAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "documents.created_successfully" });
    }

    [HttpGet("{id}")]
    //[HasPermission(GetDocumentRequest.Permission)]
    public async Task<ActionResult<DocumentResource>> Get(long id) =>
        Ok(DocumentResource.FromDto(await _getDocumentAction.ExecuteAsync(new GetDocumentRequest { Id = id }.FromRequest())));

    [HttpGet]
    //[HasPermission(ListDocumentsRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListDocumentsRequest request) =>
        Ok((await _listDocumentsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<DocumentDto, DocumentResource>(dto => DocumentResource.FromDto(dto)));

    [HttpPut("{id}")]
    [HasPermission(UpdateDocumentRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateDocumentRequest request)
    {
        request.Id = id;
        await _updateDocumentAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "documents.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteDocumentRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        var deleteRequest = new DeleteDocumentRequest { Id = id };
        await _deleteDocumentAction.ExecuteAsync(deleteRequest.FromRequest());
        return Ok(new { message = "documents.deleted_successfully" });
    }
}
