using LoadlyAggregator.Domain.Admin.DTOs.Companies;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Companies;

public class DeleteCompanyAction
{
    private readonly ICompanyRepository _companyRepository;

    public DeleteCompanyAction(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task ExecuteAsync(DeleteCompanyDto dto)
    {
        var company = await _companyRepository.GetByIdOrThrowAsync(dto.Id);
        _companyRepository.Remove(company);
        await _companyRepository.SaveChangesAsync();
    }
}
