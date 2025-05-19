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
    public static Vector2 MeasureString(FontSystem fontSystem, in string text, in int size)
    {
        var font = fontSystem.GetFont(size);
        return new Vector2(font.MeasureString(text).X, font.FontSize * CountLines(in text));
    }
    public static int CountLines(in string text)
    {
        if (string.IsNullOrEmpty(text))
            return 0;
        ReadOnlySpan<char> chars = text.AsSpan();
        int count = 1;
        int length = chars.Length;
        for (int i = 0; i < length; i++)
        {
            if (chars[i] == '\n' && i != length - 1)
                count++;
        }
        return count;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>The input string with linebreaks for wrapping.</returns>
    public static string WrapText(FontSystem font, in string value, in int size, float maxLineWidth)
    {
        return value;
    }
}