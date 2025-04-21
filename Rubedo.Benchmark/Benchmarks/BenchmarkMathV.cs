using BenchmarkDotNet.Attributes;
using Microsoft.Xna.Framework;
using Random = Rubedo.Lib.Random;

namespace Rubedo.Benchmarks;

/// <summary>
/// I am BenchmarkMathV, and this is my summary.
/// </summary>
public class BenchmarkMathV
{
    private const float RUN_COUNT = 100;

    Vector2 val1;
    Vector2 val2;

    public BenchmarkMathV()
    {
        val1 = new Vector2(Random.Value, Random.Value);
        val2 = new Vector2(Random.Value, Random.Value);
    }

    [Benchmark(Baseline = true)]
    public bool OperatorEquals()
    {
        bool ret = false;
        for (int i = 0; i < RUN_COUNT; i++)
            ret = val1 == val2;
        return ret;
    }
    [Benchmark]
    public bool Vector2Equals()
    {
        bool ret = false;
        for (int i = 0; i < RUN_COUNT; i++)
         ret = Vector2.Equals(val1, val2);
        return ret;
    }
}