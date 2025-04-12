using Microsoft.Xna.Framework;

namespace Rubedo.Lib;

/// <summary>
/// Vector math library.
/// </summary>
public static class MathV
{
    /// <summary>
    /// Adds (<paramref name="b"/> * <paramref name="c"/>) to <paramref name="a"/>, and returns the result in <paramref name="d"/>.
    /// </summary>
    public static void MulAdd(ref Vector2 a, ref Vector2 b, float c, out Vector2 d)
    {
        d.X = a.X + (b.X * c);
        d.Y = a.Y + (b.Y * c);
    }
    /// <summary>
    /// Subtracts (<paramref name="b"/> * <paramref name="c"/>) from <paramref name="a"/>, and returns the result in <paramref name="d"/>.
    /// </summary>
    public static void MulSub(ref Vector2 a, ref Vector2 b, float c, out Vector2 d)
    {
        d.X = a.X - (b.X * c);
        d.Y = a.Y - (b.Y * c);
    }
    /// <summary>
    /// (<paramref name="a"/> * <paramref name="aScale"/>) + (<paramref name="b"/> * <paramref name="bScale"/>).
    /// </summary>
    /// <param name="a">The vector a.</param>
    /// <param name="aScale">The value <paramref name="a"/> is multiplied by.</param>
    /// <param name="b">The vector b.</param>
    /// <param name="bScale">The value <paramref name="b"/> is multiplied by.</param>
    public static void MulAdd2(ref Vector2 a, float aScale, ref Vector2 b, float bScale, out Vector2 c)
    {
        c.X = (a.X * aScale) + (b.X * bScale);
        c.Y = (a.Y * aScale) + (b.Y * bScale);
    }
    /// <summary>
    /// (<paramref name="a"/> * <paramref name="aScale"/>) - (<paramref name="b"/> * <paramref name="bScale"/>).
    /// </summary>
    /// <param name="a">The vector a.</param>
    /// <param name="aScale">The value <paramref name="a"/> is multiplied by.</param>
    /// <param name="b">The vector b.</param>
    /// <param name="bScale">The value <paramref name="b"/> is multiplied by.</param>
    public static void MulSub2(ref Vector2 a, float aScale, ref Vector2 b, float bScale, out Vector2 c)
    {
        c.X = (a.X * aScale) + (b.X * bScale);
        c.Y = (a.Y * aScale) + (b.Y * bScale);
    }
}