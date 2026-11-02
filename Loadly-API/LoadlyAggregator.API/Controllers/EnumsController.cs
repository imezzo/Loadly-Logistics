using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.Domain.Common.Interfaces;
namespace LoadlyAggregator.API.Controllers;

[ApiController]
[Route("api/enums")]
[Authorize]
public class EnumsController(IEnumService enumService) : ControllerBase
{
    private readonly IEnumService _enumService = enumService;

    [HttpGet]
    public ActionResult<object> GetAll()
    {
        var enums = _enumService.GetAllEnums();
        return Ok(enums);
    }
}
