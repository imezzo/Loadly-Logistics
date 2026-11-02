using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Helpers;
using LoadlyAggregator.API.Requests.Company.Roles;
using LoadlyAggregator.API.Resources.Company.Roles;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.Actions.Roles;
using LoadlyAggregator.Domain.Company.DTOs.Roles;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Company.Roles;

[ApiController]
[Route("api/company/roles")]
[Authorize]
public class CompanyRolesController : ControllerBase
{
    private readonly CreateCompanyRoleAction _createCompanyRoleAction;
    private readonly GetCompanyRoleAction _getCompanyRoleAction;
    private readonly ListCompanyRolesAction _listCompanyRolesAction;
    private readonly UpdateCompanyRoleAction _updateCompanyRoleAction;
    private readonly DeleteCompanyRoleAction _deleteCompanyRoleAction;
    private readonly ICompanyRepository _companyRepository;

    public CompanyRolesController(
        CreateCompanyRoleAction createCompanyRoleAction,
        GetCompanyRoleAction getCompanyRoleAction,
        ListCompanyRolesAction listCompanyRolesAction,
        UpdateCompanyRoleAction updateCompanyRoleAction,
        DeleteCompanyRoleAction deleteCompanyRoleAction,
        ICompanyRepository companyRepository)
    {
        _createCompanyRoleAction = createCompanyRoleAction;
        _getCompanyRoleAction = getCompanyRoleAction;
        _listCompanyRolesAction = listCompanyRolesAction;
        _updateCompanyRoleAction = updateCompanyRoleAction;
        _deleteCompanyRoleAction = deleteCompanyRoleAction;
        _companyRepository = companyRepository;
    }

    [HttpPost]
    [HasPermission(CreateCompanyRoleRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateCompanyRoleRequest request)
    {
        await _createCompanyRoleAction.ExecuteAsync(
            request.FromRequest(ClaimsHelper.GetCompanyId(User)));
        return Ok(new { message = "roles.created_successfully" });
    }

    [HttpGet("{id}")]
    [HasPermission(GetCompanyRoleRequest.Permission)]
    public async Task<ActionResult<CompanyRoleResource>> Get(long id) =>
        Ok(CompanyRoleResource.FromDto(await _getCompanyRoleAction.ExecuteAsync(
            new GetCompanyRoleRequest { Id = id }.FromRequest(ClaimsHelper.GetCompanyId(User)))));

    [HttpGet]
    [HasPermission(ListCompanyRolesRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListCompanyRolesRequest request) =>
        Ok((await _listCompanyRolesAction.ExecuteAsync(
            request.FromRequest(ClaimsHelper.GetCompanyId(User))))
            .ToResourceCollection<CompanyRoleDto, CompanyRoleResource>(dto => CompanyRoleResource.FromDto(dto)));

    [HttpPut("{id}")]
    [HasPermission(UpdateCompanyRoleRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateCompanyRoleRequest request)
    {
        request.Id = id;
        await _updateCompanyRoleAction.ExecuteAsync(
            request.FromRequest(ClaimsHelper.GetCompanyId(User)));
        return Ok(new { message = "roles.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteCompanyRoleRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        await _deleteCompanyRoleAction.ExecuteAsync(
            new DeleteCompanyRoleRequest { Id = id }.FromRequest(ClaimsHelper.GetCompanyId(User)));
        return Ok(new { message = "roles.deleted_successfully" });
    }
}
