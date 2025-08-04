using Microsoft.Xna.Framework;
using Rubedo.Graphics.Sprites;
using Rubedo.Object;

namespace MonoGame.Particles.Particles
{
    public class Particle : IParticle
    {
        public Transform Transform { get; set; }
        public Vector2 Velocity { get; set; }
        public float AngularVelocity { get; set; }
        public bool IsParticle { get; set; }
        public double Age { get; set; }
        public double MaxAge { get; set; }
        public float Alpha { get; set; } = 1.0f;
        public Color Color { get; set; } = Color.White;
        public TextureRegion2D Texture { get; set; }

        public Particle()
        {
            IsParticle = true;
            Transform = new Transform();
        }

        public void Reset()
        {
            Texture = null; //we want to make sure this one gets disposed of if we're done with it.
            return; //otherwise everything gets reset when it's created, so no need to reset it all.
        }
    }
}
