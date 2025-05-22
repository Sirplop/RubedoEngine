using Rubedo.Graphics;
using Rubedo.Lib;
using System;
using System.Collections.Generic;

namespace Rubedo.UI.Layout;


/// <summary>
/// A general layout panel of a fixed size.
/// </summary>
public class Panel : UIComponent
{
    public int PrefWidth
    {
        get => _prefWidth;
        set
        {
            if (_prefWidth != value)
            {
                _prefWidth = value;
                MarkLayoutAsDirty();
            }
        }
    }
    private int _prefWidth = 0;
    public int PrefHeight
    {
        get => _prefHeight;
        set
        {
            if (_prefHeight != value)
            {
                _prefHeight = value;
                MarkLayoutAsDirty();
            }
        }
    }
    private int _prefHeight = 0;

    public Panel(int prefWidth, int prefHeight)
    {
        PrefWidth = prefWidth;
        PrefHeight = prefHeight;
    }

    public override void UpdateSizes()
    {
        Width = _prefWidth;
        Height = _prefHeight;
        base.UpdateSizes();
    }
}