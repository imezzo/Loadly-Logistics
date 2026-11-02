using System.Linq;
using FluentValidation;
using LoadlyAggregator.Domain.Common.Models;

namespace LoadlyAggregator.API.Validators.Extensions;

public static class FileValidationRules
{
    public static IRuleBuilderOptions<T, FileUpload?> IsValidFileType<T>(
        this IRuleBuilder<T, FileUpload?> ruleBuilder,
        string[] allowedMimeTypes,
        string errorMessage = "documents.file_type_not_allowed")
    {
        return ruleBuilder
            .Must(file =>
            {
                if (file == null || file.Length == 0)
                    return true;
                var contentType = file.ContentType.ToLowerInvariant();
                var fileName = file.FileName.ToLowerInvariant();
                var extension = Path.GetExtension(fileName).TrimStart('.').ToLowerInvariant();
                return allowedMimeTypes.Any(type =>
                    contentType.Contains(type.ToLowerInvariant(), StringComparison.Ordinal) ||
                    fileName.EndsWith($".{type.ToLowerInvariant()}", StringComparison.Ordinal) ||
                    extension == type.ToLowerInvariant());
            })
            .WithMessage(errorMessage);
    }
}
