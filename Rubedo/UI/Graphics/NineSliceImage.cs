using FontStashSharp;
using Microsoft.Xna.Framework;
using Rubedo.Graphics;

namespace Rubedo.UI.Graphics;

/// <summary>
/// TODO: I am Image, and I don't have a summary yet.
/// </summary>
public class NineSliceImage : UIComponent, IColorable
{
    public NineSlice Image { get; set; }
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

    public NineSliceImage(NineSlice region, int prefWidth, int prefHeight) : this(region, prefWidth, prefHeight, Color.White) { }
    public NineSliceImage(NineSlice region, int prefWidth, int prefHeight, Color color)
    {
        Image = region;
        this._prefWidth = prefWidth;
        this._prefHeight = prefHeight;
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
        Width = PrefWidth;
        Height = PrefHeight;
        base.UpdateSizes();
    }
    public override void Draw()
    {
        Rectangle bounds = new Rectangle(Clip.Left, Clip.Top, (int)Width, (int)Height);
        GUI.SpriteBatch.Draw(Image, bounds, Color, Clip);
        base.Draw();
    }
}