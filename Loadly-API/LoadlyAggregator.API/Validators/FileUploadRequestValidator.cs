using FluentValidation;
namespace LoadlyAggregator.API.Validators;




public static class FileUploadValidationRules
{




    public static IRuleBuilderOptionsConditions<T, IFormFile?> File<T>(
        this IRuleBuilder<T, IFormFile?> ruleBuilder,
        bool isRequired = false,
        long maxSizeInBytes = 20 * 1024 * 1024,
        string[]? allowedMimeTypes = null)
    {
        return ruleBuilder
            .Custom((file, context) =>
            {
                if (file == null)
                {
                    if (isRequired)
                    {
                        context.AddFailure("validation.file_required");
                    }
                    return;
                }

                if (file.Length > maxSizeInBytes)
                {
                    var maxSizeInMB = maxSizeInBytes / (1024 * 1024);
                    context.AddFailure($"validation.file_size_exceeds_mb|{maxSizeInMB}");
                }

                if (allowedMimeTypes != null && allowedMimeTypes.Length > 0)
                {
                    var contentType = file.ContentType.ToLower();
                    var fileName = file.FileName.ToLower();
                    var extension = Path.GetExtension(fileName).TrimStart('.').ToLower();
                    var isValidMimeType = allowedMimeTypes.Any(type =>
                        contentType.Contains(type.ToLower()) ||
                        fileName.EndsWith($".{type.ToLower()}") ||
                        extension == type.ToLower());
                    if (!isValidMimeType)
                    {
                        var allowedTypes = string.Join(", ", allowedMimeTypes);
                        context.AddFailure($"validation.file_type_must_be_one_of|{allowedTypes}");
                    }
                }
            });
    }




    public static IRuleBuilderOptionsConditions<T, IFormFile> FileArray<T>(
        this IRuleBuilder<T, IFormFile> ruleBuilder,
        long maxSizeInBytes = 20 * 1024 * 1024,
        string[]? allowedMimeTypes = null)
    {
        return ruleBuilder
            .Custom((file, context) =>
            {
                if (file == null || file.Length == 0)
                {
                    context.AddFailure("validation.file_empty");
                    return;
                }

                if (file.Length > maxSizeInBytes)
                {
                    var maxSizeInMB = maxSizeInBytes / (1024 * 1024);
                    context.AddFailure($"validation.file_size_exceeds_mb|{maxSizeInMB}");
                }

                if (allowedMimeTypes != null && allowedMimeTypes.Length > 0)
                {
                    var contentType = file.ContentType.ToLower();
                    var fileName = file.FileName.ToLower();
                    var extension = Path.GetExtension(fileName).TrimStart('.').ToLower();
                    var isValidMimeType = allowedMimeTypes.Any(type =>
                        contentType.Contains(type.ToLower()) ||
                        fileName.EndsWith($".{type.ToLower()}") ||
                        extension == type.ToLower());
                    if (!isValidMimeType)
                    {
                        var allowedTypes = string.Join(", ", allowedMimeTypes);
                        context.AddFailure($"validation.file_type_must_be_one_of|{allowedTypes}");
                    }
                }
            });
    }
}



public class ExampleFileUploadRequest
{
    public IFormFile? File { get; set; }
    public List<IFormFile>? Files { get; set; }
}
public class ExampleFileUploadRequestValidator : AbstractValidator<ExampleFileUploadRequest>
{
    public ExampleFileUploadRequestValidator()
    {

        RuleFor(x => x.File)
            .File(
                isRequired: true,
                maxSizeInBytes: 20 * 1024 * 1024,
                allowedMimeTypes: new[] { "png", "jpg", "jpeg", "gif", "pdf" }
            );

        RuleForEach(x => x.Files)
            .FileArray(
                maxSizeInBytes: 10 * 1024 * 1024,
                allowedMimeTypes: new[] { "png", "jpg", "jpeg", "gif" }
            )
            .When(x => x.Files != null && x.Files.Any());
    }
}
