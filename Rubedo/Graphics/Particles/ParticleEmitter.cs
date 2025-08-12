using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rubedo.Lib.Collections;
using Rubedo.Graphics.Particles.Data;
using Rubedo;
using Rubedo.Graphics.Particles.Modifiers;
using Rubedo.Graphics.Particles.Origins;
using Rubedo.Lib.Extensions;

namespace Rubedo.Graphics.Particles;

public class ParticleEmitter : Emitter
{
    public ObjectPool<IParticle> ParticlePool { get; set; }

    public event ParticleDeathEventHandler ParticleDeath;
    public delegate void ParticleDeathEventHandler(object sender, ParticleEventArgs e);

    protected virtual void OnParticleDeath(ParticleEventArgs e)
    {
        ParticleDeathEventHandler handler = ParticleDeath;
        handler?.Invoke(this, e);
    }

    public ParticleEmitter(string name, Interval speed, Interval direction, float particlesPerSecond, Interval maxAge)
    {
        Name = name;
        this.speed = speed;
        this.maxAge = maxAge;
        this.direction = direction;
        ParticlesPerSecond = particlesPerSecond;
        Modifiers = new List<Modifier>();
        BirthModifiers = new List<BirthModifier>();
        Particles = new List<IParticle>(100);     

        ParticlePool = new ObjectPool<IParticle>(50, new ParticlePooledObjectPolicy());
    }

    public new bool CanDestroy()
    {
        return Particles.Count == 0 && (state == EmitterState.STOPPED || state == EmitterState.BURST);
    }

    public override void PlayBurst(int particleCount)
    {
        if (state != EmitterState.STARTED)
        {
            state = EmitterState.BURST; //it's not currently playing.
        }
        for (int i = 0; i < particleCount; i++)
        {
            AddParticle();
        }
    }

    public override void Update()
    {
        base.Update();

        if (state==EmitterState.STARTED || state==EmitterState.STOPPING)
        {
            releaseTime += Time.DeltaTime;

            double release = ParticlesPerSecond * releaseTime;
            if (release > 1)
            {
                int r = (int)Math.Floor(release);
                releaseTime -= r / ParticlesPerSecond;

                for (int i = 0; i < r; i++)
                {
                    AddParticle();
                }
            }
        }
        TotalSeconds += Time.DeltaTime;
        float dampening = Math.Clamp(1.0f - Time.DeltaTime * LinearDamping, 0.0f, 1.0f);

        //parallel might not be a good idea? IDK
        Parallel.For(0, Particles.Count, (i, a) =>
        //for (int i = 0; i < Particles.Count; i++)
        {
            IParticle p = Particles[i];
            p.Age += Time.DeltaTimeMillis;

            if (p.Age > p.MaxAge)
            {
                OnParticleDeath(new ParticleEventArgs(p));
                ParticlePool.Release(p);
            }
            else
            {
                p.Transform.Position += p.Velocity * Time.DeltaTime;
                p.Velocity += Physics2D.Common.PhysicsWorld.gravity * GravityScale * Time.DeltaTime;
                p.Velocity *= dampening;
                p.Transform.Rotation += p.AngularVelocity;

                foreach (Modifier m in Modifiers)
                {
                    m.Execute(this, Time.DeltaTime, p);
                }
            }
        });

        Particles.RemoveAll(p => p.Age > p.MaxAge);
        if (CanDestroy() && DestroyOnNoParticles)
        {
            Stop();
            this.Destroy();
        }
        _boundsDirty = true;
    }

    public void AddParticle()
    {
        OriginData data = Origin.GetPosition(this);

        if (data != null)
        {
            IParticle particle = ParticlePool.Obtain();

            Matrix matrix = Matrix.CreateRotationZ((float)direction.GetValue());

            particle.Velocity = new Vector2((float)speed.GetValue(), 0);
            particle.Velocity = Vector2.Transform(particle.Velocity, matrix);
            particle.Transform.Position = Transform.Position + data.Position;
            if (Origin.UseColorData) particle.Color = data.Color;
            particle.AngularVelocity = (float)av.GetValue();
            particle.Transform.Rotation = (float)rotation.GetValue();
            particle.AngularVelocity = (float)av.GetValue();
            particle.MaxAge = maxAge.GetValue();
            particle.Age = 0;
            particle.Texture = Texture;

            foreach (BirthModifier m in BirthModifiers) m.Execute(this, particle);

            Particles.Add(particle);
        }
    }
}

