using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tankontroller.World.Particles;

namespace Tankontroller.World
{
    public class Bullet
    {
        const int BULLET_RADIUS = 10;
        public Vector2 Position { get; private set; }
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

        public bool CollideWithPlayArea(Rectangle pRectangle)
        {
            if (!pRectangle.Contains(Position))
            {
                Vector2 collisionNormal = GetCollisionNormal(pRectangle);
                collisionNormal = -collisionNormal;
                CreateExplosion(collisionNormal);
                return true;
            }
            return false;
        }

        public bool Collide(RectWall pWall)
        {
            Rectangle rectangle = pWall.Rectangle;
            if (rectangle.Contains(Position))
            {
                Vector2 collisionNormal = GetCollisionNormal(rectangle);
                CreateExplosion(collisionNormal);
                return true;
            }
            return false;
        }
        public bool Collide(Tank pTank)
        {
            if (pTank.PointIsInTank(Position))
            {
                CreateExplosion(Vector2.Normalize(Position - pTank.GetWorldPosition()));
                return true;
            }
            return false;
        }
        public bool Collide(Bullet pBullet) // This is unused but I'm keeping it for potential implementation
        {
            if (Vector2.Distance(Position, pBullet.Position) < 2 * BULLET_RADIUS)
            {
                Vector2 collisionNormal = Vector2.Normalize(Velocity);
                CreateExplosion(collisionNormal);
                CreateExplosion(-collisionNormal);
                return true;
            }
            return false;
        }

        private Vector2 GetCollisionNormal(Rectangle pRect)
        {
            float difference = Math.Abs(Position.X - pRect.Left);
            Vector2 collisionNormal = new Vector2(-1, 0);
            if (difference > Math.Abs(Position.X - pRect.Right))
            {
                difference = Math.Abs(Position.X - pRect.Right);
                collisionNormal = new Vector2(1, 0);
            }
            if (difference > Math.Abs(Position.Y - pRect.Top))
            {
                difference = Math.Abs(Position.Y - pRect.Top);
                collisionNormal = new Vector2(0, -1);
            }
            if (difference > Math.Abs(Position.Y - pRect.Bottom))
            {
                collisionNormal = new Vector2(0, 1);
            }
            return collisionNormal;
        }

        private void CreateExplosion(Vector2 pCollisionNormal)
        {
            ExplosionInitialisationPolicy explosion = new ExplosionInitialisationPolicy(Position, pCollisionNormal, Colour);
            Particles.ParticleManager.Instance().InitialiseParticles(explosion, 100);
        }

        public void Draw(SpriteBatch pBatch, Texture2D pTexture)
        {
            Particle.DrawCircle(pBatch, pTexture, BULLET_RADIUS + 2 * DGS.Instance.GetInt("PARTICLE_EDGE_THICKNESS"), Position, Color.Black);
            Particle.DrawCircle(pBatch, pTexture, BULLET_RADIUS, Position, Colour);
        }
    }
}
