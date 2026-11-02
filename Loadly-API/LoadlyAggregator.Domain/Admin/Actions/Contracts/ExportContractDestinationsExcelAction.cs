using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Admin.Export;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Export;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Contracts;

public class ExportContractDestinationsExcelAction
{
    private readonly IContractRepository _contractRepository;
    private readonly IContractDestinationRepository _destinationRepository;
    private readonly IExcelParseService _excelParseService;
    private readonly ILanguageService _languageService;

    public ExportContractDestinationsExcelAction(
        IContractRepository contractRepository,
        IContractDestinationRepository destinationRepository,
        IExcelParseService excelParseService,
        ILanguageService languageService)
    {
        _contractRepository = contractRepository;
        _destinationRepository = destinationRepository;
        _excelParseService = excelParseService;
        _languageService = languageService;
    }

    public async Task<ContractDestinationsExcelFileDto> ExecuteAsync(ExportContractDestinationsExcelDto dto)
    {
        Contract contract = await _contractRepository.FirstOrDefaultAsync(c => c.Id == dto.Id)
            ?? throw new NotFoundException("contracts.not_found_with_id", dto.Id);

        List<ContractDestination> destinations = (await _destinationRepository.GetByContractIdAsync(dto.Id))
            .Where(d => d.Status != ContractDestinationStatus.ARCHIVED)
            .ToList();
        IReadOnlyList<ExportColumn<ContractDestination>> columns =
            ExcelExportBuilder.ResolveColumns(ContractDestinationExportColumns.All, dto.Columns);

        return new ContractDestinationsExcelFileDto
        {
            Content = ExcelExportBuilder.Build(
                "Destinations",
                destinations,
                columns,
                _languageService,
                _excelParseService),
            FileName = $"contract-{contract.ReferenceNumber}-destinations.xlsx"
        };
    }
}
