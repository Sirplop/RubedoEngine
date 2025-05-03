namespace Rubedo.Input.Conditions;

/// <summary>
/// An <see cref="ICondition"/> that returns true when any of it's conditions are met.
/// </summary>
public class AnyCondition : ICondition
{
    private readonly ICondition[] _conditions;

    public AnyCondition(params ICondition[] conditions)
    {
        _conditions = conditions;
    }

    public bool Pressed(bool consume = true)
    {
        bool pressed = false;
        for (int i = 0; i < _conditions.Length; i++)
        {
            if (_conditions[i].Pressed(false))
            {
                pressed = true;
                break;
            }
        }
        if (pressed && consume)
            Consume();
        return pressed;
    }
    public bool Held(bool consume = true)
    {
        bool held = false;
        for (int i = 0; i < _conditions.Length; i++)
        {
            if (_conditions[i].Held(false))
            {
                held = true;
                break;
            }
        }
        if (held && consume)
            Consume();
        return held;
    }
    public bool Released(bool consume = true)
    {
        bool released = false;
        for (int i = 0; i < _conditions.Length; i++)
        {
            if (_conditions[i].Released(false))
            {
                released = true;
                break;
            }
        }
        if (released && consume)
            Consume();
        return released;
    }

    public void Consume()
    {
        for (int i = 0; i < _conditions.Length; i++)
        {
            _conditions[i].Consume();
        }
    }
}