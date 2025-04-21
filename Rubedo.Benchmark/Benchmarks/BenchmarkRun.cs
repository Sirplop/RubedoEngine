using BenchmarkDotNet.Running;

namespace Rubedo.Benchmarks;

/// <summary>
/// I am BenchmarkRunner, and this is my summary.
/// </summary>
internal class BenchmarkRun
{
    public static void RunBenchmark()
    {
        BenchmarkRunner.Run<BenchmarkMathV>();
    }
}