using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rubedo.Object;
using System;

namespace Rubedo.Render;

/// <summary>
/// I am SpriteBatchWrapper, and this is my summary.
/// </summary>
public sealed class Renderer : IDisposable
{
    private bool isDisposed;
    private Game game;
    public SpriteBatch Sprites { get; }
    private BasicEffect effect;

    public Renderer(Game game)
    {
        if (game is null)
        {
            throw new ArgumentNullException(nameof(game));
        }
        this.game = game;
        isDisposed = false;
        Sprites = new SpriteBatch(game.GraphicsDevice);
        effect = new BasicEffect(game.GraphicsDevice);
        effect.FogEnabled = false;
        effect.TextureEnabled = true;
        effect.VertexColorEnabled = true;
        effect.World = Matrix.Identity;
        effect.Projection = Matrix.Identity;
        effect.View = Matrix.Identity;
    }

    public void Dispose()
    {
        if (isDisposed)
            return;
        effect?.Dispose();
        Sprites?.Dispose();
        isDisposed = true;
    }

    public void Begin(Camera camera, SamplerState sampler)
    {
        if (camera is null)
        {
            effect.View = Matrix.Identity;
            effect.Projection = Matrix.CreateOrthographicOffCenter(0, game.GraphicsDevice.Viewport.Width, 0, game.GraphicsDevice.Viewport.Height, 0, 1.0f);
            effect.World = Matrix.Identity;
        } else {
            effect.View = camera.View;
            effect.Projection = camera.Projection;
            effect.World = camera.TransformMatrix;
        }

        Sprites.Begin(blendState: BlendState.AlphaBlend, samplerState: sampler, rasterizerState: RasterizerState.CullNone, effect: effect);
    }
    public void End()
    {
        Sprites.End();
    }

    public void Draw(Texture2D texture, Vector2 position, Vector2 origin, Color color)
    {
        Sprites.Draw(texture, position, null, color, 0, origin, 1, SpriteEffects.FlipVertically, 0);
    }
    public void Draw(Texture2D texture, Transform transform, Color color)
    {
        Sprites.Draw(texture, transform.Position, null, color, transform.RotationDegrees, Vector2.Zero, transform.Scale, SpriteEffects.FlipVertically, 0);
    }

    public void Draw(Texture2D texture, Transform transform, Rectangle? sourceRectangle, Color color, Vector2 origin, SpriteEffects effects, float layerDepth)
    {
        //because we're rendering with +Y coordinates, all sprites are flipped,
        //so we need to invert any SpriteEffects' FlipVertically flags.
        if (effects.HasFlag(SpriteEffects.FlipVertically))
            effects &= ~SpriteEffects.FlipVertically;
        else
            effects |= SpriteEffects.FlipVertically;

        Sprites.Draw(texture, transform.Position, sourceRectangle, color, transform.RotationDegrees, origin, transform.Scale, effects, layerDepth);
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
    public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, float scale, SpriteEffects effects)
    {
        //because we're rendering with +Y coordinates, all sprites are flipped,
        //so we need to invert any SpriteEffects' FlipVertically flags.
        if (effects.HasFlag(SpriteEffects.FlipVertically))
            effects &= ~SpriteEffects.FlipVertically;
        else
            effects |= SpriteEffects.FlipVertically;

        Sprites.DrawString(
            spriteFont,
            text,
            position,
            color,
            rotation,
            Vector2.Zero,
            scale,
            effects, 0);
    }
}