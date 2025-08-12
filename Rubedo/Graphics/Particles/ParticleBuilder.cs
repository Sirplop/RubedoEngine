using Rubedo.Graphics.Particles.Modifiers;
using Rubedo.Graphics.Particles.Origins;
using Rubedo.Graphics.Sprites;
using System.Collections.Generic;

namespace Rubedo.Graphics.Particles;

/// <summary>
/// TODO: I am ParticleBuilder, and I don't have a summary yet.
/// </summary>
public class ParticleBuilder : ParticleBuilderBase<ParticleBuilder>
{
    protected readonly List<ParticleEmitter.ParticleDeathEventHandler> particleDeathEventHandlers;

    public ParticleBuilder(string name, float particlesPerSecond, bool destroyOnNoParticles) : base(name, particlesPerSecond, destroyOnNoParticles)
    {
        particleDeathEventHandlers = new List<ParticleEmitter.ParticleDeathEventHandler>();
    }

    public ParticleEmitter Build()
    {
        ParticleEmitter emitter = new ParticleEmitter(name, speed, direction, particlesPerSecond, maxAge);
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

        for (int i = 0; i < particleDeathEventHandlers.Count; i++)
            emitter.ParticleDeath += particleDeathEventHandlers[i];

        return emitter;
    }
    public ParticleEmitter BuildAndStart()
    {
        ParticleEmitter emitter = Build();
        emitter.Start();
        return emitter;
    }

    
    public ParticleBuilder AddDeathEvent(ParticleEmitter.ParticleDeathEventHandler deathEvent)
    {
        particleDeathEventHandlers.Add(deathEvent);
        return this;
    }
    public ParticleBuilder ClearDeathEvents()
    {
        particleDeathEventHandlers.Clear();
        return this;
    }
}