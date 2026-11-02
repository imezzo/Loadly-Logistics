using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using LoadlyAggregator.Domain.Admin.Actions.Settings;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin.Settings;

[ApiController]
[Route("api/admin/settings/general")]
[Authorize]
public class GeneralSettingsController : ControllerBase
{
    private readonly GetGeneralSettingAction _getGeneralSettingAction;
    private readonly SetGeneralSettingAction _setGeneralSettingAction;
    private readonly ListGeneralSettingsAction _listGeneralSettingsAction;

    [ActivatorUtilitiesConstructor]
    public GeneralSettingsController(
        GetGeneralSettingAction getGeneralSettingAction,
        SetGeneralSettingAction setGeneralSettingAction,
        ListGeneralSettingsAction listGeneralSettingsAction)
    {
        _getGeneralSettingAction = getGeneralSettingAction;
        _setGeneralSettingAction = setGeneralSettingAction;
        _listGeneralSettingsAction = listGeneralSettingsAction;
    }


    [HttpGet]
    [HasPermission("view_settings")]
    public async Task<ActionResult<object>> GetAll()
    {
        var settings = await _listGeneralSettingsAction.ExecuteAsync();
        return Ok(settings);
    }

    [HttpGet("{key}")]
    [HasPermission("view_settings")]
    public async Task<ActionResult<object>> GetByKey(string key)
    {
        var value = await _getGeneralSettingAction.ExecuteAsync(key);
        if (value == null)
        {
            return NotFound(new { message = "settings.not_found" });
        }
        return Ok(new { key, value });
    }

    [HttpPut("{key}")]
    [HasPermission("edit_settings")]
    public async Task<IActionResult> Set(string key, [FromBody] SetGeneralSettingRequest request)
    {
        await _setGeneralSettingAction.ExecuteAsync(key, request.Value);
        return Ok(new { message = "settings.updated_successfully" });
    }
}

public class SetGeneralSettingRequest
{
    public string Value { get; set; } = string.Empty;
}
