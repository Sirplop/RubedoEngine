using System;
using Microsoft.Xna.Framework;
using static Rubedo.UI.Layout.Vertical;

namespace Rubedo.UI.Layout;

/// <summary>
/// TODO: I am Horizontal, and I don't have a summary yet.
/// </summary>
public class Horizontal : LayoutGroup
{
    public enum LayoutDirection
    {
        Left,
        Right
    }

    /// <summary>
    /// The direction that layout items will be ordered. Defaults to left to right.
    /// </summary>
    public LayoutDirection ItemOrdering
    {
        get => itemOrder;
        set
        {
            if (itemOrder != value)
            {
                itemOrder = value;
                MarkLayoutAsDirty();
            }
        }
    }
    protected LayoutDirection itemOrder = LayoutDirection.Right;

    public override void UpdateSizes()
    {
        float maxWidth = 0;
        float maxHeight = 0;
        float absMaxWidth = float.MaxValue;
        float absMaxHeight = float.MaxValue;
        if (MaxSize.HasValue)
        {
            if (MaxSize.Value.X > -1)
                absMaxWidth = MaxSize.Value.X;
            if (MaxSize.Value.Y > -1)
                absMaxHeight = MaxSize.Value.Y;
        }

        foreach (var c in _children)
        {
            if (!c.IsVisible() || c.IgnoresLayout)
                continue;
            c.UpdateSizes();
            maxWidth += c.Width + childPadding;
            maxHeight = MathF.Max(c.Height, maxHeight);
        }
        if (maxWidth > 0) //remove extra child padding.
            maxWidth -= childPadding;

        Width = MathF.Min(maxWidth + paddingLeft + paddingRight, absMaxWidth);
        Height = MathF.Min(maxHeight + paddingTop + paddingBottom, absMaxHeight);
    }

    public override void UpdateLayout()
    {
        switch (itemOrder)
        {
            case LayoutDirection.Left:
                LayoutLeft();
                break;
            case LayoutDirection.Right:
                LayoutRight();
                break;
        }
    }

    protected virtual void LayoutLeft()
    {
        float maxHeight = Height;
        float currentX = Width - paddingRight;

        foreach (var c in _children)
        {
            if (!c.IsVisible() || c.IgnoresLayout)
                continue;

            currentX -= c.Width + childPadding; switch (c.Anchor)
            {
                case Anchor.TopLeft:
                case Anchor.Top:
                case Anchor.TopRight:
                    c.Offset = new Vector2(currentX, paddingTop);
                    break;
                case Anchor.Left:
                case Anchor.Center:
                case Anchor.Right:
                    c.Offset = new Vector2(currentX - (Height * 0.5f) + (c.Width * 0.5f), paddingTop);
                    break;
                case Anchor.BottomLeft:
                case Anchor.Bottom:
                case Anchor.BottomRight:
                    c.Offset = new Vector2(Width - currentX - c.Width, paddingTop);
                    break;
            }

            maxHeight = MathF.Max(c.Height, maxHeight);
            c.UpdateClipIfDirty();
            c.UpdateLayout();
        }
    }
    protected virtual void LayoutRight()
    {
        float maxHeight = Height;
        float currentX = paddingLeft;

        foreach (var c in _children)
        {
            if (!c.IsVisible() || c.IgnoresLayout)
                continue;
            switch (c.Anchor)
            {
                case Anchor.TopLeft:
                case Anchor.Top:
                case Anchor.TopRight:
                    c.Offset = new Vector2(currentX, paddingTop);
                    break;
                case Anchor.Left:
                case Anchor.Center:
                case Anchor.Right:
                    c.Offset = new Vector2(currentX - (Width * 0.5f) + (c.Width * 0.5f), paddingTop);
                    break;
                case Anchor.BottomLeft:
                case Anchor.Bottom:
                case Anchor.BottomRight:
                    c.Offset = new Vector2(Width - currentX - c.Width, paddingTop);
                    break;
            }

            maxHeight = MathF.Max(c.Height, maxHeight);
            c.UpdateClipIfDirty();
            c.UpdateLayout();

            currentX += c.Width + childPadding;
        }
    }
}