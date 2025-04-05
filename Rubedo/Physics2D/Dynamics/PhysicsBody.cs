using Microsoft.Xna.Framework;
using Rubedo.Components;
using Rubedo.Object;

namespace Rubedo.Physics2D.Dynamics;

/// <summary>
/// I am PhysicsObject, and this is my summary.
/// </summary>
public class PhysicsBody : Component
{
    private Vector2 velocity = Vector2.Zero;
    private float angularVelocity = 0;

    private Vector2 force;
    private float torque;

    public float mass => _mass;
    private float _mass;
    public float invMass => _invMass;
    private float _invMass;
    public float inertia => _inertia;
    private float _inertia;
    public float invInertia => _invInertia;
    private float _invInertia;

    public PhysicsMaterial material;

    public bool isStatic = false;
    public float gravityScale = 1;

    public readonly Collider collider;

    public Vector2 LinearVelocity { get => velocity; internal set => velocity = value; }

    public float AngularVelocity { get => angularVelocity; internal set => angularVelocity = value; }

    public PhysicsBody(Collider collider, PhysicsMaterial material,
        bool active = true, bool visible = true) : base(active, visible)
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

    #region Internal
    internal void IntegrateForces(float deltaTime)
    {
        if (isStatic)
            return;
        velocity += deltaTime * invMass * Lib.Math.MulAdd(force, _mass * gravityScale, RubedoEngine.Instance.World.gravity);
        angularVelocity += +deltaTime * invInertia * torque;

        velocity *= 1f / (1f + deltaTime * material.linearDamping);
        angularVelocity *= 1f / (1f + deltaTime * material.angularDamping);
    }
    internal void IntegrateVelocities(float deltaTime)
    {
        if (isStatic)
            return;
        Entity.transform.Position += velocity * deltaTime;
        Entity.transform.Rotation += angularVelocity * deltaTime;
    }
    internal void ApplyImpulse(Vector2 impulse, Vector2 contactVector)
    {
        if (isStatic)
            return;
        velocity += impulse * _invMass;
        angularVelocity += _invInertia * Lib.Math.Cross(contactVector, impulse);
    }
    #endregion
    #region Public Methods
    public void SetStatic()
    {
        isStatic = !isStatic;

        if (isStatic)
        {
            _invMass = 0f;
            _invInertia = 0f;
        }
        else
        {
            _invMass = 1f / _mass;
            _invInertia = 1f / _inertia;
        }
    }
    public void AddForce(Vector2 force)
    {
        if (isStatic)
            return;
        this.force += force;
    }
    public void AddTorque(float force)
    {
        if (isStatic)
            return;
        torque += force;
    }

    public void SetVelocity(Vector2 val)
    {
        if (isStatic)
            return;
        velocity = val;
    }
    public void SetAngularVelocity(float val)
    {
        if (isStatic)
            return;
        angularVelocity = val;
    }
    public void ClampVelocity(float max)
    {
        if (isStatic)
            return;
        float sqrMag = velocity.LengthSquared();
        if (max * max < sqrMag)
            velocity = Lib.Math.Normalize(velocity) * max;
    }
    public void ClampAngularVelocity(float max)
    {
        if (isStatic)
            return;
        if (max < angularVelocity)
            angularVelocity = max;
    }
    #endregion
}