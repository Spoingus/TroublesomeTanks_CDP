using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Tankontroller.World.Particles;

namespace Tankontroller.World
{
    class BouncyEMPBullet : Bullet
    {
        private static readonly Texture2D EMPTexture = Tankontroller.Instance().CM().Load<Texture2D>("battery");
        public BouncyEMPBullet(Vector2 pPosition, Vector2 pVelocity, float pLifeTime) : base(pPosition, pVelocity, Color.Blue, pLifeTime) { }
        private float Rotation = 0.0f;

        public override void Update(float pSeconds)
        {
            Random rand = new Random();
            EMPBlastInitPolicy explosion = new EMPBlastInitPolicy(Position, 0.1f);
            Particles.ParticleManager.Instance().InitialiseParticles(explosion, 1);
            Rotation += 0.02f;
            LifeTime -= pSeconds;
            base.Update(pSeconds);
        }

        public override bool DoCollision(Rectangle pRectangle)
        {
            Vector2 collisonNormal = GetCollisionNormal(pRectangle);
            Velocity = Vector2.Reflect(Velocity, collisonNormal);
            return false;
        }

        public override bool DoCollision(RectWall pWall)
        {
            Vector2 collisonNormal = GetCollisionNormal(pWall.Rectangle);
            Velocity = Vector2.Reflect(Velocity, collisonNormal);
            return false;
        }

        public override bool DoCollision(Tank pTank)
        {
            CreateBlast();
            return true;
        }

        public override bool DoCollision(Bullet pBullet)
        {
            Vector2 collisionNormal = Vector2.Normalize(Velocity);
            return false;
        }

        private void CreateBlast()
        {
            EMPBlastInitPolicy explosion = new EMPBlastInitPolicy(Position, 6.5f);
            Particles.ParticleManager.Instance().InitialiseParticles(explosion, 200);
        }

        public override bool LifeTimeExpired()
        {
            return (LifeTime <= 0.0f);
        }

        public override void Draw(SpriteBatch pBatch, Texture2D pTexture)
        {
            pBatch.Draw(EMPTexture, Position, null, Color.White, Rotation, new Vector2(EMPTexture.Width / 2, EMPTexture.Height / 2), BULLET_RADIUS * 0.01f, SpriteEffects.None, 0.0f);
        }
    }
}
