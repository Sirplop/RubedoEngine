using BenchmarkDotNet.Running;
using Rubedo.Benchmark.Benchmarks;
using Rubedo.Benchmarks;

namespace Rubedo.Benchmark;

/// <summary>
/// I am BenchmarkRunner, and this is my summary.
/// </summary>
internal class BenchmarkRun
{
    public static void RunBenchmark()
    {
        BenchmarkRunner.Run<BenchmarkPhysIntegrate>();
    }
}