using Microsoft.Xna.Framework;
using Rubedo.Graphics;
using Rubedo.Rendering;

namespace Rubedo.UI.Graphics;

/// <summary>
/// TODO: I am Image, and I don't have a summary yet.
/// </summary>
public class Image : UIComponent
{
    public Texture2DRegion Region { get; set; }
    public Color Color { get; set; }

    public Image(Texture2DRegion region) : this(region, Color.White) { }
    public Image(Texture2DRegion region, Color color)
    {
        Region = region;
        Color = color;
    }

    public override void UpdateSizes()
    {
        Width = Region.Width;
        Height = Region.Height;
    }
    public override void Draw()
    {
        GUI.PushScissor(Clip);

        Vector2 pos = new Vector2(Clip.Left, Clip.Top);
        GUI.SpriteBatch.Draw(Region, pos, Color);

        GUI.PopScissor();
    }
}