//Heavily based off of Apos.Camera: https://github.com/Apostolique/Apos.Camera

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Rubedo.Graphics.Viewports;

/// <summary>
/// A virtual window, making up the whole or part of the actual program window.
/// </summary>
public interface IVirtualViewport : IDisposable
{
    /// <summary>
    /// X coordinate in screen-space.
    /// </summary>
    int X { get; }
    /// <summary>
    /// Y coordinate in screen-space.
    /// </summary>
    int Y { get; }
    /// <summary>
    /// Actual width of this viewport.
    /// </summary>
    int Width { get; }
    /// <summary>
    /// Actual height of this viewport.
    /// </summary>
    int Height { get; }

    Vector2 Size => new Vector2(Width, Height);

    /// <summary>
    /// The position vector.
    /// </summary>
    Vector2 XY { get; }

    /// <summary>
    /// Origin point of this viewport.
    /// </summary>
    Vector2 Origin { get; }

    /// <summary>
    /// Virtual width of this viewport, used for scaling.
    /// </summary>
    float VirtualWidth { get; }
    /// <summary>
    /// Virtual height of this viewport, used for scaling.
    /// </summary>
    float VirtualHeight { get; }


    /// <summary>
    /// Event called whenever this viewport updates its size.
    /// </summary>
    event Action<IVirtualViewport> SizeChanged;

    /// <summary>
    /// Transforms the view matrix into screen-space.
    /// </summary>
    /// <param name="view"></param>
    /// <returns></returns>
    Matrix Transform(Matrix view);
    /// <summary>
    /// Set this viewport to be rendered to.
    /// </summary>
    void Set();
    /// <summary>
    /// Unset this viewport, and set the one it had unset in <see cref="Set"/>.
    /// </summary>
    void Reset();
}