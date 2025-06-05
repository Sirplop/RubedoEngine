using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rubedo.Graphics;
using Rubedo.Object;
using System;
using System.Collections.Generic;

namespace Rubedo.Rendering;

/// <summary>
/// Game renderer, wrapping <see cref="SpriteBatch"/>, using <see cref="NeoCamera"/>s to render onto the screen.
/// </summary>
public class Renderer : IDisposable
{
    public enum Space
    {
        World,
        Screen
    }

    private bool _isDisposed;
    private Game _game;
    private BasicEffect _effect;
    private List<NeoCamera> _cameras;

    public SpriteBatch Sprites { get; }

    public Renderer(Game game)
    {
        ArgumentNullException.ThrowIfNull(game);
        this._game = game;
        _cameras = new List<NeoCamera>();
        _isDisposed = false;
        Sprites = new SpriteBatch(game.GraphicsDevice);
        _effect = new BasicEffect(game.GraphicsDevice);
        _effect.FogEnabled = false;
        _effect.TextureEnabled = true;
        _effect.VertexColorEnabled = true;
        _effect.World = Matrix.Identity;
        _effect.Projection = Matrix.Identity;
        _effect.View = Matrix.Identity;
    }

    public void AddCamera(NeoCamera camera)
    {
        if (_cameras.Count == 0)
        {
            _cameras.Add(camera);
            return;
        }
        for (int i = 0; i < _cameras.Count; i++)
        {
            if (_cameras[i].Order >= camera.Order)
            {
                _cameras.Insert(i, camera);
                return;
            }
        }
        _cameras.Add(camera); //larger than all other camera orders
    }
    public void RemoveCamera(NeoCamera camera)
    {
        for (int i = 0; i < _cameras.Count; i++)
        {
            if (_cameras[i] == camera)
            {
                _cameras.RemoveAt(i);
                return;
            }
        }
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;
        _effect?.Dispose();
        Sprites?.Dispose();
        _isDisposed = true;
    }

    public void Begin(NeoCamera camera, SamplerState sampler)
    {
        ArgumentNullException.ThrowIfNull(camera);

        _effect.View = camera.View;
        _effect.Projection = camera.GetProjection();
        _effect.World = Matrix.Identity;

        Sprites.Begin(sortMode: SpriteSortMode.FrontToBack, blendState: BlendState.AlphaBlend, samplerState: sampler, rasterizerState: RasterizerState.CullNone, effect: _effect);
    }
    public void End()
    {
        Sprites.End();
    }

    public void Draw(Texture2DRegion texture, Vector2 position, Vector2 origin, Color color)
    {
        Sprites.Draw(texture, position, color, 0, origin, Vector2.One, SpriteEffects.FlipVertically, 0);
    }
    public void Draw(Texture2DRegion texture, Transform transform, Color color)
    {
        Sprites.Draw(texture, transform.Position, color, 0, Vector2.Zero, Vector2.One, SpriteEffects.FlipVertically, 0);
    }
    public void Draw(Texture2DRegion texture, Transform transform, Rectangle? sourceRectangle, Color color, Vector2 origin, SpriteEffects effects, float layerDepth)
    {
        //because we're rendering with +Y coordinates, all sprites are flipped,
        //so we need to invert any SpriteEffects' FlipVertically flags.
        if (effects.HasFlag(SpriteEffects.FlipVertically))
            effects &= ~SpriteEffects.FlipVertically;
        else
            effects |= SpriteEffects.FlipVertically;

        Sprites.Draw(texture, transform.Position, color, transform.Rotation, origin, transform.Scale, effects, layerDepth, sourceRectangle);
    }

    public void Draw(Texture2D texture, Vector2 position, Vector2 origin, Color color)
    {
        Sprites.Draw(texture, position, null, color, 0, origin, 1, SpriteEffects.FlipVertically, 0);
    }
    public void Draw(Texture2D texture, Transform transform, Color color)
    {
        Sprites.Draw(texture, transform.Position, null, color, transform.Rotation, Vector2.Zero, transform.Scale, SpriteEffects.FlipVertically, 0);
    }

    public void Draw(Texture2D texture, Transform transform, Rectangle? sourceRectangle, Color color, Vector2 origin, SpriteEffects effects, float layerDepth)
    {
        //because we're rendering with +Y coordinates, all sprites are flipped,
        //so we need to invert any SpriteEffects' FlipVertically flags.
        if (effects.HasFlag(SpriteEffects.FlipVertically))
            effects &= ~SpriteEffects.FlipVertically;
        else
            effects |= SpriteEffects.FlipVertically;

        Sprites.Draw(texture, transform.Position, sourceRectangle, color, transform.Rotation, origin, transform.Scale, effects, layerDepth);
    }

    public void Draw(Texture2D texture, Rectangle? sourceRectangle, Rectangle destinationRectangle, Color color)
    {
        Sprites.Draw(texture, destinationRectangle, sourceRectangle, color, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0);
    }

    public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
    {
        //because we're rendering with +Y coordinates, all sprites are flipped,
        //so we need to invert any SpriteEffects' FlipVertically flags.
        if (effects.HasFlag(SpriteEffects.FlipVertically))
            effects &= ~SpriteEffects.FlipVertically;
        else
            effects |= SpriteEffects.FlipVertically;

        Sprites.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
    }
    public void DrawString(SpriteFontBase spriteFont, Space space, string text, Vector2 position, Color color, float rotation, float scale, int layerDepth = 0,
        SpriteEffects effects = SpriteEffects.None, TextStyle style = TextStyle.None, FontSystemEffect fontEffect = FontSystemEffect.None, int effectAmount = 0)
    {
        Vector2 scaleVec = new Vector2(
            effects.HasFlag(SpriteEffects.FlipHorizontally) ? -scale : scale,
            effects.HasFlag(SpriteEffects.FlipVertically) ? scale : -scale
        );

        Sprites.DrawString(
            spriteFont,
            text,
            position,
            color,
            rotation,
            Vector2.Zero,
            scaleVec,
            layerDepth,
            textStyle: style,
            effect: fontEffect,
            effectAmount: effectAmount);
    }
}