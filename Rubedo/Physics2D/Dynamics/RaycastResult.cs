using Microsoft.Xna.Framework;

namespace Rubedo.Physics2D.Dynamics;

public struct RaycastResult
{
    public Vector2 point;
    public Vector2 normal;

    public float distance;

    public RaycastResult(Vector2 point, Vector2 normal, float distance)
    {
        this.point = point;
        this.normal = normal;
        this.distance = distance;
    }
}
