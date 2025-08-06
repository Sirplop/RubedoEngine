using System;
using System.Collections.Generic;
using System.Text;

namespace Rubedo.Graphics.Particles.Modifiers;

public class ScaleBirthModifier : BirthModifier
{
    private readonly Interval _interval;

    public override bool SupportsPhysics { get => false; }

    public ScaleBirthModifier(Interval scales)
    {
        _interval = scales;
    }

    public override void Execute(Emitter e, IParticle p)
    {
        if (p is Particle particle)
        {
            particle.Transform.Scale = new Microsoft.Xna.Framework.Vector2((float)_interval.GetValue());
        }
    }
}
