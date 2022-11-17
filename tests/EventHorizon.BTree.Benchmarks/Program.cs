using BenchmarkDotNet.Running;
using EventHorizon.BTree.Benchmarks;

new BenchmarkSwitcher(new[]
{
    typeof(BTreeReadBenchmarks),
    typeof(BTreeAsPriorityQueueBenchmarks),
}).Run(args, new BenchmarkConfig());