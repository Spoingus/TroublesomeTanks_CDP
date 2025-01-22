using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tankontroller.World.Particles;

namespace Tankontroller.World
{
    class BouncyEMPBullet : Bullet
    {
        public BouncyEMPBullet(Vector2 pPosition, Vector2 pVelocity) : base(pPosition, pVelocity, Color.Yellow) { }

        public override void Update(float pSeconds)
        {
            Random rand = new Random();
            EMPBlastInitPolicy explosion = new EMPBlastInitPolicy(Position, 0.1f);
            Particles.ParticleManager.Instance().InitialiseParticles(explosion, 1);
            base.Update(pSeconds);
        }

        public override void DoCollision(Rectangle pRectangle)
        {
            Vector2 collisonNormal = GetCollisionNormal(pRectangle);
            Velocity = Vector2.Reflect(Velocity, collisonNormal);
        }

        public override void DoCollision(RectWall pWall)
        {
            Vector2 collisonNormal = GetCollisionNormal(pWall.Rectangle);
            Velocity = Vector2.Reflect(Velocity, collisonNormal);
        }

        public override void DoCollision(Tank pTank)
        {
            CreateBlast();
        }

        public override void DoCollision(Bullet pBullet)
        {
            Vector2 collisionNormal = Vector2.Normalize(Velocity);
        }

        private void CreateBlast()
        {
            EMPBlastInitPolicy explosion = new EMPBlastInitPolicy(Position, 3.0f);
            Particles.ParticleManager.Instance().InitialiseParticles(explosion, 200);
        }
    }
}
