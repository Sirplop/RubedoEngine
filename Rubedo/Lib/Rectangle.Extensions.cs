using Microsoft.Xna.Framework;

namespace Rubedo.Lib;

/// <summary>
/// Extension methods for <see cref="Rectangle"/>
/// </summary>
public static class RectangleExtensions
{
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
}