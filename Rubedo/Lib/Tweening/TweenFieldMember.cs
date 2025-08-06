﻿using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System;

namespace Rubedo.Lib.Tweening;

/// <summary>
/// TODO: I am TweenFieldMember, and I don't have a summary yet.
/// </summary>
public sealed class TweenFieldMember<T> : TweenMember<T>
        where T : struct
{
    private readonly FieldInfo _fieldInfo;

    public TweenFieldMember(object target, FieldInfo fieldInfo)
        : base(target, CompileGetMethod(fieldInfo), CompileSetMethod(fieldInfo))
    {
        _fieldInfo = fieldInfo;
    }

    private static Func<object, T> CompileGetMethod(FieldInfo fieldInfo)
    {
        var entityType = fieldInfo.DeclaringType!;
        var parameter = Expression.Parameter(typeof(object), "entity");
        var property = Expression.Field(Expression.Convert(parameter, entityType), fieldInfo);
        return Expression.Lambda<Func<object, T>>(property, parameter).Compile();
    }

    private static Action<object, T> CompileSetMethod(FieldInfo fieldInfo)
    {
        Debug.Assert(fieldInfo.DeclaringType != null);

        var entityType = fieldInfo.DeclaringType!;
        var targetParam = Expression.Parameter(typeof(object), "target");
        var valueParam = Expression.Parameter(typeof(T), "value");
        var conversion = Expression.Convert(targetParam, entityType);

        var field = Expression.Field(conversion, fieldInfo);
        var assignation = Expression.Assign(field, valueParam);

        return Expression.Lambda<Action<object, T>>(assignation, targetParam, valueParam).Compile();
    }

    public override Type Type => _fieldInfo.FieldType;
    public override string Name => _fieldInfo.Name;
}