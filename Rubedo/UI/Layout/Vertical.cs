using Microsoft.Xna.Framework;
using Rubedo.Lib;
using System;
using System.Collections.Generic;

namespace Rubedo.UI.Layout;

/// <summary>
/// TODO: I am Vertical, and I don't have a summary yet.
/// </summary>
public class Vertical : LayoutGroup
{
    public enum LayoutDirection
    {
        Up,
        Down
    }

    /// <summary>
    /// The direction that layout items will be ordered. Defaults to top to bottom.
    /// </summary>
    public LayoutDirection ItemOrdering {
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
    protected LayoutDirection itemOrder = LayoutDirection.Down;

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
            maxWidth = MathF.Max(c.Width, maxWidth);
            maxHeight += c.Height + childPadding;
        }

        Width = MathF.Min(maxWidth + paddingLeft + paddingRight, absMaxWidth);
        Height = MathF.Min(maxHeight + paddingTop + paddingBottom, absMaxHeight);
    }

    public override void UpdateLayout()
    {
        switch (itemOrder)
        {
            case LayoutDirection.Down:
                LayoutDownwards();
                break;
            case LayoutDirection.Up:
                LayoutUpwards();
                break;
        }
    }

    protected virtual void LayoutUpwards()
    {
        float maxWidth = Width;
        float currentY = Height - paddingBottom;

        foreach (UIComponent c in _children)
        {
            if (!c.IsVisible() || c.IgnoresLayout)
                continue;
            currentY -= c.Height + childPadding;
            c.Offset = new Vector2(paddingLeft, currentY);

            maxWidth = MathF.Max(c.Width, maxWidth);
            c.UpdateClipIfDirty();
            c.UpdateLayout();
        }
    }
    protected virtual void LayoutDownwards()
    {
        float maxWidth = Width;
        float currentY = paddingTop;

        foreach (UIComponent c in _children)
        {
            if (!c.IsVisible() || c.IgnoresLayout)
                continue;
            c.Offset = new Vector2(paddingLeft, currentY);

            maxWidth = MathF.Max(c.Width, maxWidth);
            c.UpdateClipIfDirty();
            c.UpdateLayout();

            currentY += c.Height + childPadding;
        }
    }
}