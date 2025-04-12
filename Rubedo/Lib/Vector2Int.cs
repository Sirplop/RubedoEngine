using System;

namespace Rubedo.Lib;

/// <summary>
/// I am Vector2Int, and this is my summary.
/// </summary>
public struct Vector2Int : IEquatable<Vector2Int>
{
    public int X;
    public int Y;

    public Vector2Int(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool Equals(Vector2Int other)
    {
        return X == other.X && Y == other.Y;
    }

    public static bool operator ==(Vector2Int left, Vector2Int right)
    {
        return left.Equals(right);
    }
    public static bool operator !=(Vector2Int left, Vector2Int right)
    {
        return !left.Equals(right);
    }
    public static Vector2Int operator *(Vector2Int left, Vector2Int right)
    {
        return new Vector2Int(left.X * right.X, left.Y * right.Y);
    }
    public static Vector2Int operator /(Vector2Int left, Vector2Int right)
    {
        return new Vector2Int(left.X / right.X, left.Y / right.Y);
    }
    public static Vector2Int operator +(Vector2Int left, Vector2Int right)
    {
        return new Vector2Int(left.X + right.X, left.Y + right.Y);
    }
    public static Vector2Int operator -(Vector2Int left, Vector2Int right)
    {
        return new Vector2Int(left.X - right.X, left.Y - right.Y);
    }
}