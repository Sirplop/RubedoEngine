using Rubedo.Object;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rubedo.Render;
using System;

namespace Rubedo.Components;

public class Sprite : Component
{

    protected Texture2D _texture;
    protected Color _color = Color.White;
    protected int layer = 0;
    protected Vector2 _pivot;
    public Vector2 Pivot
    {
        get { return _pivot; }
        set
        {
            _pivot = value;
            _pixelPivot = new Vector2(_pivot.X * _texture.Width, _pivot.Y * _texture.Height);
        }
    }
    public Vector2 PixelPivot
    {
        get
        {
            return _pixelPivot;
        }
    }
    private Vector2 _pixelPivot;
    public Color Color => _color;

    public float Width => _texture.Width;
    public float Height => _texture.Height;

    public Vector2 HalfWH => _halfWH;
    protected Vector2 _halfWH;

    public Sprite(string texture) : this(AssetManager.LoadTexture(texture), true, true) { }
    public Sprite(string texture, bool active, bool visible) : this(AssetManager.LoadTexture(texture), active, visible) { }
    public Sprite(Texture2D texture) : this(texture, true, true) { }
    public Sprite(Texture2D texture, bool active, bool visible) : base(active, visible)
    {
        _texture = texture ?? throw new NullReferenceException("Sprite texture cannot be null!");
        Pivot = new Vector2(0.5f, 0.5f);
        _halfWH = new Vector2(Width * 0.5f, Height * 0.5f);
    }

    public void SetColor(Color color)
    {
        this._color = color;
    }

    public override void Draw(Renderer sb)
    {
        if (!visible)
            return;

        sb.Draw(
            _texture,
            compTransform.Position,
            null,
            _color,
            compTransform.Rotation,
            PixelPivot,
            compTransform.Scale,
            SpriteEffects.None, 0f);
    }
}
