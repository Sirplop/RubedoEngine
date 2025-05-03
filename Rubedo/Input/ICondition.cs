namespace Rubedo.Input;

/// <summary>
/// Represents a series of checks to see if this condition is met.
/// </summary>
public interface ICondition
{
    /// <returns>True when the given condition is pressed.</returns>
    /// <param name="consume">Whether or not to consume this condition, should it succeed.</param>
    bool Pressed(bool consume = true);
    /// <returns>True when the given condition is held.</returns>
    /// <param name="consume">Whether or not to consume this condition, should it succeed.</param>
    bool Held(bool consume = true);
    /// <returns>True when the given condition is released.</returns>
    /// <param name="consume">Whether or not to consume this condition, should it succeed.</param>
    bool Released(bool consume = true);
    /// <summary>
    /// Consumes this condition for this frame.
    /// </summary>
    void Consume();
}