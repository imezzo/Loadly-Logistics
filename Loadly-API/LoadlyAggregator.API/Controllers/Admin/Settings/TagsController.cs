using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Settings.Tags;
using LoadlyAggregator.API.Resources.Admin.Settings;
using LoadlyAggregator.Domain.Admin.Actions.Tags;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin.Settings;

[ApiController]
[Route("api/admin/settings/tags")]
[Authorize]
public class TagsController : ControllerBase
{
    private readonly CreateTagAction _createTagAction;
    private readonly GetTagAction _getTagAction;
    private readonly ListTagsAction _listTagsAction;
    private readonly UpdateTagAction _updateTagAction;
    private readonly DeleteTagAction _deleteTagAction;

    public TagsController(
        CreateTagAction createTagAction,
        GetTagAction getTagAction,
        ListTagsAction listTagsAction,
        UpdateTagAction updateTagAction,
        DeleteTagAction deleteTagAction)
    {
        _createTagAction = createTagAction;
        _getTagAction = getTagAction;
        _listTagsAction = listTagsAction;
        _updateTagAction = updateTagAction;
        _deleteTagAction = deleteTagAction;
    }

    [HttpGet]
    [HasPermission(ListTagsRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListTagsRequest request) =>
        Ok((await _listTagsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<TagDto, TagResource>(dto => TagResource.FromDto(dto)));

    [HttpGet("{id}")]
    [HasPermission(GetTagRequest.Permission)]
    public async Task<ActionResult<TagResource>> Get(long id) =>
        Ok(TagResource.FromDto(await _getTagAction.ExecuteAsync(new GetTagRequest { Id = id }.FromRequest())));

    [HttpPost]
    [HasPermission(CreateTagRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateTagRequest request)
    {
        await _createTagAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "tags.created_successfully" });
    }

    [HttpPut("{id}")]
    [HasPermission(UpdateTagRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateTagRequest request)
    {
        request.Id = id;
        await _updateTagAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "tags.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteTagRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        await _deleteTagAction.ExecuteAsync(new DeleteTagRequest { Id = id }.FromRequest());
        return Ok(new { message = "tags.deleted_successfully" });
    }
}
