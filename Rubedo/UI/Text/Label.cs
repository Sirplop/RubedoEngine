using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rubedo.UI.Graphics;
using Rubedo.UI.Text.Rendering;
using System.Collections.Generic;

namespace Rubedo.UI.Text;

/// <summary>
/// An uneditable text box.
/// </summary>
/// <remarks>Use <see cref="UIComponent.MinSize"/> and <see cref="UIComponent.MaxSize"/> to set text box size.</remarks>
public class Label : UIComponent, IColorable
{
    public static bool DrawBackground
    {
        get => _drawBackground;
        set
        {
            if (testWhite == null)
            {
                testWhite = new Texture2D(RubedoEngine.Graphics.GraphicsDevice, 1, 1);
                testWhite.SetData(new[] { Color.White });
            }
            _drawBackground = value;
        }
    }
    private static bool _drawBackground = false;
    private static Texture2D testWhite;

    public enum HorizontalAlignment
    {
        Left,
        Center,
        Right
    }

    protected FontSystem font;
    public string Text 
    {
        get => _text; 
        set 
        {
            if (_text != value)
            {
                _text = value;
                MarkLayoutAsDirty();
                _isDirty = true;
            }
        } 
    }
    private string _text;
    public int FontSize
    {
        get => _fontSize;
        set
        {
            if (_fontSize != value)
            {
                _fontSize = value;
                MarkLayoutAsDirty();
                _isDirty = true;
            }
        }
    }
    private int _fontSize;

    public bool TightLineHeight
    {
        get => _tightLineHeight;
        set
        {
            if (_tightLineHeight != value)
            {
                _tightLineHeight = value;
                MarkLayoutAsDirty();
            }
        }
    }
    private bool _tightLineHeight = true;

    public HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left;

    public Color color;
    public float paddingX = 0;
    public float paddingY = 0;

    protected Vector2 textSize;

    protected bool _isDirty;

    protected List<TextLine> textLines;

    public Label(FontSystem font) : this(font, string.Empty, Color.White, 12) { }
    public Label(FontSystem font, Color color) : this(font, string.Empty, color, 12) { }
    public Label(FontSystem font, string text) : this(font, text, Color.White, 12) { }
    public Label(FontSystem font, string text, Color color) : this(font, text, color, 12) { }

    public Label(FontSystem font, string text, Color color, int fontSize)
    {
        this.font = font;
        this.FontSize = fontSize;
        this.Text = text;
        this.color = color;
    }
    public void SetColor(Color color)
    {
        this.color = color;
    }

    public Color GetColor()
    {
        return color;
    }

    public override void UpdateSizes()
    {
        UpdateIfDirty();
        Width = textSize.X;
        Height = textSize.Y;
        base.UpdateSizes();
    }

    protected void UpdateIfDirty()
    {
        if (_isDirty)
        {
            int maxSizeWidth = MaxSize.HasValue ? (int)MaxSize.Value.X : int.MaxValue;
            textLines = TextLine.GetTextLinesWrap(in _text, maxSizeWidth, in font, in _fontSize, _tightLineHeight);
            textSize = Vector2.Zero;
            for (int i = 0; i < textLines.Count; i++)
            {
                TextLine line = textLines[i];
                if (line.TextSize.X > textSize.X)
                    textSize.X = line.TextSize.X;
                textSize.Y += line.TextSize.Y;
            }
            _isDirty = false;
        }
    }

    public override void Draw()
    {
        DynamicSpriteFont fontR = font.GetFont(_fontSize);
        Vector2 pos;
        switch (horizontalAlignment)
        {
            default:
            case HorizontalAlignment.Left:
                pos = new Vector2(Clip.Left, Clip.Top);
                break;
            case HorizontalAlignment.Center:
                pos = new Vector2(Clip.Center.X, Clip.Top);
                break;
            case HorizontalAlignment.Right:
                pos = new Vector2(Clip.Right, Clip.Top);
                break;
        }
        for (int i = 0; i < textLines.Count; i++)
        {
            TextLine line = textLines[i];
            Vector2 realPos;
            switch (horizontalAlignment)
            {
                default:
                case HorizontalAlignment.Left:
                    realPos = pos;
                    break;
                case HorizontalAlignment.Center:
                    realPos = new Vector2(pos.X - (line.TextSize.X * 0.5f), pos.Y);
                    break;
                case HorizontalAlignment.Right:
                    realPos = new Vector2(pos.X - line.TextSize.X, pos.Y);
                    break;
            }
            if (DrawBackground)
            {
                GUI.SpriteBatch.Draw(testWhite, new Rectangle((int)realPos.X, (int)realPos.Y, (int)line.TextSize.X, (int)line.TextSize.Y), Color.Green);
                var glyphs = fontR.GetGlyphs(line.Text, realPos);
                foreach (var g in glyphs)
                {
                    GUI.SpriteBatch.Draw(testWhite, g.Bounds, Color.Wheat);
                }
            }
            GUI.SpriteBatch.DrawString(fontR, line.Text, realPos, color);
            pos.Y += line.TextSize.Y;
        }
        base.Draw();

        /*
        DynamicSpriteFont fontR = font.GetFont(_fontSize);
        Vector2 pos = new Vector2(Clip.Left, Clip.Top);
        GUI.SpriteBatch.DrawString(fontR, _text, pos, color);
        base.Draw();
        */
    }

    public override void MarkLayoutAsDirty()
    {
        base.MarkLayoutAsDirty();
        _isDirty = true;
    }
}