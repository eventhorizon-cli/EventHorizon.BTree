using BenchmarkDotNet.Attributes;

namespace EventHorizon.BTree.Benchmarks;

public class BTreeReadBenchmarks
{
    [Params(100)] public int DataSize;

    [Params(2, 4, 8)] public int Degree;

    private BTree<int, int> _btree;
    private Dictionary<int, int> _dictionary;
    private HashSet<int> _data;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random();
        _data = new HashSet<int>();
        while (_data.Count < DataSize)
        {
            var value = random.Next();
            _data.Add(value);
        }

        _btree = new BTree<int, int>(Degree);
        _dictionary = new Dictionary<int, int>(DataSize);
        foreach (var value in _data)
        {
            _btree.Add(value, value);
            _dictionary.Add(value, value);
        }
    }

    [Benchmark]
    public void BTreeRead()
    {
        foreach (var key in _data)
        {
            _btree.TryGetValue(key, out _);
        }
    }

    [Benchmark]
    public void DictionaryRead()
    {
        foreach (var key in _data)
        {
            _dictionary.TryGetValue(key, out _);
        }
    }
}