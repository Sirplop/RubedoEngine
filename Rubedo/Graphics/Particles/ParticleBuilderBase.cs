using Rubedo.Graphics.Particles.Modifiers;
using Rubedo.Graphics.Particles.Origins;
using Rubedo.Graphics.Sprites;
using System.Collections.Generic;

namespace Rubedo.Graphics.Particles;

/// <summary>
/// TODO: I am ParticleBuilderBase, and I don't have a summary yet.
/// </summary>
public abstract class ParticleBuilderBase<T> where T : ParticleBuilderBase<T> //for pseudo-covariant return types
{
    protected TextureRegion2D texture;
    protected string name;
    protected float particlesPerSecond;
    protected float linearDamping = 0;
    protected float gravityScale = 0;
    protected Origin origin = new PointOrigin();
    protected bool destroyOnNoParticles = false;
    protected int renderLayer = 0;

    protected Interval speed = new Interval(5, 10);
    protected Interval maxAge = new Interval(1000, 1000);
    protected Interval direction = new Interval(-System.Math.PI, System.Math.PI);
    protected Interval rotation = new Interval(-System.Math.PI, System.Math.PI);
    protected Interval av = new Interval(-0.1f, 0.1f);

    protected readonly List<Modifier> modifiers;
    protected readonly List<BirthModifier> birthModifiers;

    public ParticleBuilderBase(string name, float particlesPerSecond, bool destroyOnNoParticles)
    {
        this.name = name;
        this.particlesPerSecond = particlesPerSecond;
        this.destroyOnNoParticles = destroyOnNoParticles;

        modifiers = new List<Modifier>();
        birthModifiers = new List<BirthModifier>();
    }

    public T SetName(string name)
    {
        this.name = name;
        return (T)this;
    }
    public T SetParticleRate(float rate)
    {
        particlesPerSecond = rate;
        return (T)this;
    }

    public T AddModifier(Modifier modifier)
    {
        modifiers.Add(modifier);
        return (T)this;
    }
    public T AddBirthModifier(BirthModifier modifier)
    {
        birthModifiers.Add(modifier);
        return (T)this;
    }

    public T ClearModifiers()
    {
        modifiers.Clear();
        return (T)this;
    }
    public T ClearBirthModifiers()
    {
        birthModifiers.Clear();
        return (T)this;
    }

    public T SetOrigin(Origin origin)
    {
        this.origin = origin;
        return (T)this;
    }

    public T SetGravity(float gravity)
    {
        gravityScale = gravity;
        return (T)this;
    }
    public T SetLinearDamping(float damping)
    {
        linearDamping = damping;
        return (T)this;
    }

    public T SetSpeed(Interval interval)
    {
        speed = interval;
        return (T)this;
    }
    public T SetSpeed(float min, float max)
    {
        speed = new Interval(min, max);
        return (T)this;
    }

    /// <summary>
    /// Interval is in radians.
    /// </summary>
    public T SetRotation(Interval interval)
    {
        rotation = interval;
        return (T)this;
    }
    /// <summary>
    /// Interval is in radians.
    /// </summary>
    public T SetRotation(float min, float max)
    {
        rotation = new Interval(min, max);
        return (T)this;
    }
    public T SetRotationDegrees(float min, float max)
    {
        rotation = new Interval(Lib.Math.ToDegrees(min), Lib.Math.ToDegrees(max));
        return (T)this;
    }

    /// <summary>
    /// Interval is in radians.
    /// </summary>
    public T SetAngularVelocity(Interval interval)
    {
        av = interval;
        return (T)this;
    }
    /// <summary>
    /// Interval is in radians.
    /// </summary>
    public T SetAngularVelocity(float min, float max)
    {
        av = new Interval(min, max);
        return (T)this;
    }
    public T SetAngularVelocityDegrees(float min, float max)
    {
        av = new Interval(Lib.Math.ToDegrees(min), Lib.Math.ToDegrees(max));
        return (T)this;
    }

    /// <summary>
    /// Interval is in radians.
    /// </summary>
    public T SetDirection(Interval interval)
    {
        direction = interval;
        return (T)this;
    }
    /// <summary>
    /// Interval is in radians.
    /// </summary>
    public T SetDirection(double min, double max)
    {
        direction = new Interval(min, max);
        return (T)this;
    }
    /// <summary>
    /// Interval is in degrees.
    /// </summary>
    public T SetDirectionDegrees(float min, float max)
    {
        direction = new Interval(Lib.Math.ToDegrees(min), Lib.Math.ToDegrees(max));
        return (T)this;
    }

    public T SetMaxAge(Interval interval)
    {
        maxAge = interval;
        return (T)this;
    }
    public T SetMaxAge(float min, float max)
    {
        maxAge = new Interval(min, max);
        return (T)this;
    }
    public T SetTexture(TextureRegion2D texture, int renderLayer)
    {
        this.texture = texture;
        this.renderLayer = renderLayer;

        return (T)this;
    }
}