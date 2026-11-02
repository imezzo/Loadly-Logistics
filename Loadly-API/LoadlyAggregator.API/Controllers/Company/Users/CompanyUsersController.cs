using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Helpers;
using LoadlyAggregator.API.Requests.Company.Users;
using LoadlyAggregator.API.Resources.Admin.Companies;
using LoadlyAggregator.Domain.Admin.DTOs.Companies;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.Actions.Users;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Company.Users;

[ApiController]
[Route("api/company/users")]
[Authorize]
public class CompanyUsersController : ControllerBase
{
    private readonly CreateCompanyUserAction _createCompanyUserAction;
    private readonly GetCompanyUserAction _getCompanyUserAction;
    private readonly ListCompanyUsersAction _listCompanyUsersAction;
    private readonly UpdateCompanyUserAction _updateCompanyUserAction;
    private readonly DeleteCompanyUserAction _deleteCompanyUserAction;
    private readonly ICompanyRepository _companyRepository;

    public CompanyUsersController(
        CreateCompanyUserAction createCompanyUserAction,
        GetCompanyUserAction getCompanyUserAction,
        ListCompanyUsersAction listCompanyUsersAction,
        UpdateCompanyUserAction updateCompanyUserAction,
        DeleteCompanyUserAction deleteCompanyUserAction,
        ICompanyRepository companyRepository)
    {
        _createCompanyUserAction = createCompanyUserAction;
        _getCompanyUserAction = getCompanyUserAction;
        _listCompanyUsersAction = listCompanyUsersAction;
        _updateCompanyUserAction = updateCompanyUserAction;
        _deleteCompanyUserAction = deleteCompanyUserAction;
        _companyRepository = companyRepository;
    }

    [HttpPost]
    [HasPermission(CreateCompanyUserRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateCompanyUserRequest request)
    {
        await _createCompanyUserAction.ExecuteAsync(
            request.FromRequest(ClaimsHelper.GetCompanyId(User)));
        return Ok(new { message = "company_users.created_successfully" });
    }

    [HttpGet("{id}")]
    [HasPermission(GetCompanyUserRequest.Permission)]
    public async Task<ActionResult<CompanyUserResource>> Get(long id)
    {
        return Ok(CompanyUserResource.FromDto(
            await _getCompanyUserAction.ExecuteAsync(
                new GetCompanyUserRequest { Id = id }.FromRequest(
                    ClaimsHelper.GetCompanyId(User)))));
    }

    [HttpGet]
    [HasPermission(ListCompanyUsersRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListCompanyUsersRequest request)
    {
        return Ok((await _listCompanyUsersAction.ExecuteAsync(
            request.FromRequest(ClaimsHelper.GetCompanyId(User))))
            .ToResourceCollection<CompanyUserDto, CompanyUserResource>(dto => CompanyUserResource.FromDto(dto)));
    }

    [HttpPut("{id}")]
    [HasPermission(UpdateCompanyUserRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateCompanyUserRequest request)
    {
        request.Id = id;
        await _updateCompanyUserAction.ExecuteAsync(
            request.FromRequest(ClaimsHelper.GetCompanyId(User)));
        return Ok(new { message = "company_users.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteCompanyUserRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        await _deleteCompanyUserAction.ExecuteAsync(
            new DeleteCompanyUserRequest { Id = id }.FromRequest(
                ClaimsHelper.GetCompanyId(User)));
        return Ok(new { message = "company_users.deleted_successfully" });
    }
}
