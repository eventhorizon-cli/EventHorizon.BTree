using BenchmarkDotNet.Attributes;

namespace EventHorizon.BTree.Benchmarks;

public class BTree_PriorityQueue_DequeueBenchmarks
{
    [Params(1000, 1_0000,10_0000)] public int DataSize;

    [Params(2, 4, 8, 16)] public int Degree;

    private BTree<int, int> _btree;
    private HashSet<int> _data;
    private PriorityQueue<int, int> _priorityQueue;

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

        _btree = new BTree<int, int>(Degree);
        _priorityQueue = new PriorityQueue<int, int>(DataSize);
        
        foreach (var value in _data)
        {
            _btree.Add(value, value);
            _priorityQueue.Enqueue(value, value);
        }
    }
    
    [Benchmark]
    public void BTree_TryRemoveMin()
    {
        for (int i = 0; i < DataSize; i++)
        {
            _btree.TryRemoveMin(out var value);
        }
    }
    
    [Benchmark]
    public void PriorityQueue_Dequeue()
    {
        for (int i = 0; i < DataSize; i++)
        {
            _priorityQueue.Dequeue();
        }
    }
}