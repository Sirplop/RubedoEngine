using Microsoft.Xna.Framework;
using PhysicsEngine2D;
using Rubedo.Components;
using System.Collections.Generic;
using System;
using Rubedo.Physics2D.Collision.Shapes;
using Rubedo.Physics2D.Dynamics.Shapes;

namespace Rubedo.Physics2D.Dynamics;

/// <summary>
/// A component that allows other colliders to interact with this. Use <see cref="PhysicsBody"/> for physical interactions.
/// </summary>
public class Collider : Component
{
    public static float UNIT_CIRCLE_RADIUS => 0.5f * RubedoEngine.SizeOfMeter;
    public static float UNIT_BOX_SIDE => 1f * RubedoEngine.SizeOfMeter;
    public static float UNIT_CAPSULE_LENGTH => 0.334f * RubedoEngine.SizeOfMeter;
    public static float UNIT_CAPSULE_RADIUS => 0.5f * RubedoEngine.SizeOfMeter;

    public readonly Shape shape;
    
    private Vector2 prevPos = Vector2.Zero;
    private float prevAngle = 0f;
    private Vector2 prevScale = Vector2.One;

    protected Collider(float radius) : base(true, true)
    {
        shape = new Circle(transform, radius);
    }
    
    protected Collider(ShapeType type, float r1, float r2) : base(true, true)
    {
        switch (type)
        {
            case ShapeType.Box:
                shape = new Box(transform, r1, r2);
                break;
            case ShapeType.Capsule:
                shape = new Capsule(transform, r1, r2);
                break;
            default:
                throw new ArgumentException("Given ShapeType does not belong in this constructor!");
        }
    }
    protected Collider(List<Vector2> vertices) : base(true, true)
    {
        shape = new Polygon(transform, vertices);
    }

    public static Collider CreateCircle(float radius)
    {
        return new Collider(radius);
    }
    public static Collider CreateBox(float width, float height)
    {
        return new Collider(ShapeType.Box, width, height);
    }
    public static Collider CreateCapsule(float length, float radius)
    {
        return new Collider(ShapeType.Capsule, length, radius);
    }
    public static Collider CreatePolygon(List<Vector2> vertices)
    {
        return new Collider(vertices);
    }

    public static Collider CreateUnitShape(ShapeType type, int polygonOnlySideCount = 3)
    {
        switch (type)
        {
            case ShapeType.Circle:
                return CreateCircle(UNIT_CIRCLE_RADIUS);
            case ShapeType.Box:
                return CreateBox(UNIT_BOX_SIDE, UNIT_BOX_SIDE);
            case ShapeType.Capsule:
                return CreateCapsule(UNIT_CAPSULE_LENGTH, UNIT_CAPSULE_RADIUS);
            case ShapeType.Polygon:
                List<Vector2> vertices = new List<Vector2>();
                polygonOnlySideCount = System.Math.Max(3, polygonOnlySideCount);
                float r = RubedoEngine.SizeOfMeter * 0.5f;
                float a = MathHelper.Pi / (polygonOnlySideCount % 2 == 1 ? 2 : 4); //make sure a side faces down.
                for (int i = 0; i < polygonOnlySideCount; i++)
                {
                    vertices.Add(new Vector2(r * MathF.Cos((MathHelper.TwoPi * i / polygonOnlySideCount) + a),
                        r * MathF.Sin((MathHelper.TwoPi * i / polygonOnlySideCount) + a)));
                }
                return CreatePolygon(vertices);
        }
        return null;
    }
    /*
    public override void Update()
    {
        base.Update();
        if (shape.type == ShapeType.Box ||  shape.type == ShapeType.Polygon)
        {
            Vector2 pos = transform.Position;
            float angle = transform.Rotation;
            Vector2 scale = transform.Scale;
            if (pos != prevPos || angle != prevAngle || scale != prevScale)
            {
                ((Polygon)shape).normalsDirty = true;
                prevPos = pos;
                prevAngle = angle;
                prevScale = scale;
            }
        }
    }*/
}