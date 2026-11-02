namespace LoadlyAggregator.Domain.Common.Interfaces;






public interface ILogExporter
{



    string Name { get; }






    void Configure(object loggerConfiguration);







    bool IsEnabled(string[] exporters, Dictionary<string, object> settings);
}
