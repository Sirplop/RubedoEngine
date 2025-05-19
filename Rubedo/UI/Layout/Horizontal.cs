using System;
using Microsoft.Xna.Framework;

namespace Rubedo.UI.Layout;

/// <summary>
/// TODO: I am Horizontal, and I don't have a summary yet.
/// </summary>
public class Horizontal : LayoutGroup
{
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

        Width = MathF.Min(maxWidth + paddingLeft + paddingRight, absMaxWidth);
        Height = MathF.Min(maxHeight + paddingTop + paddingBottom, absMaxHeight);
    }

    public override void UpdateLayout()
    {
        float maxHeight = Height;
        float currentX = paddingLeft;

        foreach (var c in _children)
        {
            if (!c.IsVisible() || c.IgnoresLayout)
                continue;
            c.Offset = new Vector2(currentX, paddingTop);
            //c.Height = MathF.Min(c.Height, Height);

            maxHeight = MathF.Max(c.Height, maxHeight);
            c.UpdateClipIfDirty();
            c.UpdateLayout();

            currentX += c.Width + childPadding;
        }
    }
}