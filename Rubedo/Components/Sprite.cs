using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Rubedo.Rendering;
using Rubedo.Internal.Assets;
using Rubedo.Graphics;
using Rubedo.Lib;
using Rubedo.Object;

namespace Rubedo.Components;

public class Sprite : RenderableComponent
{
    protected Texture2DRegion _texture;
    protected Color _color = Color.White;
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

    public override RectF Bounds
    {
        get
        {
            if (_boundsDirty)
            {
                _boundsDirty = false;
                Rectangle bounds = _texture.Bounds;
                Vector2 position = Entity.Transform.Position;
                Vector2 scale = Entity.Transform.Scale;
                float rotation = Entity.Transform.Rotation;

                Vector2 a = Lib.MathV.RotateRadians(new Vector2(bounds.Left, bounds.Top), rotation, scale.X, scale.Y);
                Vector2 b = Lib.MathV.RotateRadians(new Vector2(bounds.Right, bounds.Top), rotation, scale.X, scale.Y);
                Vector2 c = Lib.MathV.RotateRadians(new Vector2(bounds.Left, bounds.Bottom), rotation, scale.X, scale.Y);
                Vector2 d = Lib.MathV.RotateRadians(new Vector2(bounds.Right, bounds.Bottom), rotation, scale.X, scale.Y);

                float left = Lib.Math.Min(a.X, b.X, c.X, d.X);
                float right = Lib.Math.Max(a.X, b.X, c.X, d.X);
                float top = Lib.Math.Min(a.Y, b.Y, c.Y, d.Y);
                float bottom = Lib.Math.Max(a.Y, b.Y, c.Y, d.Y);

                float width = right - left;
                float height = bottom - top;

                _bounds = new RectF(position.X + left - width / 2, position.Y + top - height / 2, width, height);
            }
            return _bounds;
        }
    }
    private RectF _bounds;
    private bool _boundsDirty = true;

    public Sprite(string texture) : this(AssetManager.LoadTexture(texture), 0, Color.White) { }
    public Sprite(string texture, int layer) : this(AssetManager.LoadTexture(texture), layer, Color.White) { }
    public Sprite(string texture, int layer, Color color) : this(AssetManager.LoadTexture(texture), layer, color) { }
    public Sprite(Texture2D texture) : this(texture, 0, Color.White) { }
    public Sprite(Texture2D texture, int layerDepth) : this(texture, layerDepth, Color.White) { }
    public Sprite(Texture2D texture, int layerDepth, Color color) : base()
    {
        _texture = texture ?? throw new NullReferenceException("Sprite texture cannot be null!");
        LayerDepth = layerDepth;
        Pivot = new Vector2(0.5f, 0.5f);
        SetColor(color);
    }

    public override void TransformChanged()
    {
        _boundsDirty = true;
    }

    public void SetColor(Color color)
    {
        this._color = color;
    }

    public override void Render(Renderer sb, Camera camera)
    {
        if (!Visible || !IsVisibleToCamera(camera))
            return;

        sb.Draw(
            _texture,
            Entity.Transform,
            null,
            _color,
            PixelPivot,
            SpriteEffects.None, _layerDepth);
    }
}