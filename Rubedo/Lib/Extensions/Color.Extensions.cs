using Microsoft.Xna.Framework;
using System.Globalization;
using System;
using System.Runtime.CompilerServices;

namespace Rubedo.Lib.Extensions;

/// <summary>
/// Provides various extensions and functions for <see cref="Color"/>.
/// </summary>
public static class ColorExtensions
{
    /// <summary>
    /// Multiplies colors <paramref name="a"/> and <paramref name="b"/> together. Each component scaled separately.
    /// </summary>
    public static Color Multiply(this Color a, Color b)
    {
        a.Deconstruct(out float aR, out float aG, out float aB, out float aA);
        b.Deconstruct(out float bR, out float bG, out float bB, out float bA);
        return new Color(aR * bR, aG * bG, aB * bB, aA * bA);
    }

    /// <summary>
    /// Gets the grayscale value of this color.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Grayscale(this Color color)
    {
        color.Deconstruct(out float r, out float g, out float b);
        return 0.299F * r + 0.587F * g + 0.114F * b;
    }

    /// <summary>
    /// Converts an HSV color value to RGB.
    /// h (hue) should be in [0,360), s (saturation) and v (value) in [0,1].
    /// </summary>
    public static void HsvToRgb(double h, double s, double v, out int r, out int g, out int b)
    {
        double c = v * s;
        double x = c * (1 - System.Math.Abs((h / 60) % 2 - 1));
        double m = v - c;
        double r1, g1, b1;

        if (h < 60)
        {
            r1 = c; g1 = x; b1 = 0;
        }
        else if (h < 120)
        {
            r1 = x; g1 = c; b1 = 0;
        }
        else if (h < 180)
        {
            r1 = 0; g1 = c; b1 = x;
        }
        else if (h < 240)
        {
            r1 = 0; g1 = x; b1 = c;
        }
        else if (h < 300)
        {
            r1 = x; g1 = 0; b1 = c;
        }
        else
        {
            r1 = c; g1 = 0; b1 = x;
        }

        r = (int)((r1 + m) * 255);
        g = (int)((g1 + m) * 255);
        b = (int)((b1 + m) * 255);
    }

    /// <summary>
    /// Converts an ARGB hexadecimal representation of a color into an actual color object.
    /// </summary>
    public static Color FromHexARGB(string hex)
    {
        uint argb = Convert.ToUInt32(hex.TrimStart('#').ToUpper(), 16);
        return new Color(ArgbToAbgr(argb));
    }
    /// <summary>
    /// Converts an RGBA hexadecimal representation of a color into an actual color object.
    /// </summary>
    public static Color FromHexRGBA(string hex)
    {
        uint rgba = Convert.ToUInt32(hex.TrimStart('#').ToUpper(), 16);
        return new Color(RgbaToAbgr(rgba));
    }
    /// <summary>
    /// Converts from ARGB to RGBA and back.
    /// </summary>
    public static uint ArgbToRgba(uint argb)
    {
        return (argb << 8) | (argb >> 24);
    }

    /// <summary>
    /// Converts from ARGB to ABGR and back.
    /// </summary>
    public static uint ArgbToAbgr(uint argb)
    {
        // Input:  [AA RR GG BB]
        // Output: [AA BB GG RR]
        return ( argb & 0xFF00FF00) |          // A and G stay in place
               ((argb & 0x00FF0000) >> 16) |  // R → byte 0 (LSB)
               ((argb & 0x000000FF) << 16);   // B → byte 3 (MSB, after A)
    }

    /// <summary>
    /// Converts from RGBA to ABGR and back.
    /// </summary>
    public static uint RgbaToAbgr(uint rgba)
    {
        // Input:  [RR GG BB AA]
        // Output: [AA BB GG RR]
        return ((rgba & 0xFF000000) >> 24) |  // R → byte 0 (LSB)
               ((rgba & 0x00FF0000) >> 8 ) |  // G → byte 1
               ((rgba & 0x0000FF00) << 8 ) |  // B → byte 2
               ((rgba & 0x000000FF) << 24);   // A → byte 3 (MSB)
    }
}