using FontStashSharp;
using Microsoft.Xna.Framework;
using System;

namespace Rubedo.Lib;

/// <summary>
/// Provides functions for various font+text related things.
/// </summary>
public static class TextUtils
{
    /// <summary>
    /// Measures text using a font with a given size. The size is as tight as possible to the text.
    /// </summary>
    public static Vector2 MeasureStringTight(FontSystem fontSystem, string text, int size)
    {
        return fontSystem.GetFont(size).MeasureString(text);
    }
    /// <summary>
    /// Measures text using a font with a given size. The line height is the same no matter the text content.
    /// </summary>
    public static Vector2 MeasureString(FontSystem fontSystem, string text, int size)
    {
        var font = fontSystem.GetFont(size);
        return new Vector2(font.MeasureString(text).X, font.FontSize * CountLines(text));
    }
    public static int CountLines(string text)
    {
        ReadOnlySpan<char> chars = text.AsSpan();
        int count = 0;
        int length = chars.Length;
        for (int i = 0; i < length; i++)
        {
            if (chars[i] == '\n' && i != length - 1)
                count++;
        }
        return count;
    }
}