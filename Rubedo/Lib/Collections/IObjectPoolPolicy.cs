namespace Rubedo.Lib.Collections;

/// <summary>
/// Represents the method to create and reset pooled objects.
/// </summary>
public interface IObjectPoolPolicy<T>
{
    public T Create();
    public bool Reset(T obj);
}