using Microsoft.Xna.Framework;
using Rubedo.Graphics;

namespace Rubedo.UI.Graphics;

/// <summary>
/// A statically sized image.
/// </summary>
public class Image : UIComponent, IColorable
{
    public enum DrawMode
    {
        Default,
        Tiled
    }

    public Texture2DRegion Region { get; set; }
    public Color Color { get; set; }

    public int PrefWidth
    {
        get => _prefWidth;
        set
        {
            if (_prefWidth != value)
            {
                _prefWidth = value;
                MarkLayoutAsDirty();
            }
        }
    }
    private int _prefWidth = 0;
    public int PrefHeight
    {
        get => _prefHeight;
        set
        {
            if (_prefHeight != value)
            {
                _prefHeight = value;
                MarkLayoutAsDirty();
            }
        }
    }
    private int _prefHeight = 0;

    public DrawMode drawMode = DrawMode.Default;
    public Vector2 uvOffset = Vector2.Zero;

    public Image(Texture2DRegion region) : this(region, region.Width, region.Height, Color.White) { }
    public Image(Texture2DRegion region, Color color) : this(region, region.Width, region.Height, color) { }
    public Image(Texture2DRegion region, int prefWidth, int prefHeight) : this(region, prefWidth, prefHeight, Color.White) { }
    public Image(Texture2DRegion region, int prefWidth, int prefHeight, Color color)
    {
        Region = region;
        _prefWidth = prefWidth;
        _prefHeight = prefHeight;
        Color = color;
    }
    public void SetColor(Color color)
    {
        Color = color;
    }
    public Color GetColor()
    {
        return Color;
    }

    public override void UpdateSizes()
    {
        Width = _prefWidth;
        Height = _prefHeight;
        base.UpdateSizes();
    }
    public override void Draw()
    {
        switch (drawMode)
        {
            case DrawMode.Default:
                Vector2 pos = new Vector2(Clip.Left, Clip.Top);
                GUI.SpriteBatch.Draw(Region, pos, Color);
                break;
            case DrawMode.Tiled:
                Rectangle destination = new Rectangle(Clip.Left, Clip.Top, (int)Width, (int)Height);
                GUI.SpriteBatch.DrawTiled(Region, destination, Color, Clip, uvOffset.X, uvOffset.Y);
                break;
        }
        base.Draw();
    }
}