using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rubedo.Lib;
using Rubedo.Rendering;

namespace Rubedo.Components;

/// <summary>
/// Basic text renderer component.
/// </summary>
public class Text : Component
{
    public enum HorizontalAlignment
    {
        Left,
        Right,
        Center
    }
    public enum VerticalAlignment
    {
        Bottom,
        Top,
        Center
    }

    protected FontSystem font;
    public string text { get; protected set; }
    public Color color;
    public int fontSize = 12;
    public int layerDepth = 0;
    protected HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left;
    protected VerticalAlignment verticalAlignment = VerticalAlignment.Top;

    protected Vector2 alignmentOffset = Vector2.Zero;

    protected bool doShadow = false;
    protected int shadowThickness;
    protected Color shadowColor;

    public Text(FontSystem font) : this(font, string.Empty, Color.White) { }
    public Text(FontSystem font, string text, Color color) : base()
    {
        this.font = font;
        SetText(text);
        this.color = color;
    }

    public void SetShadow(int shadowThickness, Color shadowColor)
    {
        doShadow = true;
        this.shadowThickness = shadowThickness;
        this.shadowColor = shadowColor;
    }

    public void SetAlignment(HorizontalAlignment horz, VerticalAlignment vert)
    {
        this.horizontalAlignment = horz;
        this.verticalAlignment = vert;
        UpdateAlignment();
    }
    public void SetAlignment(VerticalAlignment vert)
    {
        this.verticalAlignment = vert;
        UpdateAlignment();
    }
    public void SetAlignment(HorizontalAlignment horz)
    {
        this.horizontalAlignment = horz;
        UpdateAlignment();
    }

    public void SetText(string text)
    {
        this.text = text;
    }

    protected void UpdateAlignment()
    {
        Vector2 size = TextUtils.MeasureString(font, text, fontSize);
        switch (horizontalAlignment)
        {
            case HorizontalAlignment.Left:
                alignmentOffset.X = 0;
                break;
            case HorizontalAlignment.Right:
                alignmentOffset.X = size.X;
                break;
            case HorizontalAlignment.Center:
                alignmentOffset.X = size.X * 0.5f;
                break;
        }
        switch (verticalAlignment)
        {
            case VerticalAlignment.Bottom:
                alignmentOffset.Y = 0;
                break;
            case VerticalAlignment.Top:
                alignmentOffset.Y = size.Y;
                break;
            case VerticalAlignment.Center:
                alignmentOffset.Y = size.Y * 0.5f;
                break;
        }
    }

    public override void Draw(Renderer sb)
    {
        Vector2 pos = compTransform.LocalPosition;
        MathV.MulSub(ref pos, ref alignmentOffset, fontSize, out pos);

        SpriteFontBase fontR = font.GetFont(fontSize);

        compTransform.LocalToWorldPosition(ref pos, out pos);

        sb.DrawString(fontR, Renderer.Space.World, text, pos, color, compTransform.RotationDegrees, 0.05f, layerDepth, SpriteEffects.None);
    }
}