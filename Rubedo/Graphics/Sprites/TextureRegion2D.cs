using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rubedo.Lib;
using System;

namespace Rubedo.Graphics.Sprites;

/// <summary>
/// A region of a <see cref="Texture2D"/>.
/// </summary>
public class TextureRegion2D
{
    /// <summary>
    /// The backing texture of this region.
    /// </summary>
    public Texture2D Texture { get; }

    /// <summary>
    /// The name of this resource.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The X coordinate corner of this region.
    /// </summary>
    public int X { get; }
    /// <summary>
    /// The Y coordinate corner of this region.
    /// </summary>
    public int Y { get; }
    /// <summary>
    /// The width of the region.
    /// </summary>
    public int Width { get; }
    /// <summary>
    /// The height of the region.
    /// </summary>
    public int Height { get; }

    public Point Size => new Point(Width, Height);

    /// <summary>
    /// The bounds of the texture region within the texture.
    /// </summary>
    public Rectangle Bounds { get; }

    /// <summary>
    /// The left UV coordinate of the texture region.
    /// </summary>
    public float LeftUV { get; }
    /// <summary>
    /// The top UV coordinate of the texture region.
    /// </summary>
    public float TopUV { get; }
    /// <summary>
    /// The right UV coordinate of the texture region.
    /// </summary>
    public float RightUV { get; }

    /// <summary>
    /// The bottom UV coordinate of the texture region.
    /// </summary>
    public float BottomUV { get; }

    /// <summary>
    /// Constructs a new region representing the entire texture.
    /// </summary>
    public TextureRegion2D(Texture2D texture, string name = "")
        : this(texture, 0, 0, texture.Width, texture.Height, name) { }

    /// <summary>
    /// Constructs a new region representing the given rectangle.
    /// </summary>
    public TextureRegion2D(Texture2D texture, in Rectangle region, string name = "")
        : this(texture, region.X, region.Y, region.Width, region.Height, name) { }

    /// <summary>
    /// Constructs a new region representing the given rectangle.
    /// </summary>
    public TextureRegion2D(Texture2D texture, int x, int y, int width, int height, string name = "")
    {
        ArgumentNullException.ThrowIfNull(texture);
        ObjectDisposedException.ThrowIf(texture.IsDisposed, texture);

        Texture = texture;
        X = x;
        Y = y;
        Width = width;
        Height = height;
        Bounds = new Rectangle(x, y, width, height);

        LeftUV = Bounds.Left / (float)texture.Width;
        TopUV = Bounds.Top / (float)texture.Height;
        RightUV = Bounds.Right / (float)texture.Width;
        BottomUV = Bounds.Bottom / (float)texture.Height;

        if (string.IsNullOrEmpty(name))
            Name = $"{texture.Name}.Region";
        else
            Name = name;
    }

    public static implicit operator TextureRegion2D(Texture2D texture)
    {
        if (texture == null)
            return null;
        return new TextureRegion2D(texture);
    }
    public override string ToString()
    {
        if (!string.IsNullOrEmpty(Name))
        {
            return Name;
        }

        return base.ToString();
    }
}