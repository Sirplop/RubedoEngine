//Heavily based off of Apos.Camera: https://github.com/Apostolique/Apos.Camera

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace Rubedo.Graphics.Viewports;

/// <summary>
/// A Viewport that covers a percentage of the window.
/// </summary>
public class SplitViewport : IVirtualViewport
{
    private bool _disposed = false;

    private GraphicsDevice _graphicsDevice;
    private GameWindow _window;

    /// <summary> The window to render. </summary>
    private Viewport _viewport;
    /// <summary> The viewport this overrode when it was set. </summary>
    private Viewport _oldViewport;

    private Vector2 _origin;

    // percentage extents of this viewport, 0..1
    private float _left;
    private float _top;
    private float _right;
    private float _bottom;

    public event Action<IVirtualViewport> SizeChanged;

    public int X => _viewport.X;
    public int Y => _viewport.Y;
    public int Width => _viewport.Width;
    public int Height => _viewport.Height;

    public float VirtualWidth => Width;
    public float VirtualHeight => Height;

    public Vector2 XY => new Vector2(X, Y);
    public Vector2 Origin => _origin;

    public SplitViewport(GraphicsDevice graphicsDevice, GameWindow window, float left, float top, float right, float bottom)
    {
        _graphicsDevice = graphicsDevice;

        _left = left;
        _top = top;
        _right = right;
        _bottom = bottom;

        _window = window;
        _window.ClientSizeChanged += OnClientSizeChanged;
        OnClientSizeChanged(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        if (_disposed) 
            return;
        _disposed = true;
        _window.ClientSizeChanged -= OnClientSizeChanged;
        GC.SuppressFinalize(this);
    }

    public void Set()
    {
        _oldViewport = _graphicsDevice.Viewport;
        _graphicsDevice.Viewport = _viewport;
    }
    public void Reset()
    {
        _graphicsDevice.Viewport = _oldViewport;
    }

    public Matrix Transform(Matrix view)
    {
        return view;
    }

    private void OnClientSizeChanged(object sender, EventArgs e)
    {
        int gWidth = _graphicsDevice.PresentationParameters.BackBufferWidth;
        int gHeight = _graphicsDevice.PresentationParameters.BackBufferHeight;

        _viewport = new Viewport((int)(gWidth * _left), (int)(gHeight * _top), (int)(gWidth * (_right - _left)), (int)(gHeight * (_bottom - _top)));

        _origin = new Vector2(_viewport.Width / 2f, _viewport.Height / 2f);
        SizeChanged?.Invoke(this);
    }
}