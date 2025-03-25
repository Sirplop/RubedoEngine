using System.Collections.Generic;

namespace Rubedo.Physics2D.Util;

/// <summary>
/// I am ManifoldPool, and this is my summary.
/// </summary>
internal class ManifoldPool
{
    private readonly Stack<CollisionManifold> _pool = new Stack<CollisionManifold>();

    public CollisionManifold Get()
    {
        if (_pool.Count > 0)
        {
            CollisionManifold m = _pool.Pop();
            m.Reset();
            return m;
        }
        return new CollisionManifold();
    }

    public void Release(CollisionManifold manifold)
    {
        manifold.Reset();
        _pool.Push(manifold);
    }
}