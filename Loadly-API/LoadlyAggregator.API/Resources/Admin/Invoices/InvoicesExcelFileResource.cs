using LoadlyAggregator.Domain.Admin.DTOs.Invoices;

namespace LoadlyAggregator.API.Resources.Admin.Invoices;

public sealed class InvoicesExcelFileResource
{
    public required byte[] Content { get; init; }
    public required string FileName { get; init; }

    public static InvoicesExcelFileResource FromDto(InvoicesExcelFileDto dto) =>
        new() { Content = dto.Content, FileName = dto.FileName };
}
