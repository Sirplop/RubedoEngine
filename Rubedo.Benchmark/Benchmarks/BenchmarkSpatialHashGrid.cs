using BenchmarkDotNet.Attributes;
using Microsoft.Xna.Framework;
using Rubedo.Object;
using Rubedo.Physics2D.Collision;
using Rubedo.Physics2D.Collision.Broadphase;
using Rubedo.Physics2D.Common;
using Rubedo.Physics2D.Dynamics;
using Rubedo.Physics2D.Dynamics.Shapes;
using System.Collections.Generic;

namespace Rubedo.Benchmarks;

[MemoryDiagnoser]
public class BenchmarkSpatialHashGrid
{
    const int BodyCount = 25;
    const float Spacing = 0.75f; // positional spacing requested
    const int Columns = 5; // layout: 100 columns x 10 rows = 1000 bodies

    SpatialHashGrid grid;
    List<PhysicsBody> bodies;
    List<Manifold> manifolds;

    [GlobalSetup]
    public void Setup()
    {
        // Use a cell size that maps reasonably to unit shapes.
        int cellSize = 3;
        grid = new SpatialHashGrid(cellSize);

        bodies = new List<PhysicsBody>(BodyCount);
        manifolds = new List<Manifold>();

        PhysicsMaterial defaultMaterial = new PhysicsMaterial(1f, 0.5f, 0.1f);

        for (int i = 0; i < BodyCount; i++)
        {
            int col = i % Columns;
            int row = i / Columns;
            Vector2 position = new Vector2(col * Spacing, row * Spacing);

            Entity e = new Entity(position);
            Collider c = Collider.CreateUnitShape(ShapeType.Box, false);
            e.Add(c);

            PhysicsBody body = new PhysicsBody(c, defaultMaterial);
            e.Add(body);

            // PhysicsBody.Update normally updates bounds; ensure bounds reflect transform by calling Update on the component list.
            // Entity.Update will call component.Update; call it once so body.bounds are set before broadphase update.
            e.Transform.attached = e; // already set by constructor, but kept for clarity
            // ComponentList.Update is internal; calling Entity.Update will run components Update.
            e.Update();

            bodies.Add(body);
        }

        // Run one Update to populate the spatial hash grid as requested.
        grid.Update(bodies);
    }

    [Benchmark]
    public void ComputePairsThenClear()
    {
        grid.ComputePairs(manifolds);
        grid.TestClear();
        manifolds.Clear(); // ensure next iteration starts with an empty list
    }
}