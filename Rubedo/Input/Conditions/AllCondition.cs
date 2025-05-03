namespace Rubedo.Input.Conditions;

/// <summary>
/// An <see cref="ICondition"/> that returns true when all of it's conditions are met.
/// </summary>
public class AllCondition : ICondition
{
    private readonly ICondition[] _conditions;

    public AllCondition(params ICondition[] conditions)
    {
        _conditions = conditions;
    }

    public bool Pressed(bool consume = true)
    {
        bool pressed = true;
        for (int i = 0; i < _conditions.Length; i++)
        {
            if (!_conditions[i].Pressed(false))
            {
                pressed = false;
                break;
            }
        }
        if (pressed && consume)
            Consume();
        return pressed;
    }
    public bool Held(bool consume = true)
    {
        bool held = true;
        for (int i = 0; i < _conditions.Length; i++)
        {
            if (!_conditions[i].Held(false))
            {
                held = false;
                break;
            }
        }
        if (held && consume)
            Consume();
        return held;
    }
    public bool Released(bool consume = true)
    {
        bool released = true;
        for (int i = 0; i < _conditions.Length; i++)
        {
            if (!_conditions[i].Released(false))
            {
                released = false;
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