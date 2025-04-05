using System.Collections.Generic;

namespace Rubedo.Physics2D.Util;

/// <summary>
/// I am ManifoldPool, and this is my summary.
/// </summary>
internal class ManifoldPool
{
    private readonly Stack<Manifold> _pool = new Stack<Manifold>();

    public Manifold Get()
    {
        if (_pool.Count > 0)
        {
            Manifold m = _pool.Pop();
            m.Reset();
            return m;
        }
        return new Manifold();
    }

    public void Release(Manifold manifold)
    {
        manifold.Reset();
        _pool.Push(manifold);
    }
}