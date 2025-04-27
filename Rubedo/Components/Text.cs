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

    /*
     *  Text is, without adjustment, aligned to the bottom left.
     */

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

    protected SpriteFont font;
    public string text { get; protected set; }
    public Color color;
    public float textSize = 1;
    protected HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left;
    protected VerticalAlignment verticalAlignment = VerticalAlignment.Top;

    protected Vector2 alignmentOffset = Vector2.Zero;

    protected bool doShadow = false;
    protected int shadowThickness;
    protected Color shadowColor;

    public Text(SpriteFont font) : this(font, string.Empty, Color.White, true, true) { }

    public Text(SpriteFont font, string text, Color color) : this(font, text, color, true, true) { }

    public Text(SpriteFont font, string text, Color color, bool active, bool visible) : base(active, visible)
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
        Vector2 size = font.MeasureString(text);
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
        MathV.MulSub(ref pos, ref alignmentOffset, textSize, out pos);

        float scale = textSize * (Math.Max(compTransform.Scale) / RubedoEngine.Instance.Camera.GetZoom());

        if (doShadow)
        {
            Vector2 shadowPos = new Vector2(shadowThickness, -shadowThickness);
            Vector2.Add(ref pos, ref shadowPos, out shadowPos);
            compTransform.LocalToWorldPosition(ref shadowPos, out shadowPos);

            sb.DrawString(font, text, shadowPos, shadowColor, compTransform.RotationDegrees, scale, SpriteEffects.None);
        }

        compTransform.LocalToWorldPosition(ref pos, out pos);

        sb.DrawString(font, text, pos, color, compTransform.RotationDegrees, scale, SpriteEffects.None);
    }
}