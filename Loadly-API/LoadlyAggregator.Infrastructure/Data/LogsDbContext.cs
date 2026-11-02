using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Infrastructure.Data;

public class LogsDbContext : DbContext
{
    public LogsDbContext(DbContextOptions<LogsDbContext> options) : base(options)
    {
    }

    public DbSet<NotificationsLog> NotificationsLogs { get; set; }

    public DbSet<AuditLog> AuditLogs { get; set; }

    public DbSet<OdometerScanLog> OdometerScanLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.ApplyConfiguration(new Configurations.NotificationsLogConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.AuditLogConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.OdometerScanLogConfiguration());


        ApplySoftDeleteQueryFilter(modelBuilder);
    }






    private void ApplySoftDeleteQueryFilter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {

            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {

                var parameter = Expression.Parameter(entityType.ClrType, "entity");
                var property = Expression.Property(parameter, nameof(ISoftDeletable.DeletedAt));
                var filter = Expression.Lambda(
                    Expression.Equal(property, Expression.Constant(null, typeof(DateTime?))),
                    parameter
                );


                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }
    }
}

