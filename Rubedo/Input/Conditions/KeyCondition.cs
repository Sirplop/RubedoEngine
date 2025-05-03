using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Rubedo.Input.Conditions;

/// <summary>
/// An <see cref="ICondition"/> that handles a given key event.
/// </summary>
public class KeyCondition : ICondition
{
    private Keys key;
    private bool modifier;

    /// <param name="key">The key whose state to check.</param>
    /// <param name="modifier">Overrides the condition to not be consumeable, and to only check if the key is down. Useful for modifier keys.</param>
    public KeyCondition(Keys key, bool modifier = false)
    {
        this.key = key;
        this.modifier = modifier;
    }

    public bool Pressed(bool consume = true)
    {
        if (modifier)
        {
            return InputManager.KeyDown(key);
        }
        if (IsNotConsumed(key) && InputManager.KeyPressed(key))
        {
            if (consume)
                Consume();
            return true;
        }
        return false;
    }

    public bool Held(bool consume = true)
    {
        if (modifier)
        {
            return InputManager.KeyDown(key);
        }
        if (IsNotConsumed(key) && InputManager.KeyHeld(key))
        {
            if (consume)
                Consume();
            return true;
        }
        return false;
    }

    public bool Released(bool consume = true)
    {
        if (modifier)
        {
            return InputManager.KeyDown(key);
        }
        if (IsNotConsumed(key) && InputManager.KeyReleased(key))
        {
            if (consume)
                Consume();
            return true;
        }
        return false;
    }

    public void Consume()
    {
        if (!modifier) //only consume if not modifier.
            Consume(key);
    }

    #region Static functionality
    protected static readonly Dictionary<Keys, ushort> Consumed = new Dictionary<Keys, ushort>();
    public static void Consume(Keys key)
    {
        Consumed[key] = InputManager.FrameCounter;
    }
    public static bool IsNotConsumed(Keys key)
    {
        return !Consumed.ContainsKey(key) || Consumed[key] != InputManager.FrameCounter;
    }
    #endregion
}