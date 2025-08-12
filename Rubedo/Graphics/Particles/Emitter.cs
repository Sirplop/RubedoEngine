using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rubedo;
using Rubedo.Components;
using Rubedo.Graphics.Particles.Modifiers;
using Rubedo.Graphics.Particles.Origins;
using Rubedo.Graphics.Sprites;
using Rubedo.Lib;
using System.Collections.Generic;

namespace Rubedo.Graphics.Particles;

public abstract class Emitter : RenderableComponent
{
    public enum EmitterState { INIT, BURST, STARTED, STOPPING, STOPPED }

    public List<IParticle> Particles { get; set; }
    public TextureRegion2D Texture { get; set; }
    public string Name { get; set; }
    public float ParticlesPerSecond;
    public float LinearDamping { get; set; }
    public float GravityScale { get; set; } = 0;
    public Origin Origin { get; set; } = new PointOrigin();
    public double TotalSeconds { get; set; }
    public bool DestroyOnNoParticles { get; set; }

    protected internal Interval speed;
    protected internal List<Modifier> Modifiers { get; set; }
    protected internal List<BirthModifier> BirthModifiers { get; set; }

    public override RectF Bounds
    {
        get
        {
            if (_boundsDirty)
            {
                _boundsDirty = false;
                _bounds = new RectF(0, 0, 0, 0);
                for (int i = 0; i < Particles.Count; i++) 
                {
                    IParticle particle = Particles[i];
                    Vector2 position = particle.Transform.Position;
                    Vector2 scale = particle.Transform.Scale;
                    float rotation = particle.Transform.Rotation;

                    //particles always have a pivot of 0.5.
                    float halfWidth = particle.Texture.Width * 0.5f;
                    float halfHeight = particle.Texture.Height * 0.5f;

                    Vector2 a = MathV.RotateRadians(new Vector2(-halfWidth, halfHeight), rotation, scale.X, scale.Y) + position;
                    Vector2 b = MathV.RotateRadians(new Vector2(halfWidth, halfHeight), rotation, scale.X, scale.Y) + position;
                    Vector2 c = MathV.RotateRadians(new Vector2(-halfWidth, -halfHeight), rotation, scale.X, scale.Y) + position;
                    Vector2 d = MathV.RotateRadians(new Vector2(halfWidth, -halfHeight), rotation, scale.X, scale.Y) + position;

                    float left = Math.Min(_bounds.x, a.X, b.X, c.X, d.X);
                    float right = Math.Max(_bounds.x + _bounds.width, a.X, b.X, c.X, d.X);
                    float top = Math.Min(_bounds.y, a.Y, b.Y, c.Y, d.Y);
                    float bottom = Math.Max(_bounds.y + _bounds.height, a.Y, b.Y, c.Y, d.Y);

                    _bounds.x = left;
                    _bounds.y = top;
                    _bounds.width = right - left;
                    _bounds.height = bottom - top;
                }
            }
            return _bounds;
        }
    }
    private RectF _bounds;
    protected bool _boundsDirty = true;

    protected internal Interval maxAge;
    protected internal EmitterState state = EmitterState.INIT;
    protected double releaseTime = 0;
    protected internal Interval direction;
    protected internal Interval rotation = new Interval(-System.Math.PI, System.Math.PI);
    protected internal Interval av = new Interval(-0.1f, 0.1f);

    protected double _stopTime;
    protected float _stopCount;

    public virtual void AddModifier(Modifier modifier)
    {
        Modifiers.Add(modifier);
    }

    public virtual void AddBirthModifier(BirthModifier modifier)
    {
        BirthModifiers.Add(modifier);
    }

    public void Start()
    {
        releaseTime = 0;
        state = EmitterState.STARTED;
    }

    public void Stop()
    {
        if (state == EmitterState.STARTED)
        {
            state = EmitterState.STOPPING;
            _stopCount = ParticlesPerSecond;
        }
    }

    public abstract void PlayBurst(int particleCount);

    public bool CanDestroy()
    {
        return Particles.Count == 0 && state == EmitterState.STOPPED;
    }

    public override void Update()
    {
        UpdateHelper(Time.DeltaTime);
    }

    public void UpdateHelper(float deltaTime)
    {
        if (state == EmitterState.STOPPING)
        {
            _stopTime += deltaTime;
            ParticlesPerSecond = MathHelper.SmoothStep(_stopCount, 0, (float)_stopTime);
            if (ParticlesPerSecond <= 0)
            {
                state = EmitterState.STOPPED;
            }
        }
    }

    public override void Render(Renderer renderer, Camera camera)
    {
        foreach (IParticle p in Particles)
        {
            renderer.Draw(p.Texture, p.Transform, null, p.Color * p.Alpha, new Vector2(Texture.Width * 0.5f, Texture.Height * 0.5f), SpriteEffects.None, _layerDepth);
        }
    }
}
