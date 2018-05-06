using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Tankontroller.World.Particles;

namespace Tankontroller.World
{
    public class Bullet
    {
        public Vector2 Position { get; private set;}
        public Vector2 Velocity { get; private set; }
        public Color Colour { get; private set; }

        public Bullet(Vector2 pPosition, Vector2 pVelocity, Color pColour)
        {
            Position = pPosition;
            Velocity = pVelocity;
            Colour = pColour;
        }

        public void Update(float pSeconds)
        {
            Position = Position + Velocity * pSeconds;
            BulletInitialisationPolicy bulletParticles = new BulletInitialisationPolicy(Position, Colour);
            ParticleManager.Instance().InitialiseParticles(bulletParticles, 2);
        }

        public bool Collide(Rectangle pRectangle, out Vector2 pCollisionNormal)
        {
            if(!pRectangle.Contains(Position))
            {
                float difference = Math.Abs(Position.X - pRectangle.Left);
                pCollisionNormal = new Vector2(1, 0);
                if (difference > Math.Abs(Position.X - pRectangle.Right))
                {
                    difference = Math.Abs(Position.X - pRectangle.Right);
                    pCollisionNormal = new Vector2(-1, 0);
                }
                if (difference > Math.Abs(Position.Y - pRectangle.Top))
                {
                    difference = Math.Abs(Position.Y - pRectangle.Top);
                    pCollisionNormal = new Vector2(0, 1);
                }
                if (difference > Math.Abs(Position.Y - pRectangle.Bottom))
                {
                    pCollisionNormal = new Vector2(0, -1);
                }
                return true;
            }
            else
            {
                pCollisionNormal = Vector2.Zero;
            }
            return false;
        }
        public bool Collide(Tank pTank, out Vector2 pCollisionNormal)
        {
            /*
            Vector2 tankPos = pTank.GetWorldPosition();
            float tankRot = pTank.GetRotation();
            Vector2 bulletPos = Position;

            if((tankPos - bulletPos).Length() < DGS.TANK_RADIUS)
            {
                pCollisionNormal = (bulletPos - tankPos);
                pCollisionNormal.Normalize();
                return true;
            }
            pCollisionNormal = Vector2.Zero;
            return false;
        */
            pCollisionNormal = Vector2.Normalize(Position - pTank.GetWorldPosition());
            return pTank.PointIsInTank(Position);
        }
    }
}
