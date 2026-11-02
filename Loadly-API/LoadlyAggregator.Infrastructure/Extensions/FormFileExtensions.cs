using Microsoft.AspNetCore.Http;
using LoadlyAggregator.Domain.Common.Models;
namespace LoadlyAggregator.Infrastructure.Extensions;




public static class FormFileExtensions
{



    public static FileUpload ToDomainFile(this IFormFile formFile)
    {
        return new FileUpload
        {
            FileName = formFile.FileName,
            ContentType = formFile.ContentType,
            Length = formFile.Length,
            Content = formFile.OpenReadStream()
        };
    }



    public static FileUpload? ToDomainFileOrNull(this IFormFile? formFile)
    {
        return formFile?.ToDomainFile();
    }
}
