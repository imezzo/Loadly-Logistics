using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
namespace LoadlyAggregator.Infrastructure.Data;

public class LogsDbContextFactory : IDesignTimeDbContextFactory<LogsDbContext>
{
    public LogsDbContext CreateDbContext(string[] args)
    {
        var configuration = DesignTimeConfiguration.Build();
        var connectionString = configuration.GetConnectionString("LogsConnection")
            ?? throw new InvalidOperationException("Connection string 'LogsConnection' not found.");
        var optionsBuilder = new DbContextOptionsBuilder<LogsDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        return new LogsDbContext(optionsBuilder.Options);
    }
}
