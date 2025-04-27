using Microsoft.Xna.Framework;
using PhysicsEngine2D;
using Rubedo.Components;
using Rubedo.Lib;

namespace Rubedo.Physics2D.Dynamics;

/// <summary>
/// I am PhysicsBody, and this is my summary.
/// </summary>
public class PhysicsBody : Component
{
    internal Vector2 velocity = Vector2.Zero;
    internal float angularVelocity = 0;

    private Vector2 force;
    private float torque;

    public AABB bounds;

    public float mass => _mass;
    internal float _mass;
    public float invMass => _invMass;
    internal float _invMass;
    public float inertia => _inertia;
    internal float _inertia;
    public float invInertia => _invInertia;
    internal float _invInertia;

    public PhysicsMaterial material;

    public bool isStatic = false;
    public float gravityScale = 1;

    public readonly Collider collider; //TODO: Handle linked colliders better.

    // For use in broadphase
    public object data;

    public Vector2 position => compTransform.Position;

    public Vector2 LinearVelocity { get => velocity; internal set => velocity = value; }
    public float AngularVelocity { get => angularVelocity; internal set => angularVelocity = value; }

    public PhysicsBody(Collider collider, PhysicsMaterial material, bool active, bool visible) : base(active, visible)
    {
        this.material = material;
        this.collider = collider;
        force = Vector2.Zero;
        torque = 0;

        _mass = collider.shape.GetArea() * material.density;
        _inertia = collider.shape.GetMomentOfInertia(_mass);

        _invMass = 1f / _mass;
        _invInertia = 1f / _inertia;
    }

    public void SetStatic()
    {
        _invMass = 0;
        _invInertia = 0;
        isStatic = true;
    }

    internal void IntegrateForces(float dt)
    {
        if (_invMass == 0)
            return;

        MathV.MulAdd(ref velocity, ref force, dt * _invMass, out velocity);
        angularVelocity += dt * torque * _invInertia;

        Vector2.Multiply(ref velocity, 1f / (1f + dt * material.linearDamping), out velocity);
        angularVelocity *= 1f / (1f + dt * material.angularDamping);

        //apply gravity after so we don't have any weird drag.
        MathV.MulAdd(ref velocity, ref PhysicsWorld.gravity, gravityScale * dt, out velocity);

        force = Vector2.Zero;
        torque = 0;
    }

    internal void IntegrateVelocity(float dt)
    {
        if (_invMass == 0)
            return;

        Vector2 pos = Entity.transform.Position;
        MathV.MulAdd(ref pos, ref velocity, dt, out pos);
        Entity.transform.SetPosition(ref pos);
        Entity.transform.Rotation += angularVelocity * dt;
    }

    public override void Update()
    {
        base.Update();
        //Update bounds for Broad phase
        bounds = collider.shape.GetBoundingBox();
    }

    public override void EntityRemoved(GameState state)
    {
        base.EntityRemoved(state);
        RubedoEngine.Instance.World.RemoveBody(this);
    }

    internal void ApplyImpulseA(ref Vector2 impulse, ref Vector2 contactRadius)
    {
        MathV.MulSub(ref velocity, ref impulse, invMass, out velocity);
        MathV.Cross(ref contactRadius, ref impulse, out float lambda);
        angularVelocity -= invInertia * lambda;
    }
    internal void ApplyImpulseB(ref Vector2 impulse, ref Vector2 contactRadius)
    {
        MathV.MulAdd(ref velocity, ref impulse, invMass, out velocity);
        MathV.Cross(ref contactRadius, ref impulse, out float lambda);
        angularVelocity += invInertia * lambda;
    }
}