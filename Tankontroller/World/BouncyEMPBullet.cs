using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tankontroller.World
{
    class BouncyEMPBullet : Bullet
    {
        public BouncyEMPBullet(Vector2 pPosition, Vector2 pVelocity, Color pColour) : base(pPosition, pVelocity, pColour)
        {
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
            
            IncreaseRadius();
        }

        public override void DoCollision(Bullet pBullet)
        {
            Vector2 collisionNormal = Vector2.Normalize(Velocity);
        }

        public void IncreaseRadius()
        {
            BULLET_RADIUS += 0.05f;
        }
    }
}
