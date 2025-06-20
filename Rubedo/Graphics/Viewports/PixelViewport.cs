//Heavily based off of Apos.Camera: https://github.com/Apostolique/Apos.Camera

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace Rubedo.Graphics.Viewports;

/// <summary>
/// A full-window viewport with a target resolution that it will try to scale to.
/// </summary>
public class PixelViewport : IVirtualViewport
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

    private bool _isSet;

    public event Action<IVirtualViewport> SizeChanged;

    public int X => _viewport.X;
    public int Y => _viewport.Y;
    public int Width => _viewport.Width;
    public int Height => _viewport.Height;

    public float VirtualWidth => _virtualWidth;
    public float VirtualHeight => _virtualHeight;

    public float TargetWidth { get; set; }
    public float TargetHeight { get; set; }
    public float Ratio { get; private set; }

    public Vector2 XY => new Vector2(X, Y);
    public Vector2 Origin => _origin;

    public PixelViewport(GraphicsDevice graphicsDevice, GameWindow window, float targetWidth, float targetHeight)
    {
        _graphicsDevice = graphicsDevice;
        _window = window;

        TargetWidth = targetWidth;
        TargetHeight = targetHeight;

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
        return Matrix.CreateScale(Ratio, Ratio, 1f);
    }

    public void Set()
    {
        if (_isSet)
            throw new Exception("Trying to set an already set screen: " + nameof(PixelViewport));
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
        _viewport = new Viewport(0, 0, _graphicsDevice.PresentationParameters.BackBufferWidth, _graphicsDevice.PresentationParameters.BackBufferHeight);

        float ratioWidth = _viewport.Width / TargetWidth;
        float ratioHeight = _viewport.Height / TargetHeight;

        if (ratioWidth < ratioHeight)
        {
            Ratio = ratioWidth;

            _virtualWidth = _viewport.Width / ratioWidth;
            _virtualHeight = _viewport.Height / ratioWidth;
        }
        else
        {
            Ratio = ratioHeight;

            _virtualWidth = _viewport.Width / ratioHeight;
            _virtualHeight = _viewport.Height / ratioHeight;
        }

        _origin = new Vector2(_virtualWidth / 2f, _virtualHeight / 2f);
    }
}