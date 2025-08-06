using System;

namespace Rubedo.Lib.Tweening;

/// <summary>
/// TODO: I am TweenMember, and I don't have a summary yet.
/// </summary>
public abstract class TweenMember
{
    protected TweenMember(object target)
    {
        Target = target;
    }

    public object Target { get; }
    public abstract Type Type { get; }
    public abstract string Name { get; }
}

public abstract class TweenMember<T> : TweenMember
    where T : struct
{
    protected TweenMember(object target, Func<object, T> getMethod, Action<object, T> setMethod)
        : base(target)
    {
        _getMethod = getMethod;
        _setMethod = setMethod;
    }

    private readonly Func<object, T> _getMethod;
    private readonly Action<object, T> _setMethod;

    public T Value
    {
        get { return _getMethod(Target); }
        set { _setMethod(Target, value); }
    }
}