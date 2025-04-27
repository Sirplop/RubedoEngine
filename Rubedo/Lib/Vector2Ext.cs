using Microsoft.Xna.Framework;
using System.Text;

namespace Rubedo.Lib;

/// <summary>
/// Extension methods for Vector2.
/// </summary>
public static class Vector2Ext
{
    /// <summary>
    /// Returns the string representation of this vector in the form [X: 0.00, Y: 0.00]
    /// </summary>
    public static string ToNiceString(this Vector2 vec)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("[X: ");
        sb.Append(vec.X.ToString("0.00"));
        sb.Append(", Y: ");
        sb.Append(vec.Y.ToString("0.00"));
        sb.Append(']');
        return sb.ToString();
    }
}