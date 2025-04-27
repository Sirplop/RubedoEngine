using Rubedo.Lib;
using System;
using System.Collections.Generic;

namespace Rubedo.UI.Layout;

/*
/// <summary>
/// A layout that anchors its children to a given side. Note: Will overlap components.
/// </summary>
public class AnchorPanel : LayoutGroup
{
    public enum AnchorPoint
    {
        TopLeft,
        Top,
        TopRight,
        Left,
        Center,
        Right,
        BottomLeft,
        Bottom,
        BottomRight,
    }

    public AnchorPoint anchor = AnchorPoint.TopLeft;

    protected float realX = 0;
    protected float realY = 0;

    public AnchorPanel(int sizeX, int sizeY)
    {

    }
    
    public override void UpdatePrefSizes()
    {
        float maxWidth = 0;
        float maxHeight = 0;

        foreach (IComponent c in _children)
        {
            if (!c.isVisible)
                continue;
            c.UpdatePrefSizes();
            maxWidth = MathF.Max(c.PrefWidth, maxWidth);
            maxHeight = MathF.Max(c.PrefHeight, maxHeight);
        }

        prefWidth = maxWidth;
        prefHeight = maxHeight;
    }

    public override void UpdateLayout()
    {
        realX = x;
        realY = y;

        RectF clip = Clip;
        foreach (var c in _children)
        {
            if (!c.isVisible)
                continue;
            c.X = x;
            c.Y = y;
            c.Width = c.PrefWidth;
            c.Height = MathF.Min(c.PrefHeight, height);

            maxHeight = MathF.Max(c.Height, maxHeight);
            c.Clip = c.Bounds.Intersection(clip);
            if (c is IParent p)
                p.UpdateLayout();

            currentX += c.Width;
        }
    }
}*/