using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Tankontroller.World.Particles;

namespace Tankontroller.World
{
    class Shockwave : Bullet
    {
        public Shockwave(Vector2 pPosition, Vector2 pVelocity, Color pColour, float pLifeTime) : base(pPosition, pVelocity, pColour, pLifeTime) { }
        private new float BULLET_RADIUS = 1.0f;
        public override void Update(float pSeconds)
        {
            BULLET_RADIUS += 1.0f;
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
        public override bool DoCollision(Tank pTank)
        {

            float distance = Vector2.Distance(Position, pTank.GetWorldPosition());
            if (distance < BULLET_RADIUS)
            {
                Vector2 CollisionNormal = Vector2.Normalize(Position - pTank.GetWorldPosition());
                ExplosionInitialisationPolicy explosion = new ExplosionInitialisationPolicy(Position, CollisionNormal, Colour);
                Particles.ParticleManager.Instance().InitialiseParticles(explosion, 20);
                return true;
            }
            return false;
        }
        public override bool DoCollision(Bullet pBullet)
        {
            return false;
        }
        public override bool LifeTimeExpired()
        {
            if(BULLET_RADIUS >= 30.0f)
            {
                return true;
            }
            return false;
        }
    }
}
