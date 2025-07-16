using Microsoft.Xna.Framework;
namespace Rubedo.Lib;

/// <summary>
/// Like <seealso cref="AABB"/>, this represents an axis-aligned bounding box, but this one is constructed from the top-left corner.
/// </summary>
public struct RectF
{
    public float x; public float y;
    public float width; public float height;

    public readonly Vector2 Center => new Vector2(x + width * 0.5f, y + height * 0.5f);

    public readonly Vector2 TopLeft => new Vector2(x, y);

    public readonly Vector2 TopRight => new Vector2(x + width, y);

    public readonly Vector2 BottomLeft => new Vector2(x, y + height);

    public readonly Vector2 BottomRight => new Vector2(x + width, y + height);


    public RectF(float x, float y, float width, float height)
    {
        this.x = x;  this.y = y;
        this.width = width;  this.height = height;
    }
    public RectF(Vector2 XY, float width, float height)
    {
        this.x = XY.X; this.y = XY.Y;
        this.width = width; this.height = height;
    }
    public RectF(float x, float y, Vector2 size)
    {
        this.x = x; this.y = y;
        this.width = size.X; this.height = size.Y;
    }
    public RectF(Vector2 XY, Vector2 size)
    {
        this.x = XY.X; this.y = XY.Y;
        this.width = size.X; this.height = size.Y;
    }

    public static implicit operator RectF(Rectangle rectangle)
    {
        return new RectF(rectangle.Left, rectangle.Top, rectangle.Size.X, rectangle.Size.Y);
    }

    public static RectF FromCorners(Vector2 min, Vector2 max)
    {
        return new RectF(min.X, min.Y, max.X - min.X, max.Y - min.Y);
    }

    public RectF Intersection(RectF second)
    {
        Intersection(ref this, ref second, out RectF rect);
        return rect;
    }

    public static void Intersection(ref RectF first, ref RectF second, out RectF result)
    {
        //this implementation seems a little wasteful with all of these allocations.
        Vector2 firstMinimum = first.TopLeft;
        Vector2 firstMaximum = first.BottomRight;
        Vector2 secondMinimum = second.TopLeft;
        Vector2 secondMaximum = second.BottomRight;

        MathV.Maximize(ref firstMinimum, ref secondMinimum, out firstMinimum);
        MathV.Minimize(ref firstMaximum, ref secondMaximum, out firstMaximum);

        if ((firstMaximum.X < firstMinimum.X) || (firstMaximum.Y < firstMinimum.Y))
            result = new RectF();
        else
            result = FromCorners(firstMinimum, firstMaximum);
    }
    public bool Intersects(in RectF rectangle)
    {
        return Intersects(in this, in rectangle);
    }
    public static bool Intersects(in RectF first, in RectF second)
    {
        return first.x < second.x + second.width && first.x + first.width > second.x &&
               first.y < second.y + second.height && first.y + first.height > second.y;
    }

    public bool Intersects(in AABB aabb)
    {
        return Intersects(in this, in aabb);
    }
    public static bool Intersects(in RectF first, in AABB second)
    {
        return first.x < second.max.X && first.x + first.width > second.min.Y &&
               first.y < second.max.Y && first.y + first.height > second.min.Y;
    }


    public bool Contains(in Vector2 point)
    {
        float X = point.X - x;
        float Y = point.Y - y;

        return X >= 0 && Y >= 0 && X <= width && Y <= height;
    }

    public void GetExtents(out float left, out float right, out float top, out float bottom)
    {
        left = x;
        right = x + width;
        bottom = y;
        top = y + height;
    }
}