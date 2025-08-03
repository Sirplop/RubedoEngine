using System.Collections.Generic;

namespace Rubedo.Lib.Collections;

/// <summary>
/// Static class that can be used to globally pool any object. Use with an <see cref="IPoolable"/> object for additional functionality.
/// For instanced pools, use <see cref="ObjectPool"/>.
/// </summary>
public static class GlobalPool<T> where T : new()
{
    private static Queue<T> _pool = new Queue<T>(10);

    /// <summary>
    /// Pre-fills the pool with <paramref name="cacheSize"/> objects.
    /// </summary>
    /// <param name="cacheSize">The number of objects to create.</param>
    public static void Warm(int cacheSize)
    {
        cacheSize -= _pool.Count;
        if (cacheSize > 0)
        {
            for (int i = 0; i < cacheSize; i++)
                _pool.Enqueue(new T());
        }
    }

    /// <summary>
    /// Empties the pool.
    /// </summary>
    public static void Clear()
    {
        _pool.Clear();
    }

    /// <summary>
    /// Gets a pooled item. If none exist, creates a new one. Use <see cref="Release(T)"/> to return it to the pool.
    /// </summary>
    public static T Obtain()
    {
        if (_pool.Count > 0)
            return _pool.Dequeue();

        return new T();
    }

    /// <summary>
    /// Puts an object back into the pool.
    /// </summary>
    public static void Release(T obj)
    {
        _pool.Enqueue(obj);

        if (obj is IPoolable poolable)
            poolable.Reset();
    }
}

/// <summary>
/// Objects implementing this will have <see cref="Reset"/> called when freed to their pool.
/// </summary>
public interface IPoolable
{
    /// <summary>
    /// Resets the object to be used by the pool again.
    /// </summary>
    public void Reset();
}