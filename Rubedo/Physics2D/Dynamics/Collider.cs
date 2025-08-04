using Microsoft.Xna.Framework;
using Rubedo.Components;
using System.Collections.Generic;
using System;
using Rubedo.Physics2D.Dynamics.Shapes;
using Rubedo.Object;
using Rubedo.Physics2D.Collision;

namespace Rubedo.Physics2D.Dynamics;

public delegate void OnCollisionEnterEventHandler(PhysicsBody sender, PhysicsBody other, Manifold m);
public delegate void OnCollisionStayEventHandler(PhysicsBody sender, PhysicsBody other, Manifold m);
public delegate void OnCollisionExitEventHandler(PhysicsBody sender, PhysicsBody other);

public delegate void OnTriggerEnterEventHandler(PhysicsBody sender, PhysicsBody other, Manifold m);
public delegate void OnTriggerStayEventHandler(PhysicsBody sender, PhysicsBody other, Manifold m);
public delegate void OnTriggerExitEventHandler(PhysicsBody sender, PhysicsBody other);

/// <summary>
/// A component that allows other colliders to interact with this. Must be attached to a <see cref="PhysicsBody"/>.
/// </summary>
public class Collider : Component
{
    public static float UNIT_CIRCLE_RADIUS => 0.5f * RubedoEngine.SizeOfMeter;
    public static float UNIT_BOX_SIDE => 1f * RubedoEngine.SizeOfMeter;
    public static float UNIT_CAPSULE_LENGTH => 0.334f * RubedoEngine.SizeOfMeter;
    public static float UNIT_CAPSULE_RADIUS => 0.5f * RubedoEngine.SizeOfMeter;

    public readonly Shape shape;

    public bool isTrigger;
    public byte physicsLayer = 0;


    internal OnCollisionEnterEventHandler onCollisionEnterEventHandler;
    public event OnCollisionEnterEventHandler OnCollisionEnter
    {
        add { onCollisionEnterEventHandler += value; }
        remove { onCollisionEnterEventHandler -= value; }
    }
    internal OnCollisionStayEventHandler onCollisionStayEventHandler;
    public event OnCollisionStayEventHandler OnCollisionStay
    {
        add { onCollisionStayEventHandler += value; }
        remove { onCollisionStayEventHandler -= value; }
    }
    internal OnCollisionExitEventHandler onCollisionExitEventHandler;
    public event OnCollisionExitEventHandler OnCollisionExit
    {
        add { onCollisionExitEventHandler += value; }
        remove { onCollisionExitEventHandler -= value; }
    }
    internal OnTriggerEnterEventHandler onTriggerEnterEventHandler;
    public event OnTriggerEnterEventHandler OnTriggerEnter
    {
        add { onTriggerEnterEventHandler += value; }
        remove { onTriggerEnterEventHandler -= value; }
    }
    internal OnTriggerStayEventHandler onTriggerStayEventHandler;
    public event OnTriggerStayEventHandler OnTriggerStay
    {
        add { onTriggerStayEventHandler += value; }
        remove { onTriggerStayEventHandler -= value; }
    }
    internal OnTriggerExitEventHandler onTriggerExitEventHandler;
    public event OnTriggerExitEventHandler OnTriggerExit
    {
        add { onTriggerExitEventHandler += value; }
        remove { onTriggerExitEventHandler -= value; }
    }

    protected Collider(Shape shape, bool isTrigger)
    {
        this.shape = shape;
        this.isTrigger = isTrigger;
    }

    protected Collider(float radius, bool isTrigger) : base()
    {
        shape = new Circle(radius);
        this.isTrigger = isTrigger;
    }
    
    protected Collider(ShapeType type, bool isTrigger, float r1, float r2) : base()
    {
        switch (type)
        {
            case ShapeType.Box:
                shape = new Box(r1, r2);
                break;
            case ShapeType.Capsule:
                shape = new Capsule(r1, r2);
                break;
            default:
                throw new ArgumentException("Given ShapeType does not belong in this constructor!");
        }
        this.isTrigger = isTrigger;
    }
    protected Collider(List<Vector2> vertices, bool isTrigger) : base()
    {
        shape = new Polygon(vertices);
        this.isTrigger = isTrigger;
    }

    public static Collider CreateCircle(float radius, bool isTrigger = false)
    {
        return new Collider(radius, isTrigger);
    }
    public static Collider CreateBox(float width, float height, bool isTrigger = false)
    {
        return new Collider(ShapeType.Box, isTrigger, width, height);
    }
    public static Collider CreateCapsule(float length, float radius, bool isTrigger = false)
    {
        return new Collider(ShapeType.Capsule, isTrigger, length, radius);
    }
    public static Collider CreatePolygon(List<Vector2> vertices, bool isTrigger = false)
    {
        return new Collider(vertices, isTrigger);
    }

    public static Collider CreateUnitShape(ShapeType type, bool isTrigger = false, int polygonOnlySideCount = 3)
    {
        switch (type)
        {
            case ShapeType.Circle:
                return CreateCircle(UNIT_CIRCLE_RADIUS, isTrigger);
            case ShapeType.Box:
                return CreateBox(UNIT_BOX_SIDE, UNIT_BOX_SIDE, isTrigger);
            case ShapeType.Capsule:
                return CreateCapsule(UNIT_CAPSULE_LENGTH, UNIT_CAPSULE_RADIUS, isTrigger);
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
                return CreatePolygon(vertices, isTrigger);
        }
        return null;
    }

    /// <summary>
    /// Creates a collider by cloning the given <paramref name="shape"/>.
    /// </summary>
    /// <param name="shape"></param>
    /// <param name="isTrigger"></param>
    /// <returns></returns>
    public static Collider CreateFromShape(Shape shape, bool isTrigger = false)
    {
        return new Collider(shape.Clone(), isTrigger);
    }

    public override void Added(Entity entity)
    {
        base.Added(entity);
        shape.SetTransform(entity.Transform);
    }
    public override void Removed(Entity entity)
    {
        base.Removed(entity);
        shape.SetTransform(null); //This WILL cause crashes if you try to do stuff without transforms!
    }

    public override void TransformChanged()
    {
        shape.transformDirty = true;
        shape.normalsDirty = true;
        shape.boundsDirty = true;
    }
}