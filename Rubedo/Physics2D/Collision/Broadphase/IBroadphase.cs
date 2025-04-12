using System.Collections.Generic;
using PhysicsEngine2D;
using Rubedo.Physics2D.Dynamics;

namespace Rubedo.Physics2D.Collision.Broadphase;

internal interface IBroadphase
{
    void Add(PhysicsBody body);
    void Remove(PhysicsBody body);
    void Update(List<PhysicsBody> bodies);

    bool Raycast(Ray2 ray, float distance, out RaycastResult result);

    void ComputePairs(HashSet<Manifold> manifolds);
    void Clear();

    void DebugDraw(Render.Shapes shapes);
}
