
using Microsoft.Xna.Framework;

namespace PhysicsEngine2D;

public interface IDebugDrawer
{
    void Draw(Vector2[] vertices, params object[] data);
    void Draw(Vector2 center, float radius, params object[] data);
}
