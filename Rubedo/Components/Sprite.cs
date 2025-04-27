using Rubedo.Object;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Rubedo.Rendering;

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
    public Color Color
    {
        get { return _color; }
        set => SetColor(value);
    }

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

    [Obsolete]
    public bool Test(Vector2 point)
    {
        Color[] T = new Color[1];
        Vector2 pos = compTransform.Position;
        int x = Lib.Math.FloorToInt(point.X) - Lib.Math.FloorToInt(pos.X) + _texture.Width / 2;
        int y = Lib.Math.FloorToInt(point.Y) - Lib.Math.FloorToInt(pos.Y) + _texture.Height / 2;
        if (x < 0 || y < 0)
            return false;
        if (x > _texture.Width || y > _texture.Height)
            return false;

        x = Math.Min(x, _texture.Width - 1);
        y = Math.Min(y, _texture.Height - 1);
        Rectangle rect = new Rectangle(x, y, 1, 1);
        _texture.GetData(0, rect, T, 0, 1);

        return T[0].A != 0;
    }
}