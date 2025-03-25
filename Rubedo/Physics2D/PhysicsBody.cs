using Microsoft.Xna.Framework;
using Rubedo.Components;
using Rubedo.Object;
using Rubedo.Physics2D.ColliderShape;

namespace Rubedo.Physics2D;

/// <summary>
/// I am PhysicsObject, and this is my summary.
/// </summary>
public class PhysicsBody : Component
{
    private Vector2 velocity = Vector2.Zero;
    private float angularVelocity = 0;

    private Vector2 force;

    public readonly float density;
    public readonly float mass;
    public readonly float invMass;
    public readonly float restitution;
    public readonly float inertia;
    public readonly float invInertia;

    public bool isStatic = false;

    public readonly Collider collider;

    public Vector2 LinearVelocity { get => velocity; internal set => velocity = value; }
    
    public float AngularVelocity { get => angularVelocity; internal set => angularVelocity = value; }

    public PhysicsBody(Collider collider, Vector2 position, float rotation, Vector2 scale, float density, float restitution, bool isStatic = true, bool active = true, bool visible = true) : base(active, visible)
    {
        localTransform = new Transform(position, rotation, scale);
        this.density = density;
        this.mass = collider.shape.GetArea() * density;
        this.invMass = 1f / mass;
        this.restitution = restitution;
        this.isStatic = isStatic;
        this.collider = collider;
        this.force = Vector2.Zero;

        if (isStatic)
        {
            this.invMass = 0f;
        }
        else
        {
            this.invMass = 1f / mass;
        }

        this.inertia = collider.shape.GetMomentOfInertia(mass);
        this.invInertia = 1f / inertia;
    }
    public PhysicsBody(Collider collider, Transform transform, float density, float restitution, bool isStatic = true, bool active = true, bool visible = true) : base(active, visible)
    {
        localTransform = transform;
        this.density = density;
        this.restitution = restitution;
        this.isStatic = isStatic;
        this.collider = collider;
        this.force = Vector2.Zero;
        this.mass = collider.shape.GetArea() * density;
        this.inertia = collider.shape.GetMomentOfInertia(mass);

        if (isStatic)
        {
            this.invMass = 0f;
            this.invInertia = 0f;
        }
        else
        {
            this.invMass = 1f / mass;
            this.invInertia = 1f / inertia;
        }
    }

    internal void Step(float deltaTime)
    {
        if (isStatic)
            return;

        //force = mass * acceleration
        //velocity += force * oneOverMass;
        velocity += RubedoEngine.Instance.World.gravity * deltaTime;

        Entity.transform.Position += velocity * deltaTime;
        Entity.transform.Rotation += angularVelocity * deltaTime;

        force = Vector2.Zero;
        collider.shape.TransformUpdateRequired = true;
        collider.shape.BoundsUpdateRequired = true;
    }

    public void AddForce(Vector2 force)
    {
        if (!isStatic)
            this.force += force;
    }

    public void Move(Vector2 move)
    {
        Entity.transform.Position += move;
    }
}