using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Rubedo.Rendering;

/// <summary>
/// I am Screen, and this is my summary.
/// </summary>
public sealed class Screen : IDisposable
{
    private const int MIN_DIM = 64;

    private bool _isDisposed;
    private RenderTarget2D _target;
    private RubedoEngine _game;
    private bool _isSet;

    public int Width 
    { 
        get
        {
            return _target.Width;
        } 
    }
    public int Height
    {
        get
        {
            return _target.Height;
        }
    }
    public int LetterboxWidth => RubedoEngine.Graphics.PreferredBackBufferWidth - _target.Width;
    public int LetterboxHeight => RubedoEngine.Graphics.PreferredBackBufferHeight - _target.Height;

    public Screen(RubedoEngine game, int width, int height)
    {
        width = Math.Max(width, MIN_DIM);
        height = Math.Max(height, MIN_DIM);

        _game = game ?? throw new ArgumentNullException("game");

        _target = new RenderTarget2D(game.GraphicsDevice, width, height);
        _isSet = false;
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;
        _target.Dispose();
        _isDisposed = true;
    }

    public void Set()
    {
        if (_isSet)
            throw new Exception("Trying to set render target while already set.");
        _game.GraphicsDevice.SetRenderTarget(_target);
        _isSet = true;
    }
    public void Unset()
    {
        if (!_isSet)
            throw new Exception("Trying to unset render target while already unset.");
        _game.GraphicsDevice.SetRenderTarget(null);
        _isSet = false;
    }

    public void Preset(Renderer renderer, SamplerState sampler)
    {
        if (renderer == null)
            throw new ArgumentNullException("renderer");

        renderer.Begin(null, sampler);
        renderer.Draw(_target, null, GetDestinationRect(), Color.White);
        renderer.End();
    }
    private Rectangle GetDestinationRect()
    {
        Rectangle backBuffer = _game.GraphicsDevice.PresentationParameters.Bounds;
        float aspectRatio = (float)backBuffer.Width / backBuffer.Height;
        float screenRatio = (float)Width / Height;

        float rx = 0;
        float ry = 0;
        float rw = backBuffer.Width;
        float rh = backBuffer.Height;

        if (aspectRatio > screenRatio)
        {
            rw = Height * screenRatio;
            rx = (backBuffer.Width - rw) * 0.5f;
        }
        else if (aspectRatio < screenRatio)
        {
            rh = Width / screenRatio;
            ry = (backBuffer.Height - rh) * 0.5f;
        }
        return new Rectangle((int)rx, (int)ry, (int)rw, (int)rh);
    }
}