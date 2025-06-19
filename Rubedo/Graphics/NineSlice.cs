using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rubedo.Lib;
using Rubedo.UI.Input;
using System;

namespace Rubedo.Graphics;

/// <summary>
/// A specialized Texture2DRegion that has been sliced into 9 sections.
/// </summary>
/// <remarks>The corners will not scale, the edges will stretch along their axis, and the center will stretch in both axis.</remarks>
public class NineSlice
{
    public enum DrawMode
    {
        Scale,
        Tile
    }

    public const int TOP_LEFT = 0;
    public const int TOP = 1;
    public const int TOP_RIGHT = 2;
    public const int CENTER_LEFT = 3;
    public const int CENTER = 4;
    public const int CENTER_RIGHT = 5;
    public const int BOTTOM_LEFT = 6;
    public const int BOTTOM = 7;
    public const int BOTTOM_RIGHT = 8;

    public DrawMode drawMode = DrawMode.Scale;
    /// <summary>
    /// Draw the center of the nine slice or not.
    /// </summary>
    public bool filled = true;
    /// <summary>
    /// Amount the edge thickness should be multiplied by.
    /// </summary>
    public float pixelMultiplier = 1;

    /// <summary>
    /// The name of this resource.
    /// </summary>
    public string Name { get; }

    private readonly Texture2DRegion[] _slices;
    public ReadOnlySpan<Texture2DRegion> Slices => _slices;

    /// <summary>
    /// Size information of the patches around the center patch.    
    /// </summary>
    public Padding Padding { get; }

    /// <summary>
    /// Constructs a new NineSlice from the given array of <see cref="Texture2DRegion"/>s.
    /// </summary>
    /// <param name="slices"></param>
    public NineSlice(Texture2DRegion[] slices, string name = "")
    {
        ArgumentNullException.ThrowIfNull(slices);
        if (slices.Length != 9)
        {
            throw new ArgumentException($"{nameof(slices)} must contain exactly 9 elements.", nameof(slices));
        }

        _slices = slices;

        Point topLeft = slices[TOP_LEFT].Size;
        Point bottomRight = slices[BOTTOM_RIGHT].Size;
        Padding = new Padding(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);

        if (string.IsNullOrEmpty(name))
        {
            Name = $"{slices[0].Name}.NineSlice";
        }
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