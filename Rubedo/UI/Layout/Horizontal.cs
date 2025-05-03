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

        foreach (var c in _children)
        {
            if (!c.IsVisible())
                continue;
            c.UpdateSizes();
            maxWidth += c.Width + childPadding;
            maxHeight = MathF.Max(c.Height, maxHeight);
        }

        Width = maxWidth + paddingLeft + paddingRight;
        Height = maxHeight + paddingTop + paddingBottom;
    }

    public override void UpdateLayout()
    {
        float maxHeight = Height;
        float currentX = paddingLeft;

        foreach (var c in _children)
        {
            if (!c.IsVisible())
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