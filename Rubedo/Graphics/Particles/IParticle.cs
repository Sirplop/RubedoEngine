using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rubedo.Graphics.Sprites;
using Rubedo.Lib.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Particles.Particles
{
    public interface IParticle : IPoolable
    {
        double Age { get; set; }
        double MaxAge { get; set; }
        float Alpha { get; set; } 
        Color Color { get; set; }
        bool IsParticle { get; set; }
        Vector2 Position { get; set; }
        Vector2 Velocity { get; set; }
        float Orientation { get; set; }
        float AngularVelocity { get; set; }    
        TextureRegion2D Texture { get; set; }
    }
}
