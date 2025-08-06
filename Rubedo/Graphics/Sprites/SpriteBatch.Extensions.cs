using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rubedo.Lib.Extensions;
using System;
using System.Reflection;

namespace Rubedo.Graphics.Sprites;

/// <summary>
/// Extension methods for <see cref="SpriteBatch"/>
/// </summary>
public static class SpriteBatchExtensions
{
    /// <summary>
    /// Draws a texture to the sprite batch with optional clipping.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch.</param>
    /// <param name="texture">The texture to draw.</param>
    /// <param name="sourceRectangle">The source rectangle.</param>
    /// <param name="destinationRectangle">The destination rectangle.</param>
    /// <param name="color">The color to tint the texture.</param>
    /// <param name="clippingRectangle">An optional clipping rectangle.</param>
    public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Rectangle sourceRectangle, Rectangle destinationRectangle, Color color, Rectangle? clippingRectangle)
    {
        if (!ClipRectangles(ref sourceRectangle, ref destinationRectangle, clippingRectangle))
            return;

        if (destinationRectangle.Width > 0 && destinationRectangle.Height > 0)
        {
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color);
        }
    }

    /// <summary>
    /// Draws a texture region to the sprite batch.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch.</param>
    /// <param name="textureRegion">The texture region to draw.</param>
    /// <param name="position">The position to draw the texture region.</param>
    /// <param name="color">The color to tint the texture region.</param>
    /// <param name="clippingRectangle">An optional clipping rectangle.</param>
    public static void Draw(this SpriteBatch spriteBatch, TextureRegion2D textureRegion, Vector2 position, Color color, Rectangle? clippingRectangle = null)
    {
        spriteBatch.Draw(textureRegion, position, color, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0, clippingRectangle);
    }

    /// <summary>
    /// Draws a texture region to the sprite batch.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch.</param>
    /// <param name="textureRegion">The texture region to draw.</param>
    /// <param name="destinationRectangle">The destination rectangle.</param>
    /// <param name="color">The color to tint the texture region.</param>
    /// <param name="clippingRectangle">An optional clipping rectangle.</param>
    public static void Draw(this SpriteBatch spriteBatch, TextureRegion2D textureRegion, Rectangle destinationRectangle, Color color, Rectangle? clippingRectangle = null)
    {
        spriteBatch.Draw(textureRegion.Texture, textureRegion.Bounds, destinationRectangle, color, clippingRectangle);
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
    public static void Draw(this SpriteBatch spriteBatch, TextureRegion2D textureRegion, Vector2 position, Color color,
    float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth, Rectangle? clippingRectangle = null)
    {
        Rectangle sourceRectangle = textureRegion.Bounds;

        if (clippingRectangle.HasValue)
        {
            int x = (int)(position.X - origin.X);
            int y = (int)(position.Y - origin.Y);
            int width = (int)(textureRegion.Width * scale.X);
            int height = (int)(textureRegion.Height * scale.Y);
            Rectangle destinationRectangle = new Rectangle(x, y, width, height);

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
    #region Nine Slice
    private static readonly Rectangle[] _sliceCache = new Rectangle[9];

    /// <summary>
    /// Draws a nine slice region to the sprite batch.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch.</param>
    /// <param name="nineSlice">The nine slice.</param>
    /// <param name="destinationRectangle">The destination rectangle.</param>
    /// <param name="color">The color to tint the nine slice.</param>
    /// <param name="clippingRectangle">An optional clipping rectangle.</param>
    public static void Draw(this SpriteBatch spriteBatch, NineSlice nineSlice, Rectangle destinationRectangle, Color color, Rectangle? clippingRectangle = null)
    {
        CreateDestinationSlices(nineSlice, destinationRectangle);

        switch (nineSlice.drawMode)
        {
            default:
            case NineSlice.DrawMode.Scale:
                DrawNineSliceScale(spriteBatch, nineSlice, color, clippingRectangle);
                break;
            case NineSlice.DrawMode.Tile:
                DrawNineSliceTile(spriteBatch, nineSlice, color, clippingRectangle);
                break;
        }
    }

    private static void DrawNineSliceScale(SpriteBatch spriteBatch, NineSlice nineSlice, Color color, Rectangle? clippingRectangle = null)
    {
        ReadOnlySpan<TextureRegion2D> rawSlices = nineSlice.Slices;

        for (int i = 0; i < rawSlices.Length; i++)
        {
            if (!nineSlice.filled && i == NineSlice.CENTER)
                continue; //skip the center if we're not filled.

            Rectangle source = rawSlices[i].Bounds; //source is the unscaled texture slice
            Rectangle destination = _sliceCache[i]; //destination is the total area that slice texture must fill, using whatever drawmode is set.
                                                    
            if (clippingRectangle.HasValue)
            {
                ClipSourceRectangle(in source, in destination, clippingRectangle.Value, out source);
                ClipDestinationRectangle(in destination, clippingRectangle.Value, out destination);
                spriteBatch.Draw(rawSlices[i].Texture, source, destination, color, clippingRectangle);
            }
            else
            {
                if (destination.Width > 0 && destination.Height > 0)
                {
                    spriteBatch.Draw(rawSlices[i].Texture, destination, source, color);
                }
            }
        }
    }
    private static void DrawNineSliceTile(SpriteBatch spriteBatch, NineSlice nineSlice, Color color, Rectangle? clippingRectangle = null)
    {
        ReadOnlySpan<TextureRegion2D> rawSlices = nineSlice.Slices;

        for (int i = 0; i < rawSlices.Length; i++)
        {
            switch (i)
            {
                case NineSlice.CENTER:
                    if (!nineSlice.filled)
                        continue; //skip the center if we're not filled.
                    DrawNineSliceCenter(spriteBatch, nineSlice, i, color, clippingRectangle);
                    break;
                case NineSlice.TOP_LEFT:
                case NineSlice.TOP_RIGHT:
                case NineSlice.BOTTOM_LEFT:
                case NineSlice.BOTTOM_RIGHT:
                    Rectangle source = rawSlices[i].Bounds; //source is the unscaled texture slice
                    Rectangle destination = _sliceCache[i]; //destination is the total area that slice texture must fill.
                    if (clippingRectangle.HasValue)
                    {
                        ClipSourceRectangle(in source, in destination, clippingRectangle.Value, out source);
                        ClipDestinationRectangle(in destination, clippingRectangle.Value, out destination);
                        spriteBatch.Draw(rawSlices[i].Texture, source, destination, color, clippingRectangle);
                    }
                    else
                    {
                        if (destination.Width > 0 && destination.Height > 0)
                        {
                            spriteBatch.Draw(rawSlices[i].Texture, destination, source, color);
                        }
                    }
                    break;
                case NineSlice.CENTER_LEFT:
                case NineSlice.CENTER_RIGHT:
                    DrawNineSliceYEdge(spriteBatch, nineSlice, i, color, clippingRectangle);
                    break;
                case NineSlice.TOP:
                case NineSlice.BOTTOM:
                    DrawNineSliceXEdge(spriteBatch, nineSlice, i, color, clippingRectangle);
                    break;
            }

        }
    }


    private static void DrawNineSliceCenter(SpriteBatch spriteBatch, NineSlice nineSlice, int index, Color color, Rectangle? clippingRectangle = null)
    {
        ReadOnlySpan<TextureRegion2D> rawSlices = nineSlice.Slices;

        Rectangle source = rawSlices[index].Bounds; //source is the unscaled texture slice
        Rectangle area = _sliceCache[index];        //area is the total area that slice texture must fill
        Rectangle destination = new Rectangle();    //destination is where the source is drawn within the area

        ClipDestinationRectangle(in area, clippingRectangle.Value, out area);

        int sourceWidth = Lib.Math.FloorToInt(source.Width * nineSlice.pixelMultiplier);
        int sourceHeight = Lib.Math.FloorToInt(source.Height * nineSlice.pixelMultiplier);

        int x = area.X;
        int y = area.Y;
        int endX = x + area.Width;
        int endY = y + area.Height;
        while (x < endX)
        {
            while (y < endY)
            {
                destination.Width = source.Width;
                destination.Height = source.Height;
                destination.X = x;
                destination.Y = y;
                ClipDestinationRectangle(in destination, in area, out destination);
                ClipSourceRectangle(in source, in destination, in area, out Rectangle drawSource);
                drawSource.Width = destination.Width;
                drawSource.Height = destination.Height;

                destination.Width = Lib.Math.FloorToInt(sourceWidth * (destination.Width / (float)source.Width));
                destination.Height = Lib.Math.FloorToInt(sourceHeight * (destination.Height / (float)source.Height));

                spriteBatch.Draw(rawSlices[index].Texture, drawSource, destination, color, clippingRectangle);
                y += sourceHeight;
            }
            y = area.Y;
            x += sourceWidth;
        }
    }
    private static void DrawNineSliceXEdge(SpriteBatch spriteBatch, NineSlice nineSlice, int index, Color color, Rectangle? clippingRectangle = null)
    {
        ReadOnlySpan<TextureRegion2D> rawSlices = nineSlice.Slices;

        Rectangle source = rawSlices[index].Bounds; //source is the unscaled texture slice
        Rectangle area = _sliceCache[index];        //area is the total area that slice texture must fill
        Rectangle destination = new Rectangle();    //destination is where the source is drawn within the area

        ClipDestinationRectangle(in area, clippingRectangle.Value, out area);

        int sourceWidth = Lib.Math.FloorToInt(source.Width * nineSlice.pixelMultiplier);
        int sourceHeight = Lib.Math.FloorToInt(source.Height * nineSlice.pixelMultiplier);

        int x = area.X;
        int endX = x + area.Width;
        while (x < endX)
        {
            destination.Width = source.Width;
            destination.Height = source.Height;
            destination.X = x;
            destination.Y = area.Y;
            ClipDestinationRectangle(in destination, in area, out destination);
            ClipSourceRectangle(in source, in destination, in area, out Rectangle drawSource);
            drawSource.Width = destination.Width;
            drawSource.Height = destination.Height;

            destination.Width = Lib.Math.FloorToInt(sourceWidth * (destination.Width / (float)source.Width));
            destination.Height = Lib.Math.FloorToInt(sourceHeight * (destination.Height / (float)source.Height));

            spriteBatch.Draw(rawSlices[index].Texture, drawSource, destination, color, clippingRectangle);
            x += sourceWidth;
        }
    }
    private static void DrawNineSliceYEdge(SpriteBatch spriteBatch, NineSlice nineSlice, int index, Color color, Rectangle? clippingRectangle = null)
    {
        ReadOnlySpan<TextureRegion2D> rawSlices = nineSlice.Slices;

        Rectangle source = rawSlices[index].Bounds; //source is the unscaled texture slice
        Rectangle area = _sliceCache[index];        //area is the total area that slice texture must fill
        Rectangle destination = new Rectangle();    //destination is where the source is drawn within the area

        ClipDestinationRectangle(in area, clippingRectangle.Value, out area);

        int sourceWidth = Lib.Math.FloorToInt(source.Width * nineSlice.pixelMultiplier);
        int sourceHeight = Lib.Math.FloorToInt(source.Height * nineSlice.pixelMultiplier);

        int y = area.Y;
        int endY = y + area.Height;
        while (y < endY)
        {
            destination.Width = source.Width;
            destination.Height = source.Height;
            destination.X = area.X;
            destination.Y = y;
            ClipDestinationRectangle(in destination, in area, out destination);
            ClipSourceRectangle(in source, in destination, in area, out Rectangle drawSource);
            drawSource.Width = destination.Width;
            drawSource.Height = destination.Height;

            destination.Width = Lib.Math.FloorToInt(sourceWidth * (destination.Width / (float)source.Width));
            destination.Height = Lib.Math.FloorToInt(sourceHeight * (destination.Height / (float)source.Height));

            spriteBatch.Draw(rawSlices[index].Texture, drawSource, destination, color, clippingRectangle);
            y += sourceHeight;
        }
    }

    private static void CreateDestinationSlices(NineSlice nineSlice, Rectangle destinationRect)
    {
        destinationRect.Deconstruct(out int x, out int y, out int width, out int height);
        nineSlice.Padding.Deconstruct(out int leftPadding, out int topPadding, out int rightPadding, out int bottomPadding);

        leftPadding = Lib.Math.FloorToInt(leftPadding * nineSlice.pixelMultiplier);
        topPadding = Lib.Math.FloorToInt(topPadding * nineSlice.pixelMultiplier);
        rightPadding = Lib.Math.FloorToInt(rightPadding * nineSlice.pixelMultiplier);
        bottomPadding = Lib.Math.FloorToInt(bottomPadding * nineSlice.pixelMultiplier);

        int top = y + topPadding;
        int right = x + width - rightPadding;
        int bottom = y + height - bottomPadding;
        int left = x + leftPadding;
        int midWidth = width - leftPadding - rightPadding;
        int midHeight = height - topPadding - bottomPadding;

        //corners are always the same, no matter the draw mode.
        _sliceCache[NineSlice.TOP_LEFT] = new Rectangle(x, y, leftPadding, topPadding);
        _sliceCache[NineSlice.TOP_RIGHT] = new Rectangle(right, y, rightPadding, topPadding);
        _sliceCache[NineSlice.BOTTOM_LEFT] = new Rectangle(x, bottom, leftPadding, bottomPadding);
        _sliceCache[NineSlice.BOTTOM_RIGHT] = new Rectangle(right, bottom, rightPadding, bottomPadding);

        //X-scaling edges
        _sliceCache[NineSlice.TOP] = new Rectangle(left, y, midWidth, topPadding);
        _sliceCache[NineSlice.BOTTOM] = new Rectangle(left, bottom, midWidth, bottomPadding);
        //Y-scaling edges
        _sliceCache[NineSlice.CENTER_LEFT] = new Rectangle(x, top, leftPadding, midHeight);
        _sliceCache[NineSlice.CENTER_RIGHT] = new Rectangle(right, top, rightPadding, midHeight);
        //the center
        _sliceCache[NineSlice.CENTER] = new Rectangle(left, top, midWidth, midHeight);
    }
    #endregion
    #region Tileable
    public static void DrawTiled(this SpriteBatch spriteBatch, TextureRegion2D textureRegion, Rectangle destinationRectangle, Color color, Rectangle? clippingRectangle = null, float uvOffsetX = 0, float uvOffsetY = 0)
    {
        Rectangle source = textureRegion.Bounds;    //source is the unscaled texture region
        Rectangle subDestination = new Rectangle();    //subDestination is where the source is drawn within the destinationRectangle

        ClipDestinationRectangle(in destinationRectangle, clippingRectangle.Value, out destinationRectangle);

        if (destinationRectangle.Width > 0 && destinationRectangle.Height > 0)
        {
            if (uvOffsetX < 0)
                uvOffsetX = 1 + (uvOffsetX - MathF.Ceiling(uvOffsetX));
            else
                uvOffsetX = uvOffsetX - MathF.Floor(uvOffsetX);

            if (uvOffsetY < 0)
                uvOffsetY = 1 + (uvOffsetY - MathF.Ceiling(uvOffsetY));
            else
                uvOffsetY = uvOffsetY - MathF.Floor(uvOffsetY);

            int endX = destinationRectangle.X + destinationRectangle.Width;
            int endY = destinationRectangle.Y + destinationRectangle.Height;
            int startY = destinationRectangle.Y - Lib.Math.FloorToInt(uvOffsetY * source.Height);

            for (int x = destinationRectangle.X - Lib.Math.FloorToInt(uvOffsetX * source.Width); x < endX; x += source.Width)
            {
                for (int y = startY; y < endY; y += source.Height)
                {
                    subDestination.Width = source.Width;
                    subDestination.Height = source.Height;
                    subDestination.X = x;
                    subDestination.Y = y;
                    spriteBatch.Draw(textureRegion.Texture, source, subDestination, color, clippingRectangle);
                }
            }
        }
    }
    #endregion
    #region Utility Functions

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

    private static void ClipSourceRectangle(in Rectangle sourceRectangle, in Rectangle destinationRectangle, in Rectangle clippingRectangle, out Rectangle result)
    {
        int left = clippingRectangle.Left - destinationRectangle.Left;
        int right = destinationRectangle.Right - clippingRectangle.Right;
        int top = clippingRectangle.Top - destinationRectangle.Top;
        int bottom = destinationRectangle.Bottom - clippingRectangle.Bottom;
        int x = left > 0 ? left : 0;
        int y = top > 0 ? top : 0;
        int w = (right > 0 ? right : 0) + x;
        int h = (bottom > 0 ? bottom : 0) + y;

        float scaleX = (float)destinationRectangle.Width / sourceRectangle.Width;
        float scaleY = (float)destinationRectangle.Height / sourceRectangle.Height;
        x = (int)(x / scaleX);
        y = (int)(y / scaleY);
        w = (int)(w / scaleX);
        h = (int)(h / scaleY);

        result.X = sourceRectangle.X + x;
        result.Y = sourceRectangle.Y + y;
        result.Width = sourceRectangle.Width - w;
        result.Height = sourceRectangle.Height - h;
    }

    private static void ClipDestinationRectangle(in Rectangle destinationRectangle, in Rectangle clippingRectangle, out Rectangle result)
    {
        var left = clippingRectangle.Left < destinationRectangle.Left ? destinationRectangle.Left : clippingRectangle.Left;
        var top = clippingRectangle.Top < destinationRectangle.Top ? destinationRectangle.Top : clippingRectangle.Top;
        var bottom = clippingRectangle.Bottom < destinationRectangle.Bottom ? clippingRectangle.Bottom : destinationRectangle.Bottom;
        var right = clippingRectangle.Right < destinationRectangle.Right ? clippingRectangle.Right : destinationRectangle.Right;
        result = new Rectangle(left, top, right - left, bottom - top);
    }
    #endregion
}