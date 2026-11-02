using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;

namespace LoadlyAggregator.API.Controllers;

[ApiController]
[Route("api/lookups")]
[Authorize]
public class ExtraFeeTypesController : ControllerBase
{

    [HttpGet("extra-fee-types")]
    public ActionResult GetExtraFeeTypes()
    {
        var data = ExtraFeeMetadataBuilder
            .Build()
            .Where(type => type.TypeId != (int)ExtraFeeTypes.Fuel)
            .ToList();
        return Ok(data);
    }
}
