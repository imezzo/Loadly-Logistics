namespace LoadlyAggregator.Domain.Common.Interfaces;



public interface IRequiresPermission
{



    string RequiredPermission { get; }
}
