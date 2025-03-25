using Microsoft.Xna.Framework;
using System;

namespace Rubedo.Lib;

/// <summary>
/// Implementation of Squirrel3 RNG. Very fast and Burst compatible.
/// </summary>
public struct Squirrel3
{
    private const uint NOISE1 = 0xb5297a4d;
    private const uint NOISE2 = 0x68e31da4;
    private const uint NOISE3 = 0x1b56c4e9;
    private const uint CAP = uint.MaxValue;

    private int _n;
    private long _seed;

    public Squirrel3(long seed = 0)
    {
        _n = 0;
        _seed = seed;
    }

    public float Next()
    {
        ++_n;
        return Rnd(_n, _seed) / (float)CAP;
    }

    public long NextRaw()
    {
        ++_n;
        return Rnd(_n, _seed);
    }

    public float Range(float min, float max)
    {
        return Next() * (max - min) + min;
    }
    public int Range(int min, int max)
    {
        return Math.FloorToInt(Next() * (max - min) + min);
    }

    private static long Rnd(long n, long seed = 0)
    {
        n *= NOISE1;
        n += seed;
        n ^= n >> 8;
        n += NOISE2;
        n ^= n << 8;
        n *= NOISE3;
        n ^= n >> 8;
        return n % CAP;
    }

    //TODO
    //public void Print() => Debug.Log($"{_seed} - {_n}");
}

public static class Random
{
    private static Squirrel3 rnd = new Squirrel3(DateTime.Now.Ticks);

    /// <summary>
    /// Gets a random value in the range 0..1
    /// </summary>
    public static float Value => rnd.Range(0f, 1f);

    /// <summary>
    /// Returns true or false, with equal chance for either.
    /// </summary>
    public static bool Flip => rnd.Range(0f, 1f) < 0.5f;
    /// <summary>
    /// Uses Flip to yield either a positive or negative 1.
    /// </summary>
    public static int PosNeg => Flip ? -1 : 1;
    /// <summary>
    /// Gets an integer between 1 and 100 (inclusive).
    /// </summary>
    public static int Percent => Math.CeilToInt(rnd.Range(0f, 100f));
    /// <summary>
    /// Gets a float between 0 and 360f.
    /// </summary>
    public static float Angle => rnd.Range(0f, 360f);

    public static float Next => rnd.Next();

    /// <summary>
    /// Gets a random float between the min and max  (inclusive)
    /// </summary>
    public static float Range(float min, float max)
    {
        return rnd.Range(min, max);
    }
    /// <summary>
    /// Gets a random integer between min and max (exclusive)
    /// </summary>
    public static int Range(int min, int max)
    {
        return rnd.Range(min, max);
    }

    public static Color Color()
    {
        return new Color(rnd.Range(0, 256), rnd.Range(0, 256), rnd.Range(0, 256));
    }
}
