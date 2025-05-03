using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Rubedo.Input.Conditions;

/// <summary>
/// An <see cref="ICondition"/> that handles a given mouse event.
/// </summary>
public class MouseCondition : ICondition
{
    private InputManager.MouseButtons button;

    public MouseCondition(InputManager.MouseButtons button)
    {
        this.button = button;
    }

    public bool Pressed(bool consume = true)
    {
        if (IsNotConsumed(button) && InputManager.MousePressed(button))
        {
            if (consume)
                Consume();
            return true;
        }
        return false;
    }

    public bool Held(bool consume = true)
    {
        if (IsNotConsumed(button) && InputManager.MouseHeld(button))
        {
            if (consume)
                Consume();
            return true;
        }
        return false;
    }

    public bool Released(bool consume = true)
    {
        if (IsNotConsumed(button) && InputManager.MouseReleased(button))
        {
            if (consume)
                Consume();
            return true;
        }
        return false;
    }
    public void Consume()
    {
        Consume(button);
    }

    #region Static functionality
    protected static readonly Dictionary<InputManager.MouseButtons, ushort> Consumed = new Dictionary<InputManager.MouseButtons, ushort>();
    protected static void Consume(InputManager.MouseButtons button)
    {
        Consumed[button] = InputManager.FrameCounter;
    }
    protected static bool IsNotConsumed(InputManager.MouseButtons button)
    {
        return !Consumed.ContainsKey(button) || Consumed[button] != InputManager.FrameCounter;
    }
    #endregion
}