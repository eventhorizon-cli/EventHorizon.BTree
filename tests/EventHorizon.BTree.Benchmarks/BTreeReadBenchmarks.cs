using BenchmarkDotNet.Attributes;

namespace EventHorizon.BTree.Benchmarks;

public class BTreeReadBenchmarks
{
    [Params(2, 3, 4, 5, 6)] public int Degree { get; set; }

    private BTree<int, int> _bTree;
    
    [GlobalSetup]
    public void Setup()
    {
        _bTree = new BTree<int, int>(Degree);
        for (var i = 0; i < 1000; i++)
        {
            _bTree.Add(i, i);
        }
    }

    [Benchmark]
    public void ReadSequential()
    {
        for (var i = 0; i < 1000; i++)
        {
            _bTree.TryGetValue(i, out var value);
        }
    }

    [Benchmark]
    public void ReadRandom()
    {
        var random = new Random();
        for (var i = 0; i < 1000; i++)
        {
            _bTree.TryGetValue(random.Next(0, 1000), out var value);
        }
    }

    [Benchmark]
    public void Min()
    {
        _bTree.Min();
    }
    
    [Benchmark]
    public void Max()
    {
        _bTree.Max();
    }
    
    [Benchmark]
    public void Enumerate()
    {
        foreach (var _ in _bTree)
        {
        }
    }
}