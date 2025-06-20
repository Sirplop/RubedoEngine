//Heavily based off of Apos.Camera: https://github.com/Apostolique/Apos.Camera

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace Rubedo.Graphics.Viewports;

/// <summary>
/// A viewport that preserves its given aspect ratio. Will create pillar/letterboxing.
/// </summary>
public class BestFitViewport : IVirtualViewport
{
    private bool _disposed = false;

    private GraphicsDevice _graphicsDevice;
    private GameWindow _window;

    private float _virtualWidth;
    private float _virtualHeight;

    /// <summary> The window to render. </summary>
    private Viewport _viewport;
    /// <summary> The viewport this overrode when it was set. </summary>
    private Viewport _oldViewport;

    private Vector2 _origin;

    private float _targetRatio;
    public float TargetWidth { get; set; }
    public float TargetHeight { get; set; }

    private bool _isSet;

    public event Action<IVirtualViewport> SizeChanged;

    public int X => _viewport.X;
    public int Y => _viewport.Y;
    public int Width => _viewport.Width;
    public int Height => _viewport.Height;

    public float VirtualWidth => _virtualWidth;
    public float VirtualHeight => _virtualHeight;

    public Vector2 XY => new Vector2(X, Y);
    public Vector2 Origin => _origin;

    public BestFitViewport(GraphicsDevice graphicsDevice, GameWindow window, float targetWidth, float targetHeight)
    {
        _graphicsDevice = graphicsDevice;
        _window = window;

        TargetWidth = targetWidth;
        TargetHeight = targetHeight;
        _targetRatio = targetWidth / targetHeight;

        _isSet = false;

        _window.ClientSizeChanged += OnClientSizeChanged;
        OnClientSizeChanged(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        if (_isSet)
            Reset();
        _window.ClientSizeChanged -= OnClientSizeChanged;
        GC.SuppressFinalize(this);
    }

    public Matrix GetScaleMatrix()
    {
        return Matrix.CreateScale(_targetRatio, _targetRatio, 1f);
    }

    public void Set()
    {
        if (_isSet)
            throw new Exception("Trying to set an already set screen: " + nameof(BestFitViewport));
        ObjectDisposedException.ThrowIf(_disposed, this);
        _isSet = true;
        _oldViewport = _graphicsDevice.Viewport;
        _graphicsDevice.Viewport = _viewport;
    }
    public void Reset()
    {
        if (_isSet)
        {
            _isSet = false;
            _graphicsDevice.Viewport = _oldViewport;
        }
    }

    public Matrix Transform(Matrix view)
    {
        return view * GetScaleMatrix();
    }

    private void OnClientSizeChanged(object sender, EventArgs e)
    {
        float backBufferWidth = _graphicsDevice.PresentationParameters.BackBufferWidth;
        float backBufferHeight = _graphicsDevice.PresentationParameters.BackBufferHeight;

        float ratioWidth = backBufferWidth / TargetWidth;
        float ratioHeight = backBufferHeight / TargetHeight;

        float rx = 0;
        float ry = 0;
        float rw = backBufferWidth;
        float rh = backBufferHeight;

        _virtualWidth = TargetWidth;
        _virtualHeight = TargetHeight;

        if (ratioWidth < ratioHeight) //the width is smaller, scale based on width.
        {
            _targetRatio = ratioWidth;
            rh = TargetHeight * ratioWidth;
            ry = (backBufferHeight - rh) / 2f;
        }
        else //the height is smaller, scale width based on height.
        {
            _targetRatio = ratioHeight;
            rw = TargetWidth * ratioHeight;
            rx = (backBufferWidth - rw) / 2f;
        }

        _viewport = new Viewport((int)rx, (int)ry, (int)rw, (int)rh);

        _origin = new Vector2(_virtualWidth / 2f, _virtualHeight / 2f);
    }
}