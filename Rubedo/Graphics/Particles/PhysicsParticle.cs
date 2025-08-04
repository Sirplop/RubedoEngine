using Microsoft.Xna.Framework;
using Rubedo.Graphics.Sprites;
using Rubedo.Object;
using Rubedo.Physics2D;
using Rubedo.Physics2D.Collision;
using Rubedo.Physics2D.Dynamics;
using Rubedo.Physics2D.Dynamics.Shapes;
using System;
using System.Collections.Generic;

namespace MonoGame.Particles.Particles
{
    public class PhysicsParticle : IParticle
    {
        public Transform Transform { get; set; }
        public double Age { get; set; }
        public double MaxAge { get; set; }
        public float Alpha { get; set; } = 1.0f;
        public Color Color { get; set; } = Color.White;
        public TextureRegion2D Texture { get; set; }
        public PhysicsBody Body { get; set; }
        public bool IsParticle { get => Body.IsParticle; set => throw new NotImplementedException(); }
        public Vector2 Velocity { get => Body.LinearVelocity; set => Body.LinearVelocity = value; }
        public float AngularVelocity { get => Body.AngularVelocity; set => Body.AngularVelocity = value; }
        public float LinearDamping { get => Body.material.linearDamping; }
        public float GravityScale { get => Body.gravityScale; set => Body.gravityScale = value; }

        public delegate ContactAction OnParticleCollisionEventHandler(PhysicsParticle sender, PhysicsBody other, Manifold m);

        internal OnParticleCollisionEventHandler onParticleCollisionEventHandler;
        public event OnParticleCollisionEventHandler OnCollision
        {
            add { onParticleCollisionEventHandler += value; }
            remove { onParticleCollisionEventHandler -= value; }
        }

        public PhysicsParticle(PhysicsMaterial material, Shape shape, Vector2 pos, bool isTrigger)
        {
            Collider collider = Collider.CreateFromShape(shape, isTrigger);
            Body = new PhysicsBody(collider, material);
            Body.TargetTransform = Transform;
            collider.shape.SetTransform(Transform);
            Body.IsParticle = true;

            if (isTrigger)
                Body.collider.OnTriggerEnter += BodyOnCollision;
            else
                Body.collider.OnCollisionEnter += BodyOnCollision;
        }
        private void BodyOnCollision(PhysicsBody sender, PhysicsBody other, Manifold m)
        {
            onParticleCollisionEventHandler?.Invoke(this, other, m);
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
