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

        }

        public override void DoCollision(RectWall pWall)
        {

        }

        public override void DoCollision(Tank pTank)
        {

        }

        public override void DoCollision(Bullet pBullet)
        {

        }
    }
}
