// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;


BenchmarkRunner.Run<BenchmarkPerformance>();

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class BenchmarkPerformance
{
    [Params(1000,2000)]
    public int N;

    private string countries;
    private int index, numberOfCharactersToExract;

    [GlobalSetup]
    public void GlobalSetup()
    {
        countries = "India, USA, UK, Australia, Netherlands, Belgium";
        index = countries.LastIndexOf(",", StringComparison.Ordinal);
        numberOfCharactersToExract = countries.Length - index;
    }

    [Benchmark]
    public void Substring()
    {
        _ = countries.Substring(index, numberOfCharactersToExract);
    }

    [Benchmark]
    public void Span()
    {
        _ = countries.AsSpan().Slice(index, numberOfCharactersToExract);
    }
    
    [Benchmark]
    public void MemorySpan()
    {
        _ = countries.AsMemory().Span.Slice(index, numberOfCharactersToExract);
    }

    [Benchmark]
    public void Memory()
    {
        _ = countries.AsMemory().Slice(index, numberOfCharactersToExract);
    }
}