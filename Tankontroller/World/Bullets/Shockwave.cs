using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Tankontroller.World.Bullets;
using Tankontroller.World.Particles;

namespace Tankontroller.World.Bullets
{
    class Shockwave : Bullet
    {
        private static readonly Texture2D ShockWaveTexture = Tankontroller.Instance().CM().Load<Texture2D>("Shockwave");
        public Shockwave(Vector2 pPosition, Vector2 pVelocity, Color pColour, float pLifeTime) : base(pPosition, pVelocity, pColour, pLifeTime) { }
        private new float BULLET_RADIUS = 1.0f;
        public override void Update(float pSeconds)
        {
            BULLET_RADIUS += 1.0f;
            LifeTime -= pSeconds;
            base.Update(pSeconds);
        }
        public override bool DoCollision(Rectangle pRectangle)
        {
            return false;
        }
        public override bool DoCollision(RectWall pWall)
        {
            return false;
        }
        public override bool Collide(Tank pTank)
        {
            float distance = Vector2.Distance(pTank.GetWorldPosition(), Position);
            if (distance < BULLET_RADIUS)
            {
                return true;
            }
            return false;
        }

        public override bool DoCollision(Tank pTank)
    {
            Vector2 CollisionNormal = Vector2.Normalize(Position - pTank.GetWorldPosition());
            ExplosionInitialisationPolicy explosion = new ExplosionInitialisationPolicy(Position * 2, CollisionNormal, Colour);
            Particles.ParticleManager.Instance().InitialiseParticles(explosion, 20);
            return false;
        }
        public override bool DoCollision(Bullet pBullet)
        {
            return false;
        }
        public override bool LifeTimeExpired()
        {
            return (LifeTime <= 0);
        }
        public override void Draw(SpriteBatch pBatch, Texture2D pTexture)
        {
            Color ShockWaveColour = Color.Yellow;
            ShockWaveColour.A = (byte)(0.0f);
            Particle.DrawCircle(pBatch, ShockWaveTexture, (int)BULLET_RADIUS + 2 * Particle.EDGE_THICKNESS, Position, ShockWaveColour);
        }
    }
}
