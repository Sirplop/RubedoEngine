using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rubedo.Render;

namespace Rubedo.Components;

/// <summary>
/// Basic text renderer component.
/// </summary>
public class Text : Component
{
    public enum CenterText
    {
        None,
        Horizontal,
        Vertical,
        Both
    }

    protected SpriteFont font;
    public string text;
    public Color color;
    public CenterText centerMode = CenterText.None;

    public Text(SpriteFont font, string text, Color color, bool active, bool visible) : base(active, visible)
    {
        this.font = font;
        this.text = text;
        this.color = color;
    }

    public override void Draw(Renderer sb)
    {
        //AdjustPositionForCentering(sb.Sprites);
        sb.DrawString(font, text, transform.WorldPosition, color, transform.RotationDegrees, Lib.Math.Max(transform.WorldScale) / RubedoEngine.Instance.Camera.GetZoom(), SpriteEffects.None);
    }

    protected void AdjustPositionForCentering(SpriteBatch sb)
    {
        Rectangle viewport = sb.GraphicsDevice.Viewport.Bounds;
        Vector2 stringSize = font.MeasureString(text);

        float x = 0;
        float y = 0;
        switch (centerMode)
        {
            case CenterText.Horizontal:
                x = (viewport.Width - stringSize.X) * 0.5f;
                break;
            case CenterText.Vertical:
                y = (viewport.Height - stringSize.Y) * 0.5f;
                break;
            case CenterText.Both:
                x = (viewport.Width - stringSize.X) * 0.5f;
                y = (viewport.Height - stringSize.Y) * 0.5f;
                break;
            case CenterText.None:
            default:
                break;
        }
        transform.Position = new Vector2(x, y);
    }
}