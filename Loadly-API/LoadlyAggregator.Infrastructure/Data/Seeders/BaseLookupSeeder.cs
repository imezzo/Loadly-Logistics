using LoadlyAggregator.Domain.Common.Interfaces;
namespace LoadlyAggregator.Infrastructure.Data.Seeders;

public abstract class BaseLookupSeeder<T> where T : class, IEntity
{
    protected readonly ApplicationDbContext Context;
    protected readonly string? JsonFilePath;
    protected BaseLookupSeeder(ApplicationDbContext context, string? jsonFileName)
    {
        Context = context;

        if (!string.IsNullOrWhiteSpace(jsonFileName))
        {

            var projectRoot = GetInfrastructureProjectRoot();
            JsonFilePath = Path.Combine(
                projectRoot,
                "Data", "Seeders", "json", jsonFileName
            );
        }
    }



    private static string GetInfrastructureProjectRoot([System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
    {
        if (!string.IsNullOrEmpty(sourceFilePath))
        {
            var directory = new DirectoryInfo(Path.GetDirectoryName(sourceFilePath)!);

            while (directory != null)
            {
                if (Directory.GetFiles(directory.FullName, "*.csproj").Length > 0)
                {
                    return directory.FullName;
                }
                directory = directory.Parent;
            }
        }

        return AppDomain.CurrentDomain.BaseDirectory;
    }
    protected string GetJsonContent()
    {
        if (string.IsNullOrWhiteSpace(JsonFilePath))
        {
            throw new InvalidOperationException("JSON file path is not configured. This seeder does not use JSON files.");
        }
        if (!File.Exists(JsonFilePath))
        {
            throw new FileNotFoundException($"Seed file not found: {JsonFilePath}");
        }
        return File.ReadAllText(JsonFilePath);
    }
    public abstract Task SeedAsync();
    protected void LogSeeding(int count, string entityName)
    {
        Console.WriteLine($"✅ Seeded {count} {entityName}");
    }
    protected void LogSkipping(string entityName)
    {
        Console.WriteLine($"⏭️  {entityName} already seeded. Skipping.");
    }
    protected void LogError(string message)
    {
        Console.WriteLine($"❌ {message}");
    }
}
