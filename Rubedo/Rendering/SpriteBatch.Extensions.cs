using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rubedo.Graphics;
using Rubedo.Lib;

namespace Rubedo.Rendering;

/// <summary>
/// Extension methods for <see cref="SpriteBatch"/>
/// </summary>
public static class SpriteBatchExtensions
{
    /// <summary>
    /// Draws a texture region to the sprite batch.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch.</param>
    /// <param name="textureRegion">The texture region to draw.</param>
    /// <param name="position">The position to draw the texture region.</param>
    /// <param name="color">The color to tint the texture region.</param>
    /// <param name="clippingRectangle">An optional clipping rectangle.</param>
    public static void Draw(this SpriteBatch spriteBatch, Texture2DRegion textureRegion, Vector2 position, Color color, Rectangle? clippingRectangle = null)
    {
        Draw(spriteBatch, textureRegion, position, color, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0, clippingRectangle);
    }

    /// <summary>
    /// Draws a texture region to the sprite batch with specified parameters.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch.</param>
    /// <param name="textureRegion">The texture region to draw.</param>
    /// <param name="position">The position to draw the texture region.</param>
    /// <param name="color">The color to tint the texture region.</param>
    /// <param name="rotation">The rotation of the texture region.</param>
    /// <param name="origin">The origin of the texture region.</param>
    /// <param name="scale">The scale of the texture region.</param>
    /// <param name="effects">The sprite effects to apply.</param>
    /// <param name="layerDepth">The layer depth.</param>
    /// <param name="clippingRectangle">An optional clipping rectangle.</param>
    public static void Draw(this SpriteBatch spriteBatch, Texture2DRegion textureRegion, Vector2 position, Color color,
    float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth, Rectangle? clippingRectangle = null)
    {
        var sourceRectangle = textureRegion.Bounds;

        if (clippingRectangle.HasValue)
        {
            var x = (int)(position.X - origin.X);
            var y = (int)(position.Y - origin.Y);
            var width = (int)(textureRegion.Width * scale.X);
            var height = (int)(textureRegion.Height * scale.Y);
            var destinationRectangle = new Rectangle(x, y, width, height);

            if (!ClipRectangles(ref sourceRectangle, ref destinationRectangle, clippingRectangle))
            {
                // Clipped rectangle is empty, nothing to draw
                return;
            }

            position.X = destinationRectangle.X + origin.X;
            position.Y = destinationRectangle.Y + origin.Y;
        }

        spriteBatch.Draw(textureRegion.Texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
    }

    private static bool ClipRectangles(ref Rectangle sourceRectangle, ref Rectangle destinationRectangle, Rectangle? clippingRectangle)
    {
        if (!clippingRectangle.HasValue)
            return true;

        var originalDestination = destinationRectangle;
        destinationRectangle = destinationRectangle.Clip(clippingRectangle.Value);

        if (destinationRectangle == Rectangle.Empty)
            return false; // Clipped rectangle is empty, nothing to draw

        var scaleX = (float)sourceRectangle.Width / originalDestination.Width;
        var scaleY = (float)sourceRectangle.Height / originalDestination.Height;

        int leftDiff = destinationRectangle.Left - originalDestination.Left;
        int topDiff = destinationRectangle.Top - originalDestination.Top;

        sourceRectangle.X += (int)(leftDiff * scaleX);
        sourceRectangle.Y += (int)(topDiff * scaleY);
        sourceRectangle.Width = (int)(destinationRectangle.Width * scaleX);
        sourceRectangle.Height = (int)(destinationRectangle.Height * scaleY);

        return true;
    }
}