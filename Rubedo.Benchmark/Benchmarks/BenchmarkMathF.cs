using BenchmarkDotNet.Attributes;
using Random = Rubedo.Lib.Random;

namespace Rubedo.Benchmarks;

[MemoryDiagnoser]
public class BenchmarkMathF
{
    private const float RUN_COUNT = 100;

    float val1;
    float val2;
    float val3;

    int val1I;
    int val2I;
    int val3I;

    public BenchmarkMathF()
    {
        val1 = Random.Range(0, RUN_COUNT);
        val2 = Random.Range(RUN_COUNT, RUN_COUNT * 2);
        val3 = Random.Range(RUN_COUNT * 2, RUN_COUNT * 3);
        val1I = Random.Range(0, 101);
        val2I = Random.Range(0, 101);
        val3I = Random.Range(0, 101);
    }

    [Benchmark(Baseline = true)]
    public float ClampFloat()
    {
        return Lib.Math.Clamp(val2, val1, val3);
    }
    [Benchmark]
    public float ClampFloatSystem2()
    {
        return System.Math.Clamp(val2, val1, val3);
    }
}
