using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Companies;
using LoadlyAggregator.API.Resources.Admin.Companies;
using LoadlyAggregator.Domain.Admin.Actions.Companies;
using LoadlyAggregator.Domain.Admin.DTOs.Companies;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin;

[ApiController]
[Route("api/admin/companies")]
[Authorize]
public class CompaniesController : ControllerBase
{
    private readonly StoreCompanyAction _storeCompanyAction;
    private readonly GetCompanyAction _getCompanyAction;
    private readonly ListCompaniesAction _listCompaniesAction;
    private readonly UpdateCompanyAction _updateCompanyAction;
    private readonly DeleteCompanyAction _deleteCompanyAction;
    private readonly BulkImportCompaniesAction _bulkImportCompaniesAction;

    private readonly CreateCompanyUserAction _createCompanyUserAction;
    private readonly ListCompanyUsersAction _listCompanyUsersAction;
    private readonly GetCompanyUserAction _getCompanyUserAction;
    private readonly UpdateCompanyUserAction _updateCompanyUserAction;
    private readonly DeleteCompanyUserAction _deleteCompanyUserAction;

    public CompaniesController(
        StoreCompanyAction storeCompanyAction,
        GetCompanyAction getCompanyAction,
        ListCompaniesAction listCompaniesAction,
        UpdateCompanyAction updateCompanyAction,
        DeleteCompanyAction deleteCompanyAction,
        BulkImportCompaniesAction bulkImportCompaniesAction,
        CreateCompanyUserAction createCompanyUserAction,
        ListCompanyUsersAction listCompanyUsersAction,
        GetCompanyUserAction getCompanyUserAction,
        UpdateCompanyUserAction updateCompanyUserAction,
        DeleteCompanyUserAction deleteCompanyUserAction)
    {
        _storeCompanyAction = storeCompanyAction;
        _getCompanyAction = getCompanyAction;
        _listCompaniesAction = listCompaniesAction;
        _updateCompanyAction = updateCompanyAction;
        _deleteCompanyAction = deleteCompanyAction;
        _bulkImportCompaniesAction = bulkImportCompaniesAction;
        _createCompanyUserAction = createCompanyUserAction;
        _listCompanyUsersAction = listCompanyUsersAction;
        _getCompanyUserAction = getCompanyUserAction;
        _updateCompanyUserAction = updateCompanyUserAction;
        _deleteCompanyUserAction = deleteCompanyUserAction;
    }

    [HttpPost]
    [HasPermission(StoreCompanyRequest.Permission)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> Store([FromForm] StoreCompanyRequest request)
    {
        await _storeCompanyAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "companies.created_successfully" });
    }

    [HttpGet("{id}")]
    [HasPermission(GetCompanyRequest.Permission)]
    public async Task<ActionResult<CompanyResource>> Get(long id) =>
        Ok(CompanyResource.FromDto(await _getCompanyAction.ExecuteAsync(new GetCompanyRequest { Id = id }.FromRequest())));

    [HttpGet]
    [HasPermission(ListCompaniesRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListCompaniesRequest request) =>
        Ok((await _listCompaniesAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<CompanyDto, CompanyResource>(dto => CompanyResource.FromDto(dto)));

    [HttpPut("{id}")]
    [HasPermission(UpdateCompanyRequest.Permission)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> Update(long id, [FromForm] UpdateCompanyRequest request)
    {
        request.Id = id;
        await _updateCompanyAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "companies.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteCompanyRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        var deleteRequest = new DeleteCompanyRequest { Id = id };
        await _deleteCompanyAction.ExecuteAsync(deleteRequest.FromRequest());
        return Ok(new { message = "companies.deleted_successfully" });
    }

    [HttpPost("{companyId}/users")]
    [HasPermission(CreateCompanyUserRequest.Permission)]
    public async Task<ActionResult> CreateUser(long companyId, [FromForm] CreateCompanyUserRequest request)
    {
        request.CompanyId = companyId;
        await _createCompanyUserAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "company_users.created_successfully" });
    }

    [HttpGet("{companyId}/users")]
    [HasPermission(ListCompanyUsersRequest.Permission)]
    public async Task<ActionResult> ListUsers(long companyId, [FromQuery] ListCompanyUsersRequest request) =>
     Ok((await _listCompanyUsersAction.ExecuteAsync(request.FromRequest()))
         .ToResourceCollection<CompanyUserDto, CompanyUserResource>(dto => CompanyUserResource.FromDto(dto)));

    [HttpGet("{companyId}/users/{id}")]
    [HasPermission(GetCompanyUserRequest.Permission)]
    public async Task<ActionResult<CompanyUserResource>> GetUser(long companyId, long id) =>
     Ok(CompanyUserResource.FromDto(await _getCompanyUserAction.ExecuteAsync(new GetCompanyUserRequest { CompanyId = companyId, Id = id }.FromRequest())));

    [HttpPut("{companyId}/users/{id}")]
    [HasPermission(UpdateCompanyUserRequest.Permission)]
    public async Task<ActionResult> UpdateUser(long companyId, long id, [FromForm] UpdateCompanyUserRequest request)
    {
        request.CompanyId = companyId;
        request.Id = id;
        await _updateCompanyUserAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "company_users.updated_successfully" });
    }

    [HttpDelete("{companyId}/users/{id}")]
    [HasPermission(DeleteCompanyUserRequest.Permission)]
    public async Task<ActionResult> DeleteUser(long companyId, long id)
    {

        var deleteRequest = new DeleteCompanyUserRequest { CompanyId = companyId, Id = id };
        await _deleteCompanyUserAction.ExecuteAsync(deleteRequest.FromRequest());
        return Ok(new { message = "company_users.deleted_successfully" });
    }

    [HttpPost("import")]
    [HasPermission(ImportCompaniesRequest.Permission)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<BulkImportCompaniesResultResource>> Import([FromForm] ImportCompaniesRequest request)
    {
        var result = await _bulkImportCompaniesAction.ExecuteAsync(new ImportCompaniesFromExcelDto { OpenStream = () => request.ExcelFile.OpenReadStream() });
        return Ok(BulkImportCompaniesResultResource.FromResult(result));
    }
}
