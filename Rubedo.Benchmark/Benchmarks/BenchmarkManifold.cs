using BenchmarkDotNet.Attributes;
using Microsoft.Xna.Framework;
using Rubedo.Physics2D.Collision;

namespace Rubedo.Benchmarks;

public class BenchmarkManifold
{
    Manifold manifold;
    Contact contact1;
    Contact contact2;

    public BenchmarkManifold()
    {
        manifold = new Manifold(null, null);
        contact1 = new Contact(Vector2.Zero);
        contact2 = new Contact(Vector2.Zero);
    }

    [Benchmark(Baseline = true)]
    public Manifold ManifoldOld()
    {
        manifold.Update(2, contact1, contact2);
        return manifold;
    }
    [Benchmark]
    public Manifold ManifoldNew()
    {
        manifold.Update(contact1, contact2);
        return manifold;
    }
}
