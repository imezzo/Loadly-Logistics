using System.Linq;

namespace LoadlyAggregator.Infrastructure.Filters;

public interface IFilter<TEntity>
{
    string Key { get; }

    IQueryable<TEntity> Apply(IQueryable<TEntity> query, object? value);
}
