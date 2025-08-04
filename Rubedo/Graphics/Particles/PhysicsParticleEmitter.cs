using Microsoft.Xna.Framework;
using MonoGame.Particles.Particles.Modifiers;
using MonoGame.Particles.Particles.Origins;
using Rubedo;
using Rubedo.Physics2D;
using Rubedo.Physics2D.Collision;
using Rubedo.Physics2D.Dynamics;
using Rubedo.Physics2D.Dynamics.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonoGame.Particles.Particles;
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
    public PhysicsMaterial Material { get; set; }

    public PhysicsParticleEmitter(String name, Shape shape, PhysicsMaterial material, Interval speed, Interval direction, float particlesPerSecond, Interval maxAge, bool triggerColliders)
    {
        this.Name = name;
        this.Material = material;
        this.speed = speed;
        this.maxAge = maxAge;
        this.direction = direction;
        this.ParticlesPerSecond = particlesPerSecond;
        this.shape = shape;
        this.TriggerColliders = triggerColliders;
        Modifiers = new List<Modifier>();
        BirthModifiers = new List<BirthModifier>();
        Particles = new List<IParticle>(100);
    }

    public override void AddBirthModifier(BirthModifier modifier)
    {
        if (!modifier.SupportsPhysics) throw new ArgumentException("Modifier does not support physics", modifier.GetType().Name);
        base.AddBirthModifier(modifier);
    }

    public override void AddModifier(Modifier modifier)
    {
        if (!modifier.SupportsPhysics) throw new ArgumentException("Modifier does not support physics", modifier.GetType().Name);
        base.AddModifier(modifier);
    }

    public override void FixedUpdate()
    {
        if (state == EmitterState.STOPPING)
        {
            _stopTime += Time.FixedDeltaTime;
            ParticlesPerSecond = MathHelper.SmoothStep(_stopCount, 0, (float)_stopTime);
            if (ParticlesPerSecond <= 0)
            {
                state = EmitterState.STOPPED;
            }
        }

        if (state == EmitterState.STARTED || state == EmitterState.STOPPING)
        {
            releaseTime += Time.FixedDeltaTime;

            double release = ParticlesPerSecond * releaseTime;
            if (release > 1)
            {
                int r = (int)Math.Floor(release);
                releaseTime -= (r / ParticlesPerSecond);

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

        List<IParticle> remove = Particles.FindAll(p => p.Age > p.MaxAge);

        Parallel.Invoke(
            () =>
            {
                Particles.RemoveAll(p => p.Age > p.MaxAge);
            },
            () =>
            {
                foreach (PhysicsParticle p in remove.OfType<PhysicsParticle>()) RubedoEngine.Instance.World.RemoveBody(p.Body);
            }
        );
    }

    public void AddParticle()
    {
        OriginData data = Origin.GetPosition(this);
        if (data != null)
        {
            PhysicsParticle particle = new PhysicsParticle(Material, shape, Transform.Position + data.Position, TriggerColliders);

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
            (sender).Age = (sender).MaxAge;
            return ContactAction.IGNORE;
        }
        //Let the other object determine if it wants to collide
        return action;
    }
}
