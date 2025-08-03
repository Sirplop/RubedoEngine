using System.Collections.Generic;

namespace Rubedo.Lib.Collections;

/// <summary>
/// Instanced object pool. For global object pools, use <see cref="GlobalPool{T}"/>
/// </summary>
public class ObjectPool<T> where T : IPoolable
{
    private Queue<T> _pool;
    private IObjectPoolPolicy<T> policy;

    public ObjectPool(int initialSize, IObjectPoolPolicy<T> policy)
    {
        this.policy = policy;
        _pool = new Queue<T>(initialSize);
        Warm(initialSize);
    }

    /// <summary>
    /// Pre-fills the pool with <paramref name="cacheSize"/> objects.
    /// </summary>
    /// <param name="cacheSize">The number of objects to create.</param>
    public void Warm(int cacheSize)
    {
        cacheSize -= _pool.Count;
        if (cacheSize > 0)
        {
            for (int i = 0; i < cacheSize; i++)
                _pool.Enqueue(policy.Create());
        }
    }

    /// <summary>
    /// Empties the pool.
    /// </summary>
    public void Clear()
    {
        _pool.Clear();
    }

    /// <summary>
    /// Gets a pooled item. If none exist, creates a new one. Use <see cref="Release(T)"/> to return it to the pool.
    /// </summary>
    public T Obtain()
    {
        if (_pool.Count > 0)
            return _pool.Dequeue();

        return policy.Create();
    }

    /// <summary>
    /// Puts an object back into the pool.
    /// </summary>
    public void Release(T obj)
    {
        _pool.Enqueue(obj);

        if (obj is IPoolable poolable)
            poolable.Reset();
    }
}