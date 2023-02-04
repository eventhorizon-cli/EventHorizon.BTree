using BenchmarkDotNet.Attributes;

namespace EventHorizon.BTree.Benchmarks;

public class BTree_PriorityQueue_EnequeueBenchmarks
{
    [Params(1000, 1_0000, 10_0000)] public int DataSize;

    [Params(2, 4, 8, 16)] public int Degree;

    private HashSet<int> _data;

    [IterationSetup]
    public void Setup()
    {
        var random = new Random();
        _data = new HashSet<int>();
        while (_data.Count < DataSize)
        {
            var value = random.Next();
            _data.Add(value);
        }
    }

    [Benchmark]
    public void BTree_Add()
    {
        var btree = new BTree<int, int>(Degree);

        foreach (var value in _data)
        {
            btree.Add(value, value);
        }
    }

    [Benchmark]
    public void PriorityQueue_Enqueue()
    {
        var priorityQueue = new PriorityQueue<int, int>(DataSize);

        foreach (var value in _data)
        {
            priorityQueue.Enqueue(value, value);
        }
    }
}