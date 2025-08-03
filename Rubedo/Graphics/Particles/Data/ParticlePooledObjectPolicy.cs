using MonoGame.Particles.Particles;
using Rubedo.Lib.Collections;

namespace Rubedo.Graphics.Particles.Data;

/// <summary>
/// Primary policy for pooled particles.
/// </summary>
public class ParticlePooledObjectPolicy : IObjectPoolPolicy<IParticle>
{
    public IParticle Create()
    {
        return new Particle();
    }

    public bool Reset(IParticle obj)
    {
        return true;
    }
}