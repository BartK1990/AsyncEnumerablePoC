using BenchmarkDotNet.Attributes;

namespace AsyncEnumerablePoC.Client;
public class HistoricalDataBenchmarkDbReadTransformedTwice : HistoricalDataBenchmark
{
    [Benchmark(Baseline = true)]
    public async Task<double> DbReadTransformedTwiceCollection()
    {
        var results = await HistoricalDataProvider.GetHistoricalDataTransformedTwiceCollection();
        double a = 0.0;
        foreach (var result in results)
        {
            a = result.Value;
        }

        return a;
    }

    [Benchmark]
    public async Task<double> DbReadTransformedTwiceAsyncEnumerable()
    {
        var results = HistoricalDataProvider.GetHistoricalDataTransformedTwiceAsyncEnumerable();
        double a = 0.0;
        await foreach (var result in results)
        {
            a = result.Value;
        }

        return a;
    }
}
