using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.DriverDocuments;
using LoadlyAggregator.API.Resources.Admin.DriverDocuments;
using LoadlyAggregator.Domain.Admin.Actions.DriverDocuments;
using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
using LoadlyAggregator.Infrastructure.Authorization;
using LoadlyAggregator.Infrastructure.Extensions;

namespace LoadlyAggregator.API.Controllers.Admin;

[ApiController]
[Route("api/admin/driver-documents")]
[Authorize]
public class DriverDocumentsController : ControllerBase
{
    private readonly CreateDriverDocumentAction _createDriverDocumentAction;
    private readonly GetDriverDocumentAction _getDriverDocumentAction;
    private readonly ListDriverDocumentsAction _listDriverDocumentsAction;
    private readonly UpdateDriverDocumentAction _updateDriverDocumentAction;
    private readonly DeleteDriverDocumentAction _deleteDriverDocumentAction;
    private readonly DownloadDriverDocumentAction _downloadDriverDocumentAction;

    public DriverDocumentsController(
        CreateDriverDocumentAction createDriverDocumentAction,
        GetDriverDocumentAction getDriverDocumentAction,
        ListDriverDocumentsAction listDriverDocumentsAction,
        UpdateDriverDocumentAction updateDriverDocumentAction,
        DeleteDriverDocumentAction deleteDriverDocumentAction,
        DownloadDriverDocumentAction downloadDriverDocumentAction)
    {
        _createDriverDocumentAction = createDriverDocumentAction;
        _getDriverDocumentAction = getDriverDocumentAction;
        _listDriverDocumentsAction = listDriverDocumentsAction;
        _updateDriverDocumentAction = updateDriverDocumentAction;
        _deleteDriverDocumentAction = deleteDriverDocumentAction;
        _downloadDriverDocumentAction = downloadDriverDocumentAction;
    }



    [HttpPost]
    [HasPermission(CreateDriverDocumentRequest.Permission)]
    [Consumes("multipart/form-data")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult> Create(
        [FromForm] long documentId,
        [FromForm] long driverId,
        [FromForm] IFormFile? file,
        [FromForm] DateTime? expiryDate,
        [FromForm] Domain.Common.Enums.DocumentVerificationStatus status = Domain.Common.Enums.DocumentVerificationStatus.PENDING,
        [FromForm] long? addedBy = null)
    {
        var request = new CreateDriverDocumentRequest
        {
            DocumentId = documentId,
            DriverId = driverId,
            File = file?.ToDomainFile(),
            ExpiryDate = expiryDate,
            Status = status,
            AddedBy = addedBy
        };
        await _createDriverDocumentAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "driver_documents.created_successfully" });
    }



    [HttpGet("{id}")]
    [HasPermission(GetDriverDocumentRequest.Permission)]
    public async Task<ActionResult<DriverDocumentResource>> Get(long id) =>
        Ok(DriverDocumentResource.FromDto(await _getDriverDocumentAction.ExecuteAsync(new GetDriverDocumentRequest { Id = id }.FromRequest())));

    [HttpGet("{id}/download")]
    [HasPermission(DownloadDriverDocumentRequest.Permission)]
    public async Task<IActionResult> Download(long id) =>
        Redirect(await _downloadDriverDocumentAction.ExecuteAsync(new DownloadDriverDocumentRequest { Id = id }.FromRequest()));

    [HttpGet]
    [HasPermission(ListDriverDocumentsRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListDriverDocumentsRequest request) =>
        Ok((await _listDriverDocumentsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<DriverDocumentDto, DriverDocumentResource>(dto => DriverDocumentResource.FromDto(dto)));



    [HttpPut("{id}")]
    [HasPermission(UpdateDriverDocumentRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateDriverDocumentRequest request)
    {
        request.Id = id;
        await _updateDriverDocumentAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "driver_documents.updated_successfully" });
    }



    [HttpDelete("{id}")]
    [HasPermission(DeleteDriverDocumentRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        var deleteRequest = new DeleteDriverDocumentRequest { Id = id };
        await _deleteDriverDocumentAction.ExecuteAsync(deleteRequest.FromRequest());
        return Ok(new { message = "driver_documents.deleted_successfully" });
    }
}
