using Microsoft.Xna.Framework;
using Rubedo.Input;
using System;

namespace Rubedo.UI.Input;

/// <summary>
/// A pressable UI component, the base of many.
/// </summary>
public class Button : Selectable
{
    public bool Clicked => clicked;
    protected bool clicked = false;

    public float HeldDuration => heldDuration;
    protected float heldDuration = 0;

    protected bool held = false;
    protected bool mousePressed = false;
    protected bool buttonPressed = false;

    /// <summary>
    /// Called the frame the button is pressed.
    /// </summary>
    public event Action<Button> OnPressed;
    /// <summary>
    /// Called every frame the button is held. Use <see cref="HeldDuration"/> for how long it's been held.
    /// </summary>
    public event Action<Button> OnHeld;
    /// <summary>
    /// Called the frame the button is released.
    /// </summary>
    public event Action<Button> OnReleased;
    /// <summary>
    /// Called only when a button press fails, for whatever reason (usually by moving the focus off of the button while holding it down.)
    /// </summary>
    public event Action<Button> OnReleaseFailed;

    public override void UpdateSizes()
    {
        float maxWidth = 0;
        float maxHeight = 0;

        foreach (var c in _children)
        {
            if (!c.IsVisible() || c.IgnoresLayout)
                continue;
            c.UpdateSizes();
            maxWidth = MathF.Max(c.Width, maxWidth);
            maxHeight = MathF.Max(c.Height, maxHeight);
        }
        Width = maxWidth;
        Height = maxHeight;
    }
    public override void UpdateInput()
    {
        base.UpdateInput();
        if (GUI.MouseControlsEnabled && isHovered)
        {
            GUI.Root.GrabFocus(this);
            if (NavControls.MouseInteract.Pressed())
            {
                clicked = true;
                mousePressed = true;
                buttonPressed = false;
                OnPressed?.Invoke(this);
            }
        } else if (!GUI.MouseControlsEnabled && IsFocused && NavControls.ButtonInteract.Pressed())
        {
            clicked = true;
            mousePressed = false;
            buttonPressed = true;
            OnPressed?.Invoke(this);
        }

        if (mousePressed && !isHovered)
        { //we've moved off the button, stop doing button stuff.
            heldDuration = 0;
            clicked = false;
            held = false;
            mousePressed = false;
            buttonPressed = false;
            OnReleaseFailed?.Invoke(this);
        }

        if (IsFocused)
        {
            if (clicked)
            {
                heldDuration += RubedoEngine.DeltaTime;
                if (buttonPressed && NavControls.ButtonInteract.Released())
                {
                    heldDuration = 0;
                    clicked = false;
                    held = false;
                    buttonPressed = false;
                    OnReleased?.Invoke(this);
                }
                else if (mousePressed && NavControls.MouseInteract.Released())
                {
                    heldDuration = 0;
                    clicked = false;
                    held = false;
                    mousePressed = false;
                    OnReleased?.Invoke(this);
                }
                else if (heldDuration > 0)
                {
                    OnHeld?.Invoke(this);
                }
            }
        }
        else if (clicked) //no longer focused, which means we've lost control.
        {
            heldDuration = 0;
            clicked = false;
            held = false;
            mousePressed = false;
            buttonPressed = false;
            OnReleaseFailed?.Invoke(this);
        }
        base.UpdateInput();
    }
}