using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rubedo.Lib;
using System;

namespace Rubedo.Graphics;

/// <summary>
/// A region of a <see cref="Texture2D"/>.
/// </summary>
public class Texture2DRegion
{
    public Texture2D Texture { get; }

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

    public Vector2Int Size => new Vector2Int(Width, Height);

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
    public Texture2DRegion(Texture2D texture)
        : this(texture, 0, 0, texture.Width, texture.Height) { }

    /// <summary>
    /// Constructs a new region representing the given rectangle.
    /// </summary>
    public Texture2DRegion(Texture2D texture, in Rectangle region)
        : this(texture, region.X, region.Y, region.Width, region.Height) { }

    /// <summary>
    /// Constructs a new region representing the given rectangle.
    /// </summary>
    public Texture2DRegion(Texture2D texture, int x, int y, int width, int height)
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
    }

    public static implicit operator Texture2DRegion(Texture2D texture)
    {
        return new Texture2DRegion(texture);
    }
}