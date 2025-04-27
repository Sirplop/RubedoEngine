using Microsoft.Xna.Framework;
using System;

namespace Rubedo.UI.Input;

/// <summary>
/// TODO: I am Button, and I don't have a summary yet.
/// </summary>
public class Button : UIComponent
{
    public bool clicked = false;

    public event Action OnPressed;
    public event Action OnHeld;
    public event Action OnReleased;

    public override void UpdateSizes()
    {
        float maxWidth = 0;
        float maxHeight = 0;

        foreach (var c in _children)
        {
            if (!c.isVisible)
                continue;
            c.UpdateSizes();
            maxWidth = MathF.Max(c.Width, maxWidth);
            maxHeight = MathF.Max(c.Height, maxHeight);
        }
        Width = maxWidth;
        Height = maxHeight;
    }
    public override void UpdateLayout()
    {
        foreach (var c in _children)
        {
            if (!c.isVisible)
                continue;
            c.UpdateClipIfDirty();
            c.UpdateLayout();
        }
    }
    public override void UpdateInput()
    {
        if (Clip.Contains(RubedoEngine.Input.MouseScreenPosition()))
        {
            if (RubedoEngine.Input.MouseDown(InputManager.MouseButtons.Left))
            {
                if (!clicked)
                {
                    clicked = true;
                    OnPressed?.Invoke();
                }
                else
                {
                    OnHeld?.Invoke();
                }
            }
            else if (clicked)
            {
                clicked = false;
                OnReleased?.Invoke();
            }
        } else if (clicked)
        { //we've moved off the button, release everything.
            clicked = false;
            OnReleased?.Invoke();
        }

        base.UpdateInput();
    }
}