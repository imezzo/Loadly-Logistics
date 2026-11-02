using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Helpers;
using LoadlyAggregator.API.Requests.Driver.Documents;
using LoadlyAggregator.Domain.Driver.Actions.Documents;

namespace LoadlyAggregator.API.Controllers.Driver.Documents;

[ApiController]
[Route("api/driver/documents")]
[Authorize]
public class DriverDocumentsController : ControllerBase
{
    private readonly DriverUploadDocumentAction _uploadDocumentAction;
    private readonly DriverUpdateDocumentAction _updateDocumentAction;

    public DriverDocumentsController(
        DriverUploadDocumentAction uploadDocumentAction,
        DriverUpdateDocumentAction updateDocumentAction)
    {
        _uploadDocumentAction = uploadDocumentAction;
        _updateDocumentAction = updateDocumentAction;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> Upload([FromForm] DriverUploadDocumentRequest request)
    {
        var driverId = ClaimsHelper.GetUserId(User);
        await _uploadDocumentAction.ExecuteAsync(request.FromRequest(driverId));
        return Ok(new { message = "driver_documents.created_successfully" });
    }

    [HttpPut("{id}")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> Update(long id, [FromForm] DriverUpdateDocumentRequest request)
    {
        request.Id = id;
        var driverId = ClaimsHelper.GetUserId(User);
        await _updateDocumentAction.ExecuteAsync(request.FromRequest(driverId));
        return Ok(new { message = "driver_documents.updated_successfully" });
    }
}
