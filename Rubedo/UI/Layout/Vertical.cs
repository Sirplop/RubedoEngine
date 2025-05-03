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
    public override void UpdateSizes()
    {
        float maxWidth = 0;
        float maxHeight = 0;

        foreach (var c in _children)
        {
            if (!c.IsVisible())
                continue;
            c.UpdateSizes();
            maxWidth = MathF.Max(c.Width, maxWidth);
            maxHeight += c.Height + childPadding;
        }

        Width = maxWidth + paddingLeft + paddingRight;
        Height = maxHeight + paddingTop + paddingBottom;
    }

    public override void UpdateLayout()
    {
        float maxWidth = Width;
        float currentY = paddingTop;

        foreach (UIComponent c in _children)
        {
            if (!c.IsVisible())
                continue;
            c.Offset = new Vector2(paddingLeft, currentY);
            //c.Width = MathF.Min(c.Width, Width);

            maxWidth = MathF.Max(c.Width, maxWidth);
            c.UpdateClipIfDirty();
            c.UpdateLayout();

            currentY += c.Height;
        }
    }
}