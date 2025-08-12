using Rubedo.Physics2D.Common;
using Rubedo.Physics2D.Dynamics.Shapes;
using System.Collections.Generic;
using static Rubedo.Graphics.Particles.ParticleEmitter;

namespace Rubedo.Graphics.Particles;

/// <summary>
/// TODO: I am PhysicsParticleBuilder, and I don't have a summary yet.
/// </summary>
public class PhysicsParticleBuilder : ParticleBuilderBase<PhysicsParticleBuilder>
{
    private readonly List<PhysicsParticleEmitter.OnCollisionEventHandler> particleCollisionEventHandlers;
    private bool triggerColliders = false;
    private byte physicsLayer = 0;
    private PhysicsMaterial material;
    private Shape physicsShape;

    public PhysicsParticleBuilder(string name, float particlesPerSecond, bool destroyOnNoParticles) : base(name, particlesPerSecond, destroyOnNoParticles)
    {
        particleCollisionEventHandlers = new List<PhysicsParticleEmitter.OnCollisionEventHandler>();
    }

    public PhysicsParticleEmitter Build()
    {
        PhysicsParticleEmitter emitter = new PhysicsParticleEmitter(name, physicsShape, material, speed, direction, particlesPerSecond, maxAge, triggerColliders, physicsLayer);
        emitter.Modifiers.AddRange(modifiers);
        emitter.BirthModifiers.AddRange(birthModifiers);
        emitter.GravityScale = gravityScale;
        emitter.av = av;
        emitter.Texture = texture;
        emitter.LinearDamping = linearDamping;
        emitter.Origin = origin;
        emitter.RenderLayer = renderLayer;
        emitter.DestroyOnNoParticles = destroyOnNoParticles;
        emitter.rotation = rotation;

        for (int i = 0; i < particleCollisionEventHandlers.Count; i++)
            emitter.OnCollision += particleCollisionEventHandlers[i];

        return emitter;
    }

    public PhysicsParticleEmitter BuildAndStart()
    {
        PhysicsParticleEmitter emitter = Build();
        emitter.Start();
        return emitter;
    }

    public PhysicsParticleBuilder SetShape(Shape shape)
    {
        physicsShape = shape;
        return this;
    }

    public PhysicsParticleBuilder SetPhysicsLayer(byte layer)
    {
        physicsLayer = layer;
        return this;
    }
    
    public PhysicsParticleBuilder SetMaterial(PhysicsMaterial material)
    {
        this.material = material;
        return this;
    }

    public PhysicsParticleBuilder SetIsTriggerCollider(bool itSureIs)
    {
        this.triggerColliders = itSureIs;
        return this;
    }

    public PhysicsParticleBuilder AddCollisionEvent(PhysicsParticleEmitter.OnCollisionEventHandler collisionEvent)
    {
        particleCollisionEventHandlers.Add(collisionEvent);
        return this;
    }
    public PhysicsParticleBuilder ClearCollisionEvents()
    {
        particleCollisionEventHandlers.Clear();
        return this;
    }
}