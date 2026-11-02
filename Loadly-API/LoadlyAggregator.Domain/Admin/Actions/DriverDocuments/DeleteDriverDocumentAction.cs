using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.DriverDocuments;

public class DeleteDriverDocumentAction
{
    private readonly IDriverDocumentRepository _driverDocumentRepository;

    public DeleteDriverDocumentAction(IDriverDocumentRepository driverDocumentRepository)
    {
        _driverDocumentRepository = driverDocumentRepository;
    }

    public async Task ExecuteAsync(DeleteDriverDocumentDto dto)
    {
        var driverDocument = await _driverDocumentRepository.GetByIdOrThrowAsync(dto.Id);
        _driverDocumentRepository.Remove(driverDocument);
        await _driverDocumentRepository.SaveChangesAsync();
    }
}
