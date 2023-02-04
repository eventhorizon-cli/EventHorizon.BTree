using BenchmarkDotNet.Running;
using EventHorizon.BTree.Benchmarks;

new BenchmarkSwitcher(new[]
{
    typeof(BTreeReadBenchmarks),
    typeof(BTreeWriteBenchmarks),
    typeof(BTree_PriorityQueue_EnequeueBenchmarks),
    typeof(BTree_PriorityQueue_DequeueBenchmarks),
    typeof(BTree_DictionaryReadBenchmarks),
}).Run(args, new BenchmarkConfig());