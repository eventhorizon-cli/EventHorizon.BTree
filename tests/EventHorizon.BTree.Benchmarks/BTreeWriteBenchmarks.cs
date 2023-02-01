using BenchmarkDotNet.Attributes;

namespace EventHorizon.BTree.Benchmarks;

public class BTreeWriteBenchmarks
{
    [Params(2, 3, 4, 5, 6)] public int Degree { get; set; }
    
    private HashSet<int> _randomKeys;
    
    [GlobalSetup]
    public void Setup()
    {
        _randomKeys = new HashSet<int>();
        var random = new Random();
        while (_randomKeys.Count < 1000)
        {
            _randomKeys.Add(random.Next(0, 100000));
        }
    }
    
    [Benchmark]
    public void WriteSequential()
    {
        var bTree = new BTree<int, int>(Degree);
        for (var i = 0; i < 1000; i++)
        {
            bTree.Add(i, i);
        }
    }
    
    [Benchmark]
    public void WriteRandom()
    {
        var bTree = new BTree<int, int>(Degree);
        foreach (var key in _randomKeys)
        {
            bTree.Add(key, key);
        }
    }
}