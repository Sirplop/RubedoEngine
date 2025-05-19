namespace Rubedo.Lib;

/// <summary>
/// Supplies simple methods for padding and thickness.
/// </summary>
public struct Padding
{
    public int Left { get; set; }
    public int Top { get; set; }
    public int Right { get; set; }
    public int Bottom { get; set; }
    public readonly int Width => Left + Right;
    public readonly int Height => Top + Bottom;
    public readonly Vector2Int Size => new Vector2Int(Width, Height);


    public Padding(int all) : this(all, all, all, all) { }
    public Padding(int leftRight, int topBottom)
    : this(leftRight, topBottom, leftRight, topBottom) { }

    public Padding(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public readonly void Deconstruct(out int left, out int top, out int right, out int bottom)
    {
        (left, top, right, bottom) = (Left, Top, Right, Bottom);
    }
}