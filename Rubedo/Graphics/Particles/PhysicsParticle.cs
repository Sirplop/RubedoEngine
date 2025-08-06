using Microsoft.Xna.Framework;
using Rubedo.Graphics.Sprites;
using Rubedo.Object;
using Rubedo.Physics2D.Collision;
using Rubedo.Physics2D.Common;
using Rubedo.Physics2D.Dynamics;
using Rubedo.Physics2D.Dynamics.Shapes;
using System;
using System.Collections.Generic;

namespace Rubedo.Graphics.Particles;

public class PhysicsParticle : IParticle
{
    public Transform Transform { get; set; }
    public double Age { get; set; }
    public double MaxAge { get; set; }
    public float Alpha { get; set; } = 1.0f;
    public Color Color { get; set; } = Color.White;
    public TextureRegion2D Texture { get; set; }
    public PhysicsBody Body { get; set; }
    public bool IsParticle { get => Body.IsParticle; set => throw new NotImplementedException(); }
    public Vector2 Velocity { get => Body.LinearVelocity; set => Body.LinearVelocity = value; }
    public float AngularVelocity { get => Body.AngularVelocity; set => Body.AngularVelocity = value; }
    public float LinearDamping { get => Body.material.linearDamping; }
    public float GravityScale { get => Body.gravityScale; set => Body.gravityScale = value; }

    public delegate ContactAction OnParticleCollisionEventHandler(PhysicsParticle sender, PhysicsBody other, Manifold m);

    internal OnParticleCollisionEventHandler onParticleCollisionEventHandler;
    public event OnParticleCollisionEventHandler OnCollision
    {
        add { onParticleCollisionEventHandler += value; }
        remove { onParticleCollisionEventHandler -= value; }
    }

    public PhysicsParticle(PhysicsParticleEmitter parent, PhysicsMaterial material, Shape shape, Vector2 pos)
    {
        this.Transform = new Transform(pos);
        Collider collider = Collider.CreateFromShape(shape, parent.TriggerColliders);
        collider.physicsLayer = parent.PhysicsLayer;
        Body = new PhysicsBody(collider, material);
        Body.TargetTransform = Transform;
        Body.gravityScale = parent.GravityScale;
        collider.shape.SetTransform(Transform);
        Transform.attached = collider;
        Body.IsParticle = true;

        if (parent.TriggerColliders)
            Body.collider.OnTriggerEnter += BodyOnCollision;
        else
            Body.collider.OnCollisionEnter += BodyOnCollision;
    }
    private void BodyOnCollision(PhysicsBody sender, PhysicsBody other, Manifold m)
    {
        onParticleCollisionEventHandler?.Invoke(this, other, m);
    }

    public void Reset()
    {
        Texture = null;
    }
}
