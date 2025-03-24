using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tankontroller.World.Particles;

namespace Tankontroller.World.Bullets
{
    class FortniteZone : Bullet
    {
        public FortniteZone(Vector2 pPosition, Color pColour, float pLifeTime) : base(pPosition, new Vector2(0,0), pColour, pLifeTime) { Radius = 1000; }
        public override void Update(float pSeconds)
        {
            //decrease radius every seconds
            Radius -= 1.0f * pSeconds;
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
            return false;
        }
        public override bool DoCollision(Bullet pBullet)
        {
            return false;
        }
        public override bool LifeTimeExpired()
        {
            return false;
        }
    }
}
