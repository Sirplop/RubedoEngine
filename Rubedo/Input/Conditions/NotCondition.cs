namespace Rubedo.Input.Conditions;

/// <summary>
/// An <see cref="ICondition"/> that returns true when none of it's conditions are true. Inputs cannot be consumed by this condition.
/// </summary>
public class NotCondition : ICondition
{
    private readonly ICondition[] _conditions;

    public NotCondition(params ICondition[] conditions)
    {
        _conditions = conditions;
    }

    public bool Pressed(bool consume = false)
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
        return !pressed;
    }
    public bool Held(bool consume = false)
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
        return !held;
    }
    public bool Released(bool consume = false)
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
        return !released;
    }

    public void Consume()
    {
        return;
    }
}