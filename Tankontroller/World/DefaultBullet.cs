using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tankontroller.World.Particles;

namespace Tankontroller.World
{
    class DefaultBullet : Bullet
    {
        public DefaultBullet(Vector2 pPosition, Vector2 pVelocity, Color pColour) : base(pPosition, pVelocity, pColour) { }

        public override void Update(float pSeconds)
        {
            BulletInitialisationPolicy bulletParticles = new BulletInitialisationPolicy(Position, Colour);
            ParticleManager.Instance().InitialiseParticles(bulletParticles, 2);
            base.Update(pSeconds);
        }

        public override void DoCollision(Rectangle pRectangle)
        {
            Vector2 collisonNormal = GetCollisionNormal(pRectangle);
            CreateExplosion(-collisonNormal);
        }

        public override void DoCollision(RectWall pWall)
        {
            Vector2 collisonNormal = GetCollisionNormal(pWall.Rectangle);
            CreateExplosion(collisonNormal);
        }

        public override void DoCollision(Tank pTank)
        {
            CreateExplosion(Vector2.Normalize(Position - pTank.GetWorldPosition()));
        }

        public override void DoCollision(Bullet pBullet)
        {
            Vector2 collisionNormal = Vector2.Normalize(Velocity);
            CreateExplosion(collisionNormal);
            CreateExplosion(-collisionNormal);
        }

        private void CreateExplosion(Vector2 pCollisionNormal)
        {
            ExplosionInitialisationPolicy explosion = new ExplosionInitialisationPolicy(Position, pCollisionNormal, Colour);
            Particles.ParticleManager.Instance().InitialiseParticles(explosion, 100);
        }

    }

}
