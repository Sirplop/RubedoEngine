using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Rubedo.Graphics;
using Rubedo.Lib;
using Rubedo.Object;
using Rubedo.Graphics.Sprites;

namespace Rubedo.Components;

public class Sprite : RenderableComponent
{
    protected TextureRegion2D _texture;
    protected Color _color = Color.White;
    protected Vector2 _pivot;
    public Vector2 Pivot
    {
        get { return _pivot; }
        set
        {
            _pivot = value;
            if (_texture != null)
                _pixelPivot = new Vector2(MathF.Floor(_pivot.X * _texture.Width), MathF.Floor(_pivot.Y * _texture.Height));
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
                Vector2 position = Entity.Transform.Position;
                Vector2 scale = Entity.Transform.Scale;
                float rotation = Entity.Transform.Rotation;

                float left = (-Pivot.X * Width);
                float top = (-Pivot.Y * Height);
                float right = (1 - Pivot.X) * Width;
                float bottom = (1 - Pivot.Y) * Height;

                Vector2 a = Lib.MathV.RotateRadians(new Vector2(left, top), rotation, scale.X, scale.Y);
                Vector2 b = Lib.MathV.RotateRadians(new Vector2(right, top), rotation, scale.X, scale.Y);
                Vector2 c = Lib.MathV.RotateRadians(new Vector2(left, bottom), rotation, scale.X, scale.Y);
                Vector2 d = Lib.MathV.RotateRadians(new Vector2(right, bottom), rotation, scale.X, scale.Y);

                left = Lib.Math.Min(a.X, b.X, c.X, d.X);
                right = Lib.Math.Max(a.X, b.X, c.X, d.X);
                top = Lib.Math.Min(a.Y, b.Y, c.Y, d.Y);
                bottom = Lib.Math.Max(a.Y, b.Y, c.Y, d.Y);

                float width = right - left;
                float height = bottom - top;

                _bounds = new RectF(position.X + left, position.Y + top , width, height);
            }
            return _bounds;
        }
    }
    private RectF _bounds;
    private bool _boundsDirty = true;

    public Sprite(int layer, Color color) : this(string.Empty, layer, color) { }
    public Sprite(string texture) : this(Assets.LoadTexture(texture), 0, Color.White) { }
    public Sprite(string texture, int layer) : this(Assets.LoadTexture(texture), layer, Color.White) { }
    public Sprite(string texture, int layer, Color color) : this(Assets.LoadTexture(texture), layer, color) { }
    public Sprite(Texture2D texture) : this(texture, 0, Color.White) { }
    public Sprite(Texture2D texture, int layerDepth) : this(texture, layerDepth, Color.White) { }
    public Sprite(Texture2D texture, int layerDepth, Color color) : base()
    {
        _texture = texture;
        LayerDepth = layerDepth;
        Pivot = new Vector2(0.5f, 0.5f);
        SetColor(color);
    }
    public Sprite(TextureRegion2D texture) : this(texture, 0, Color.White) { }
    public Sprite(TextureRegion2D texture, int layerDepth) : this(texture, layerDepth, Color.White) { }
    public Sprite(TextureRegion2D texture, int layerDepth, Color color) : base()
    {
        _texture = texture;
        LayerDepth = layerDepth;
        Pivot = new Vector2(0.5f, 0.5f);
        SetColor(color);
    }

    public void SetTexture(TextureRegion2D newTexture)
    {
        _texture = newTexture;
        _pixelPivot = new Vector2(_pivot.X * _texture.Width, _pivot.Y * _texture.Height);
        _boundsDirty = true;
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
        if (_texture == null)
            return; //nothing to render.

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