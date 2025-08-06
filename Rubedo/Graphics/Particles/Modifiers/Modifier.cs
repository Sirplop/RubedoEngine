using System;
using System.Collections.Generic;
using System.Text;

namespace Rubedo.Graphics.Particles.Modifiers;

public abstract class Modifier
{
    public abstract void Execute(Emitter e, double seconds, IParticle p);
    public abstract bool SupportsPhysics { get; }
}
