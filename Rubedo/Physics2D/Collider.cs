using Microsoft.Xna.Framework;
using Rubedo.Components;
using Rubedo.Object;
using Rubedo.Physics2D.ColliderShape;
using System.Collections.Generic;

namespace Rubedo.Physics2D;

/// <summary>
/// I am ColliderComponent, and this is my summary.
/// </summary>
public class Collider : Component
{
    public readonly static float UNIT_CIRCLE_RADIUS = 0.5f * RubedoEngine.SizeOfMeter;
    public readonly static float UNIT_BOX_SIDE = 1f * RubedoEngine.SizeOfMeter;
    public readonly static float UNIT_CAPSULE_LENGTH = 0.334f * RubedoEngine.SizeOfMeter;
    public readonly static float UNIT_CAPSULE_RADIUS = 0.334f * RubedoEngine.SizeOfMeter;
    public readonly static List<Vector2> UNIT_POLYGON_POINTS = new List<Vector2>() { new Vector2(0, 0), new Vector2(0.5f * RubedoEngine.SizeOfMeter, 0.866f * RubedoEngine.SizeOfMeter), new Vector2(1f * RubedoEngine.SizeOfMeter, 0f) };

    public readonly IColliderShape shape;
    private Transform prevTransform = null;

    public bool IsTrigger => isTrigger;
    private bool isTrigger = false;

    private Collider(Transform transform, float radius) : base(true, true)
    {
        shape = new CircleShape(transform, radius);
        isTrigger = false;
    }
    private Collider(Transform transform, ShapeType type, float r1, float r2) : base(true, true)
    {
        isTrigger = false;
        switch (type)
        {
            case ShapeType.Box:
                shape = new BoxShape(transform, r1, r2);
                break;
            case ShapeType.Capsule:
                shape = new CapsuleShape(transform, r1, r2);
                break;
            default:
                break;
        }
    }
    private Collider(Transform transform, List<Vector2> vertices) : base(true, true)
    {
        shape = new PolygonShape(transform, vertices);
        isTrigger = false;
    }

    public static Collider CreateCircle(Transform transform, float radius)
    {
        return new Collider(transform, radius);
    }
    public static Collider CreateBox(Transform transform, float width, float height)
    {
        return new Collider(transform, ShapeType.Box, width, height);
    }
    public static Collider CreateCapsule(Transform transform, float length, float radius)
    {
        return new Collider(transform, ShapeType.Capsule, length, radius);
    }
    public static Collider CreatePolygon(Transform transform, List<Vector2> vertices)
    {
        return new Collider(transform, vertices);
    }

    public static Collider CreateUnitShape(Transform transform, ShapeType type)
    {
        switch (type)
        {
            case ShapeType.Circle:
                return CreateCircle(transform, UNIT_CIRCLE_RADIUS);
            case ShapeType.Box:
                return CreateBox(transform, UNIT_BOX_SIDE, UNIT_BOX_SIDE);
            case ShapeType.Capsule:
                return CreateCapsule(transform, UNIT_CAPSULE_LENGTH, UNIT_CAPSULE_RADIUS);
            case ShapeType.Polygon:
                return CreatePolygon(transform, UNIT_POLYGON_POINTS);
        }
        return null;
    }

    public bool TransformChanged()
    {
        return prevTransform != worldTransform;
    }

    public override void Update()
    {
        base.Update();
        if (prevTransform != worldTransform)
        {
            shape.TransformUpdateRequired = true;
            shape.BoundsUpdateRequired = true;
            prevTransform = Entity.transform;
        }
    }
}