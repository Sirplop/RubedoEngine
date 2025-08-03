using Microsoft.Xna.Framework;
using Rubedo.Graphics.Sprites;

namespace MonoGame.Particles.Particles
{
    public class Particle : IParticle
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Orientation { get; set; }
        public float AngularVelocity { get; set; }
        public bool IsParticle { get; set; }
        public double Age { get; set; }
        public double MaxAge { get; set; }
        public float Alpha { get; set; } = 1.0f;
        public Color Color { get; set; } = Color.White;
        public float Scale { get; set; } = 1.0f;
        public TextureRegion2D Texture { get; set; }

        public Particle()
        {
            IsParticle = true;
        }

        public void Reset()
        {
            Texture = null; //we want to make sure this one gets disposed of if we're done with it.
            return; //otherwise everything gets reset when it's created, so no need to reset it all.
        }
    }
}
