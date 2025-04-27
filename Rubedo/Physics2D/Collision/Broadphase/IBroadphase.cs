using System.Collections.Generic;
using PhysicsEngine2D;
using Rubedo.Physics2D.Dynamics;
using Rubedo.Physics2D.Math;

namespace Rubedo.Physics2D.Collision.Broadphase;

internal interface IBroadphase
{
    void Add(PhysicsBody body);
    void Remove(PhysicsBody body);
    void Update(List<PhysicsBody> bodies);

    bool Raycast(Ray2D ray, float distance, out RaycastResult result);

    void ComputePairs(List<Manifold> manifolds, HashSet<Manifold> manifoldSet);
    void Clear();

    void DebugDraw(Rendering.Shapes shapes);
}
