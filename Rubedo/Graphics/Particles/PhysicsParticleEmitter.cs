using Microsoft.Xna.Framework;
using Rubedo;
using Rubedo.Graphics.Particles.Modifiers;
using Rubedo.Graphics.Particles.Origins;
using Rubedo.Lib.Extensions;
using Rubedo.Physics2D.Collision;
using Rubedo.Physics2D.Common;
using Rubedo.Physics2D.Dynamics;
using Rubedo.Physics2D.Dynamics.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rubedo.Graphics.Particles;
public enum ContactAction { IGNORE, COLLIDE, DESTROY }

public class PhysicsParticleEmitter : Emitter
{
    private readonly Shape shape;

    public delegate ContactAction OnCollisionEventHandler(PhysicsParticle sender, PhysicsBody other, Manifold m);

    internal OnCollisionEventHandler onCollisionEventHandler;
    public event OnCollisionEventHandler OnCollision
    {
        add { onCollisionEventHandler += value; }
        remove { onCollisionEventHandler -= value; }
    }

    public bool TriggerColliders { get; private set; }
    public byte PhysicsLayer { get; private set; }
    public PhysicsMaterial Material { get; set; }

    public PhysicsParticleEmitter(string name, Shape shape, PhysicsMaterial material, Interval speed, Interval direction, float particlesPerSecond, Interval maxAge, bool triggerColliders, byte physicsLayer = 0)
    {
        Name = name;
        Material = material;
        this.speed = speed;
        this.maxAge = maxAge;
        this.direction = direction;
        ParticlesPerSecond = particlesPerSecond;
        this.shape = shape;
        TriggerColliders = triggerColliders;
        Modifiers = new List<Modifier>();
        BirthModifiers = new List<BirthModifier>();
        Particles = new List<IParticle>(100);
        PhysicsLayer = physicsLayer;
    }

    public override void AddBirthModifier(BirthModifier modifier)
    {
        if (!modifier.SupportsPhysics) 
            throw new ArgumentException("Modifier does not support physics", modifier.GetType().Name);
        base.AddBirthModifier(modifier);
    }

    public override void AddModifier(Modifier modifier)
    {
        if (!modifier.SupportsPhysics) 
            throw new ArgumentException("Modifier does not support physics", modifier.GetType().Name);
        base.AddModifier(modifier);
    }

    public override void Update() { }

    public override void FixedUpdate()
    {
        UpdateHelper(Time.FixedDeltaTime);

        if (state == EmitterState.STARTED || state == EmitterState.STOPPING)
        {
            releaseTime += Time.FixedDeltaTime;

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

        double milliseconds = Time.FixedDeltaTime * 1000;
        TotalSeconds += Time.FixedDeltaTime;

        foreach (IParticle p in Particles.ToArray())
        {
            p.Age += milliseconds;
            foreach (Modifier m in Modifiers)
            {
                m.Execute(this, Time.FixedDeltaTime, p);
            }
        }

        for (int i = Particles.Count - 1; i >= 0; i--)
        {
            PhysicsParticle p = (PhysicsParticle)Particles[i];
            if (p.Age > p.MaxAge)
            {
                Particles.SwapAndRemove(i);
                RubedoEngine.Instance.World.RemoveBody(p.Body);
            }
        }

        if (CanDestroy() && DestroyOnNoParticles)
        {
            Stop();
            this.Destroy();
        }
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

    public void AddParticle()
    {
        OriginData data = Origin.GetPosition(this);
        if (data != null)
        {
            PhysicsParticle particle = new PhysicsParticle(this, Material, shape, Transform.Position + data.Position);

            Matrix matrix = Matrix.CreateRotationZ((float)direction.GetValue());

            if (Origin.UseColorData) particle.Color = data.Color;

            particle.Velocity = new Vector2((float)speed.GetValue(), 0);
            particle.Velocity = Vector2.Transform(particle.Velocity, matrix);
            particle.Transform.Rotation = (float)rotation.GetValue();
            particle.AngularVelocity = (float)av.GetValue();
            particle.MaxAge = maxAge.GetValue();
            particle.GravityScale = GravityScale;
            particle.OnCollision += Particle_OnCollision;
            particle.Texture = Texture;
            foreach (BirthModifier m in BirthModifiers)
                m.Execute(this, particle);

            RubedoEngine.Instance.World.AddBody(particle.Body);
            Particles.Add(particle);                
        }
    }

    private ContactAction Particle_OnCollision(PhysicsParticle sender, PhysicsBody other, Manifold m)
    {
        ContactAction action = ContactAction.COLLIDE;
        if (onCollisionEventHandler != null)
        {
            action = onCollisionEventHandler(sender, other, m);
        }
        if (action == ContactAction.DESTROY)
        {
            sender.Age = sender.MaxAge;
            return ContactAction.IGNORE;
        }
        //Let the other object determine if it wants to collide
        return action;
    }
}
