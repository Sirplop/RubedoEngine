using FontStashSharp;
using Microsoft.Xna.Framework;
using Rubedo.Lib;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Rubedo.UI.Text.Rendering;

/// <summary>
/// A single line of text.
/// </summary>
public class TextLine
{
    public string Text { get; private set; }
    public Vector2 TextSize { get; private set; }

    public TextLine(string text, FontSystem font, int fontSize)
    {
        Text = text;
        var fontR = font.GetFont(fontSize);
        TextSize = fontR.MeasureString(text.AsSpan());

    }
    public TextLine(ReadOnlySpan<char> span, Vector2 size)
    {
        Text = span.ToString();
        TextSize = size;
    }

    /// <summary>
    /// Creates a list of TextLines from a given string, no word-wrapping.
    /// </summary>
    public static List<TextLine> GetTextLines(in string text, in FontSystem font, in int fontSize)
    {
        List<TextLine> lines = new List<TextLine>();

        if (string.IsNullOrEmpty(text))
            return lines;
        ReadOnlySpan<char> chars = text.AsSpan();
        int index = 0;
        int length = chars.Length;
        ReadOnlySpan<char> curLine;
        var fontR = font.GetFont(fontSize);
        for (int i = 0; i < length; i++)
        {
            if (chars[i] == '\n' && i != length - 1)
            {
                curLine = chars[index..(i - 1)];
                lines.Add(new TextLine(new string(curLine), font, fontSize));
                index = i;
            }
        }
        //copy final bit.
        curLine = chars.Slice(index, length - index);
        lines.Add(new TextLine(new string(curLine), font, fontSize));

        return lines;
    }

    /// <summary>
    /// Creates a list of TextLines from the <paramref name="text"/> such that they all fit within the provided <paramref name="maxWidth"/>.
    /// </summary>
    /// <param name="text">The text to be split up.</param>
    /// <param name="maxWidth">The maximum width of each line.</param>
    /// <param name="font">The font.</param>
    /// <param name="fontSize">The font size.</param>
    /// <param name="tight">If true, then each line will have a tight-fitting height. Otherwise all lines have the same height of <paramref name="fontSize"/>.</param>
    /// <returns></returns>
    public static List<TextLine> GetTextLinesWrap(in string text, in int maxWidth, in FontSystem font, in int fontSize, bool tight)
    {
        List<TextLine> lines = new List<TextLine>();
        if (string.IsNullOrEmpty(text))
            return lines;
        /*
         * We're going to scan the string for words until we run out of space for words,
         * then we'll grab the line out of the span. If we hit a newline, cut the line early.
         * UNLESS that newline is the end of the string, in which case it is ignored.
         */
        var fontR = font.GetFont(fontSize);

        ReadOnlySpan<char> chars = text.AsSpan();
        int curStartIndex = 0;
        int curWordStart = 0;

        int length = chars.Length;
        for (int i = 0; i < length; i++)
        {
            if (chars[i] == '\n' && i != length - 1) //newline, stop current line.
            {
                ReadOnlySpan<char> currentLine = chars[curStartIndex..(i)];
                Vector2 size = fontR.MeasureString(currentLine);
                if (size.X > maxWidth) //this word doesn't fit!
                {
                    currentLine = chars[curStartIndex..(curWordStart - 1)];
                    size = fontR.MeasureString(currentLine);
                    if (!tight)
                        size.Y = fontSize;
                    lines.Add(new TextLine(currentLine, size));
                    curStartIndex = curWordStart;
                } else
                {
                    if (!tight)
                        size.Y = fontSize;
                    lines.Add(new TextLine(currentLine, size));
                    curWordStart = i + 1;
                    curStartIndex = curWordStart;
                }
                curWordStart = i + 1;
                continue;
            }
            
            else if (chars[i] == ' ') //we've found the end of a word!
            {
                ReadOnlySpan<char> currentLine = chars[curStartIndex..(i)];
                Vector2 size = fontR.MeasureString(currentLine);
                if (size.X > maxWidth) //this word doesn't fit!
                {
                    currentLine = chars[curStartIndex..(curWordStart - 1)];
                    size = fontR.MeasureString(currentLine);
                    if (!tight)
                        size.Y = fontSize;
                    lines.Add(new TextLine(currentLine, size));
                    curStartIndex = curWordStart;
                }
                curWordStart = i + 1;
            }

            else if (i == length - 1) //we're at the end of the string, dump the rest into one or two lines.
            {
                ReadOnlySpan<char> currentLine = chars[curStartIndex..(i + 1)];
                Vector2 size = fontR.MeasureString(currentLine);
                if (size.X > maxWidth) //this word doesn't fit!
                {
                    currentLine = chars[curStartIndex..(curWordStart - 1)];
                    size = fontR.MeasureString(currentLine);
                    if (!tight)
                        size.Y = fontSize;
                    lines.Add(new TextLine(currentLine, size));
                    currentLine = chars[curWordStart..(i + 1)];
                    size = fontR.MeasureString(currentLine);
                    if (!tight)
                        size.Y = fontSize;
                    lines.Add(new TextLine(currentLine, size));
                } else //it all fits into one line!
                {
                    if (!tight)
                        size.Y = fontSize;
                    lines.Add(new TextLine(currentLine, size));
                }
            }
        }

        return lines;
    }
}