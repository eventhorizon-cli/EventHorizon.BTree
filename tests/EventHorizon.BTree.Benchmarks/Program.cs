using BenchmarkDotNet.Running;
using EventHorizon.BTree.Benchmarks;

new BenchmarkSwitcher(new[]
{
    typeof(BTreeReadBenchmarks),
    typeof(BTreeWriteBenchmarks),
    typeof(BTree_PriorityQueueBenchmarks),
    typeof(BTree_DictionaryReadBenchmarks),
}).Run(args, new BenchmarkConfig());