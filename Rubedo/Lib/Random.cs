using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;

namespace Rubedo.Lib;

/// <summary>
/// Implementation of Squirrel3 RNG. Extremely fast noise that does not suck eggs.
/// </summary>
public struct Squirrel3
{
    private const uint NOISE1 = 0xb5297a4d;
    private const uint NOISE2 = 0x68e31da4;
    private const uint NOISE3 = 0x1b56c4e9;

    private const float INV_2POW24 = 1f / (1 << 24);

    private int _n;
    private int _seed;

    public Squirrel3(int seed = 0)
    {
        _n = 0;
        _seed = seed;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Next()
    {
        ++_n;
        return Rnd(_n, _seed) * INV_2POW24; // Rnd already returns top-24-bit range, see below
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int NextRaw()
    {
        ++_n;
        return (int)RndFull(_n, _seed);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Range(float min, float max)
    {
        return Next() * (max - min) + min;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Range(int min, int max)
    {
        return Math.FloorToInt(Next() * (max - min) + min);
    }

    /// <summary>
    /// Truncates the output to 24 bits.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint Rnd(int n, int seed = 0)
    {
        uint x = (uint)n;
        x *= NOISE1;
        x += (uint)seed;
        x ^= x >> 8;
        x += NOISE2;
        x ^= x << 8;
        x *= NOISE3;
        x ^= x >> 8;
        return x >> 8;
    }
    /// <summary>
    /// Full length RND function without truncating to 24.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint RndFull(int n, int seed = 0)
    {
        uint x = (uint)n;
        x *= NOISE1;
        x += (uint)seed;
        x ^= x >> 8;
        x += NOISE2;
        x ^= x << 8;
        x *= NOISE3;
        x ^= x >> 8;
        return x;
    }

    /// <summary>
    /// Gets a random value in the range 0..1
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Value() => Range(0f, 1f);

    /// <summary>
    /// Returns true or false, with equal chance for either.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Flip() => Range(0f, 1f) < 0.5f;
    /// <summary>
    /// Randomly returns 0 or 1.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int FlipInt() => Range(0, 2);
    /// <summary>
    /// Uses Flip to yield either a positive or negative 1.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int PosNeg() => Flip() ? -1 : 1;
    /// <summary>
    /// Gets an integer between 0 and 100 (exclusive).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Percent() => Range(0, 100);
    /// <summary>
    /// Gets a float between 0 and 360f.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Angle() => Range(0f, 360f);
}

public static class Random
{
    private static Squirrel3 rnd = new Squirrel3((int)DateTime.Now.Ticks);
    public static ref Squirrel3 GetRND => ref rnd;

    /// <summary>
    /// Gets a random value in the range 0..1
    /// </summary>
    public static float Value => rnd.Range(0f, 1f);

    /// <summary>
    /// Returns true or false, with equal chance for either.
    /// </summary>
    public static bool Flip => rnd.Range(0f, 1f) < 0.5f;
    /// <summary>
    /// Randomly returns 0 or 1.
    /// </summary>
    public static int FlipInt => rnd.Range(0, 2);
    /// <summary>
    /// Uses Flip to yield either a positive or negative 1.
    /// </summary>
    public static int PosNeg => Flip ? -1 : 1;
    /// <summary>
    /// Gets an integer between 0 and 100 (exclusive).
    /// </summary>
    public static int Percent => rnd.Range(0, 100);
    /// <summary>
    /// Gets a float between 0 and 360f.
    /// </summary>
    public static float Angle => rnd.Range(0f, 360f);

    public static double Next()
    {
        return rnd.Next();
    }

    public static float Next(float max)
    {
        return rnd.Range(0f, max);
    }
    public static int Next(int max)
    {
        return rnd.Range(0, max);
    }

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
