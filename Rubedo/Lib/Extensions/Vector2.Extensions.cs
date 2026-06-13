using Microsoft.Xna.Framework;
using System.Text;

namespace Rubedo.Lib.Extensions;

/// <summary>
/// Extension methods for Vector2.
/// </summary>
public static class Vector2Extensions
{
    /// <summary>
    /// Returns the string representation of this vector in the form [X: format, Y: format]
    /// </summary>
    public static string ToNiceString(this Vector2 vec, string format = "0.00")
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("[X: ");
        sb.Append(vec.X.ToString(format));
        sb.Append(", Y: ");
        sb.Append(vec.Y.ToString(format));
        sb.Append(']');
        return sb.ToString();
    }
}