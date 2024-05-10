using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;

namespace AsyncEnumerablePoC.Client;
public class HistoricalDataBenchmarkDbReadOnly : HistoricalDataBenchmark
{
    [Benchmark(Baseline = true)]
    public async Task<double> DbReadOnlyCollection()
    {
        var results = await HistoricalDataProvider.GetHistoricalData().ToArrayAsync();
        double a = 0.0;
        foreach (var result in results)
        {
            a = result.Value;
        }

        return a;
    }

    [Benchmark]
    public async Task<double> DbReadOnlyAsyncEnumerable()
    {
        var results = HistoricalDataProvider.GetHistoricalData().AsAsyncEnumerable();
        double a = 0.0;
        await foreach (var result in results)
        {
            a = result.Value;
        }

        return a;
    }
}
