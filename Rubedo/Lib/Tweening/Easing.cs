using Microsoft.Xna.Framework;
using System;

namespace Rubedo.Lib.Tweening;

public static class Easing
{
    #region util functions
    /// <summary>
    /// Returns the lerp between a and b at the given weight for b.
    /// </summary>
    public static float Mix(float a, float b, float weightB)
    {
        //also written as   a + (b - a) * t = c
        return (1 - weightB) * a + weightB * b;
    }
    /// <summary>
    /// Returns the lerp between a and b at the given weight for b.
    /// </summary>
    public static int Mix(int a, int b, float weightB)
    {
        return (int)((1 - weightB) * a) + (int)(weightB * b);
    }
    /// <summary>
    /// Un-lerps a in the mixing formula, given c, b, and t.
    /// </summary>
    public static float UnMix(float c, float b, float t)
    {
        return (-b * t + c) / (-t + 1f);
    }
    public static float Scale(float i, float t)
    {
        return i * t;
    }
    public static float ReverseScale(float i, float t)
    {
        return i * (1f - t);
    }
    #endregion
    #region misc
    public static float Flip(float flip)
    {
        return 1f - Math.Clamp01(flip);
    }
    public static float ClampBottom(float t)
    {
        return System.Math.Abs(t);
    }
    public static float ClampTop(float t)
    {
        return 1f - System.Math.Abs(1f - t);
    }
    public static float ClampTopBottom(float t)
    {
        return ClampTop(ClampBottom(t));
    }
    #endregion

    public static float Linear(float t) => t;

    public static float SmoothStart2(float t) => Power.In(t, 2);
    public static float SmoothStop2(float t) => Power.Out(t, 2);
    public static float SmoothStep2(float t) => Power.InOut(t, 2);

    public static float SmoothStart3(float t) => Power.In(t, 3);
    public static float SmoothStop3(float t) => Power.Out(t, 3);
    public static float SmoothStep3(float t) => Power.InOut(t, 3);

    public static float SmoothStart4(float t) => Power.In(t, 4);
    public static float SmoothStop4(float t) => Power.Out(t, 4);
    public static float SmoothStep4(float t) => Power.InOut(t, 4);

    public static float SmoothStart5(float t) => Power.In(t, 5);
    public static float SmoothStop5(float t) => Power.Out(t, 5);
    public static float SmoothStep5(float t) => Power.InOut(t, 5);

    public static float SmoothStart6(float t) => Power.In(t, 6);
    public static float SmoothStop6(float t) => Power.Out(t, 6);
    public static float SmoothStep6(float t) => Power.InOut(t, 6);

    public static float Arch(float t) => t * (1f - t) * 4;

    public static float Spike2(float t) => InOut(t, SmoothStart2);
    public static float Spike3(float t) => InOut(t, SmoothStart3);
    public static float Spike4(float t) => InOut(t, SmoothStart4);
    public static float Spike5(float t) => InOut(t, SmoothStart5);
    public static float Spike6(float t) => InOut(t, SmoothStart6);

    public static float SineIn(float t) => (float)MathF.Sin(t * MathHelper.PiOver2 - MathHelper.PiOver2) + 1;
    public static float SineOut(float t) => (float)MathF.Sin(t * MathHelper.PiOver2);
    public static float SineInOut(float t) => (float)(MathF.Sin(t * MathHelper.Pi - MathHelper.PiOver2) + 1) * 0.5f;

    public static float ExponentialIn(float t) => (float)System.Math.Pow(2, 10 * (t - 1));
    public static float ExponentialOut(float t) => Out(t, ExponentialIn);
    public static float ExponentialInOut(float t) => InOut(t, ExponentialIn);

    public static float CircleIn(float t) => (float)System.Math.Sqrt(1 - t * t) - 1;
    public static float CircleOut(float t) => (float)System.Math.Sqrt(1 - (t - 1) * (t - 1));
    public static float CircleInOut(float t) => (float)(t <= .5 
        ? (System.Math.Sqrt(1 - t * t * 4) - 1) * -0.5f 
        : (System.Math.Sqrt(1 - (t * 2 - 2) * (t * 2 - 2)) + 1) * 0.5f);

    public static float BackIn(float t)
    {
        const float AMPLITUDE = 1f;
        return (float)(System.Math.Pow(t, 3) - t * AMPLITUDE * System.Math.Sin(t * MathHelper.Pi));
    }
    public static float BackOut(float t) => Out(t, BackIn);
    public static float BackInOut(float t) => InOut(t, BackIn);

    public static float BounceIn(float t)
    {
        const float d1 = 2.75f;
        const float n1 = 7.5625f;

        if (t < 1f / d1)
            return 1 - n1 * t * t;
        else if (t < 2f / d1)
            return 1 - n1 * (float)System.Math.Pow(t - 1.5f / d1, 2) + 0.75f;
        else if (t < 2.5f / d1)
            return 1 - n1 * (float)System.Math.Pow(t - 2.25f / d1, 2) + 0.9375f;
        else
            return 1 - n1 * (float)System.Math.Pow(t - 2.625f / d1, 2) + 0.984375f;
    }
    public static float BounceOut(float t) => Out(t, BounceIn);
    public static float BounceInOut(float t) => InOut(t, BounceIn);

    private static float Out(float value, Func<float, float> function)
    {
        return 1 - function(1 - value);
    }

    private static float InOut(float value, Func<float, float> function)
    {
        if (value < 0.5f)
            return 0.5f * function(value * 2);

        return 1f - 0.5f * function(2 - value * 2);
    }
    private static class Power
    {
        public static float In(double value, int power)
        {
            return (float)System.Math.Pow(value, power);
        }

        public static float Out(double value, int power)
        {
            double x = 1 - value;
            return (float)System.Math.Pow(x, power);
        }
        public static float InOut(double value, int power)
        {
            return In(value, power) * (float)(1 - value) + Out(value, power);
        }
    }
}