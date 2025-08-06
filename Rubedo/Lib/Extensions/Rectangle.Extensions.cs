using Microsoft.Xna.Framework;

namespace Rubedo.Lib.Extensions;

/// <summary>
/// Extension methods for <see cref="Rectangle"/>.
/// </summary>
public static class RectangleExtensions
{
    /// <summary>
    /// Gets the ratio of width / height.
    /// </summary>
    /// <param name="rectangle"></param>
    /// <returns></returns>
    public static float AspectRatio(this Rectangle rectangle)
    {
        return rectangle.Width / (float)rectangle.Height;
    }

    /// <summary>
    /// Clips the specified rectangle against the specified clipping rectangle.
    /// </summary>
    /// <param name="rectangle">The rectangle to clip.</param>
    /// <param name="clippingRectangle">The rectangle to clip against.</param>
    /// <returns>The clipped rectangle, or <see cref="Rectangle.Empty"/> if the rectangles do not intersect.</returns>
    public static Rectangle Clip(this Rectangle rectangle, Rectangle clippingRectangle)
    {
        var clip = clippingRectangle;
        rectangle.X = clip.X > rectangle.X ? clip.X : rectangle.X;
        rectangle.Y = clip.Y > rectangle.Y ? clip.Y : rectangle.Y;
        rectangle.Width = rectangle.Right > clip.Right ? clip.Right - rectangle.X : rectangle.Width;
        rectangle.Height = rectangle.Bottom > clip.Bottom ? clip.Bottom - rectangle.Y : rectangle.Height;

        if (rectangle.Width <= 0 || rectangle.Height <= 0)
            return Rectangle.Empty;

        return rectangle;
    }
    public static void Clip(ref Rectangle rectangle, ref Rectangle clippingRectangle, out Rectangle result)
    {
        var clip = clippingRectangle;
        result.X = clip.X > rectangle.X ? clip.X : rectangle.X;
        result.Y = clip.Y > rectangle.Y ? clip.Y : rectangle.Y;
        result.Width = rectangle.Right > clip.Right ? clip.Right - rectangle.X : rectangle.Width;
        result.Height = rectangle.Bottom > clip.Bottom ? clip.Bottom - rectangle.Y : rectangle.Height;

        if (result.Width <= 0 || result.Height <= 0)
            result = Rectangle.Empty;
    }

    /// <summary>
    /// Gets a rectangle that is relative to the specified source rectangle, with the specified offsets and dimensions.
    /// </summary>
    /// <param name="source">The source rectangle.</param>
    /// <param name="x">The x-coordinate of the relative rectangle, relative to the source rectangle.</param>
    /// <param name="y">The y-coordinate of the relative rectangle, relative to the source rectangle.</param>
    /// <param name="width">The width, in pixels, of the relative rectangle.</param>
    /// <param name="height">The height, in pixels, of the relative rectangle.</param>
    public static Rectangle GetRelativeRectangle(this Rectangle source, int x, int y, int width, int height)
    {
        int absoluteX = source.X + x;
        int absoluteY = source.Y + y;

        Rectangle relative;
        relative.X = Math.Clamp(absoluteX, source.Left, source.Right);
        relative.Y = Math.Clamp(absoluteY, source.Top, source.Bottom);
        relative.Width = System.Math.Max(System.Math.Min(absoluteX + width, source.Right) - relative.X, 0);
        relative.Height = System.Math.Max(System.Math.Min(absoluteY + height, source.Bottom) - relative.Y, 0);

        return relative;
    }
}