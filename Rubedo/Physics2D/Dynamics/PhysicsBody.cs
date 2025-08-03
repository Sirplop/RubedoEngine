using Microsoft.Xna.Framework;
using Rubedo.Components;
using Rubedo.Lib;
using Rubedo.Physics2D.Collision;
using Rubedo.Physics2D.Common;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Rubedo.Physics2D.Dynamics;

/// <summary>
/// I am PhysicsBody, and this is my summary.
/// </summary>
public class PhysicsBody : Component
{
    /*private enum CollisionType { Solid, TriggerSolid, Trigger }
    internal ConcurrentDictionary<PhysicsBody, Manifold> collidingWith = new ConcurrentDictionary<PhysicsBody, Manifold>();
    internal ConcurrentDictionary<PhysicsBody, Manifold> newCollisions = new ConcurrentDictionary<PhysicsBody, Manifold>();
    internal ConcurrentQueue<PhysicsBody> removeCollisions = new ConcurrentQueue<PhysicsBody>();*/

    internal Vector2 velocity = Vector2.Zero;
    internal float angularVelocity = 0;

    private Vector2 force;
    private float torque;

    public AABB bounds;

    public float Mass => _mass;
    internal float _mass;
    public float InvMass => _invMass;
    internal float _invMass;
    public float Inertia => _inertia;
    internal float _inertia;
    public float InvInertia => _invInertia;
    internal float _invInertia;

    public PhysicsMaterial material;

    public bool isStatic = false;
    public float gravityScale = 1;

    public readonly Collider collider; //TODO: Handle linked colliders better.

    public Vector2 Position => Entity.Transform.Position;

    public Vector2 LinearVelocity { get => velocity; internal set => velocity = value; }
    public float AngularVelocity { get => angularVelocity; internal set => angularVelocity = value; }

    public PhysicsBody(Collider collider, PhysicsMaterial material) : base()
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

        Vector2 pos = Entity.Transform.Position;
        MathV.MulAdd(ref pos, ref velocity, dt, out pos);
        Entity.Transform.SetPosition(ref pos);
        Entity.Transform.Rotation += angularVelocity * dt;
    }

    public override void Update()
    {
        base.Update();
        //Update bounds for Broad phase
        bounds = collider.shape.GetBoundingBox();
    }

    public override void EntityRemoved(GameState state)
    {
        RubedoEngine.Instance.World.RemoveBody(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void ApplyImpulseA(ref Vector2 impulse, ref Vector2 contactRadius)
    {
        MathV.MulSub(ref velocity, ref impulse, in _invMass, out velocity);
        MathV.Cross(ref contactRadius, ref impulse, out float lambda);
        angularVelocity -= _invInertia * lambda;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void ApplyImpulseB(ref Vector2 impulse, ref Vector2 contactRadius)
    {
        MathV.MulAdd(ref velocity, ref impulse, in _invMass, out velocity);
        MathV.Cross(ref contactRadius, ref impulse, out float lambda);
        angularVelocity += _invInertia * lambda;
    }


    /*internal void RemoveAllContacts()
    {
        if (collidingWith.IsEmpty)
            return;
        foreach (PhysicsBody body in collidingWith.Keys)
        {
            removeCollisions.Enqueue(body);
        }
    }
    internal void DoCollision(PhysicsBody other)
    {
        Manifold contact = collidingWith[other];
        if (newCollisions.ContainsKey(other))
        {
            switch (GetCollisionType(contact))
            {
                case CollisionType.Solid:
                    contact.B.collider.onCollisionEnterEventHandler?.Invoke(contact.B, this, contact);
                    collider.onCollisionEnterEventHandler?.Invoke(this, contact.B, contact);
                    break;
                case CollisionType.TriggerSolid:
                    if (collider.isTrigger)
                        collider.onTriggerEnterEventHandler?.Invoke(this, contact.B, contact);
                    else
                        contact.B.collider.onTriggerEnterEventHandler?.Invoke(contact.B, this, contact);
                    break;
                case CollisionType.Trigger:
                    contact.B.collider.onTriggerEnterEventHandler?.Invoke(contact.B, this, contact);
                    collider.onTriggerEnterEventHandler?.Invoke(this, contact.B, contact);
                    break;
            }
            return;
        }
        switch (GetCollisionType(contact))
        {
            case CollisionType.Solid:
                contact.B.collider.onCollisionStayEventHandler?.Invoke(contact.B, this, contact);
                collider.onCollisionStayEventHandler?.Invoke(this, contact.B, contact);
                break;
            case CollisionType.TriggerSolid:
                if (collider.isTrigger)
                    collider.onTriggerStayEventHandler?.Invoke(this, contact.B, contact);
                else
                    contact.B.collider.onTriggerStayEventHandler?.Invoke(contact.B, this, contact);
                break;
            case CollisionType.Trigger:
                contact.B.collider.onTriggerStayEventHandler?.Invoke(contact.B, this, contact);
                collider.onTriggerStayEventHandler?.Invoke(this, contact.B, contact);
                break;
        }
    }
    internal void FinalizeCollisions()
    {
        foreach (PhysicsBody body in removeCollisions)
        {
            collidingWith.Remove(body, out Manifold contact);
            if (newCollisions.ContainsKey(body))
                continue; //new collision to ignore.
            if (contact == null)
                continue;
            switch (GetCollisionType(contact))
            {
                case CollisionType.Solid:
                    contact.B.collider.onCollisionExitEventHandler?.Invoke(contact.B, this);
                    collider.onCollisionExitEventHandler?.Invoke(this, contact.B);
                    break;
                case CollisionType.TriggerSolid:
                    if (collider.isTrigger)
                        collider.onTriggerExitEventHandler?.Invoke(this, contact.B);
                    else
                        contact.B.collider.onTriggerExitEventHandler?.Invoke(contact.B, this);
                    break;
                case CollisionType.Trigger:
                    contact.B.collider.onTriggerExitEventHandler?.Invoke(contact.B, this);
                    collider.onTriggerExitEventHandler?.Invoke(this, contact.B);
                    break;
            }
        }
        newCollisions.Clear();
        removeCollisions.Clear();
    }*/

    /**
     * Each collision type falls into 1 of 3 categories:
     * - solid collider <-> solid collider
     * - trigger collider <- solid collider
     * - trigger collider <-> trigger collider
     * trigger/solid collisions will only trigger the trigger collider's events.
    **/
    /*private static CollisionType GetCollisionType(Manifold m)
    {
        if (m.A.collider.isTrigger && m.B.collider.isTrigger)
            return CollisionType.Trigger;
        if (m.A.collider.isTrigger || m.B.collider.isTrigger)
            return CollisionType.TriggerSolid;
        return CollisionType.Solid;
    }*/
}