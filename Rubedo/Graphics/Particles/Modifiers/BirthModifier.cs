using System;
using System.Collections.Generic;
using System.Text;

namespace Rubedo.Graphics.Particles.Modifiers;

public abstract class BirthModifier
{
    public abstract void Execute(Emitter e, IParticle p);
    public abstract bool SupportsPhysics { get; }
}
