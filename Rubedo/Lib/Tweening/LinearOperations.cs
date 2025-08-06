using System.Linq.Expressions;
using System;

namespace Rubedo.Lib.Tweening;

/// <summary>
/// TODO: I am LinearOperations, and I don't have a summary yet.
/// </summary>
public class LinearOperations<T>
{
    static LinearOperations()
    {
        var a = Expression.Parameter(typeof(T));
        var b = Expression.Parameter(typeof(T));
        var c = Expression.Parameter(typeof(float));
        Add =      Expression.Lambda<Func<T, T, T>>(Expression.Add(a, b), a, b).Compile();
        Subtract = Expression.Lambda<Func<T, T, T>>(Expression.Subtract(a, b), a, b).Compile();
        Multiply = Expression.Lambda<Func<T, float, T>>(Expression.Multiply(a, c), a, c).Compile();
    }

    public static Func<T, T, T> Add { get; }
    public static Func<T, T, T> Subtract { get; }
    public static Func<T, float, T> Multiply { get; }
}