using Microsoft.AspNetCore.Mvc.ModelBinding;
using LoadlyAggregator.Domain.Common.Models;

namespace LoadlyAggregator.API.ModelBinders;

/// <summary>
/// Unified model binder that handles both single FileUpload and List of FileUpload properties
/// </summary>
public class FileUploadModelBinder : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var modelType = bindingContext.ModelMetadata.ModelType;
        var modelName = bindingContext.ModelName;

        // Route to appropriate binding method based on type
        await (IsListType(modelType)
            ? BindListAsync(bindingContext, modelName)
            : BindSingleAsync(bindingContext, modelName));
    }

    private static async Task BindSingleAsync(ModelBindingContext bindingContext, string modelName)
    {
        var formFile = bindingContext.HttpContext.Request.Form.Files.GetFile(modelName);
        var fileUpload = formFile is { Length: > 0 }
            ? await CreateFileUploadAsync(formFile)
            : null;

        bindingContext.Result = ModelBindingResult.Success(fileUpload);
    }

    private static async Task BindListAsync(ModelBindingContext bindingContext, string modelName)
    {
        var formFiles = bindingContext.HttpContext.Request.Form.Files.GetFiles(modelName);

        if (formFiles is not { Count: > 0 })
        {
            bindingContext.Result = ModelBindingResult.Success(null);
            return;
        }

        var fileUploads = new List<FileUpload>();
        foreach (var formFile in formFiles.Where(f => f.Length > 0))
        {
            fileUploads.Add(await CreateFileUploadAsync(formFile));
        }

        bindingContext.Result = ModelBindingResult.Success(fileUploads.Count > 0 ? fileUploads : null);
    }

    private static async Task<FileUpload> CreateFileUploadAsync(Microsoft.AspNetCore.Http.IFormFile formFile)
    {
        // Buffer the stream immediately to avoid ReferenceReadStream issues
        var memoryStream = new MemoryStream();
        using (var sourceStream = formFile.OpenReadStream())
        {
            await sourceStream.CopyToAsync(memoryStream);
        }
        memoryStream.Position = 0;

        return new FileUpload
        {
            FileName = formFile.FileName,
            ContentType = formFile.ContentType,
            Length = formFile.Length,
            Content = memoryStream
        };
    }

    private static bool IsListType(Type modelType) =>
        modelType.IsGenericType &&
        (modelType.GetGenericTypeDefinition() == typeof(List<>) ||
         modelType.GetGenericTypeDefinition() == typeof(IList<>) ||
         modelType.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
         modelType.GetGenericTypeDefinition() == typeof(ICollection<>)) &&
        modelType.GetGenericArguments()[0] == typeof(FileUpload);
}

/// <summary>
/// Unified provider that handles both FileUpload and List of FileUpload
/// </summary>
public class FileUploadModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var modelType = context.Metadata.ModelType;
        var underlyingType = Nullable.GetUnderlyingType(modelType) ?? modelType;

        // Return binder for single FileUpload or List<FileUpload>
        return underlyingType == typeof(FileUpload) || IsFileUploadListType(modelType)
            ? new FileUploadModelBinder()
            : null;
    }

    private static bool IsFileUploadListType(Type modelType) =>
        modelType.IsGenericType &&
        (modelType.GetGenericTypeDefinition() == typeof(List<>) ||
         modelType.GetGenericTypeDefinition() == typeof(IList<>) ||
         modelType.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
         modelType.GetGenericTypeDefinition() == typeof(ICollection<>)) &&
        modelType.GetGenericArguments()[0] == typeof(FileUpload);
}
