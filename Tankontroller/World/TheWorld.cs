using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tankontroller.World.Particles;

namespace Tankontroller.World
{
    public class TheWorld
    {
        private static readonly Texture2D mPixelTexture = Tankontroller.Instance().CM().Load<Texture2D>("block");
        private static readonly Texture2D m_BulletTexture = Tankontroller.Instance().CM().Load<Texture2D>("circle");
        private static readonly Color GROUND_COLOUR = DGS.Instance.GetColour("COLOUR_GROUND");

        private Rectangle mPlayArea;
        private Rectangle mPlayAreaOutline;
        private List<Tank> mTanks = new List<Tank>();
        private List<RectWall> mWalls;

        public Rectangle PlayArea { get { return mPlayArea; } }

        public TheWorld(Rectangle pPlayArea, List<RectWall> pWalls, List<Tank> pTanks)
        {
            mWalls = pWalls;
            mTanks = pTanks;
            mPlayArea = pPlayArea;
            mPlayAreaOutline = new Rectangle(mPlayArea.X - 5, mPlayArea.Y - 5, mPlayArea.Width + 10, mPlayArea.Height + 10);
        }

        public List<Tank> GetTanksForPlayers(int pPlayerCount)
        {
            if (mTanks.Count >= pPlayerCount && pPlayerCount > 0)
            {
                mTanks = mTanks.GetRange(0, (int)pPlayerCount);
                return mTanks;
            }
            return null;
        }

        public void Update(float pSeconds)
        {
            Particles.ParticleManager.Instance().Update(pSeconds);

            // Check collisions for each tank
            for (int tankIndex = 0; tankIndex < mTanks.Count; tankIndex++)
            {
                mTanks[tankIndex].Update(pSeconds);

                // Wall collisions
                foreach (RectWall wall in mWalls)
                {
                    // bullet collision
                    mTanks[tankIndex].CheckBulletCollisions(wall);
                    // tank collision
                    mTanks[tankIndex].Collide(wall);
                }

                // Collisions with other tanks
                for (int i = 0; i < mTanks.Count; i++)
                {
                    // bullet collision
                    mTanks[tankIndex].CheckBulletCollisions(mTanks[i]);

                    if (tankIndex == i) // Skip collision with self
                    {
                        continue;
                    }
                    // tank against tanks
                    mTanks[tankIndex].Collide(mTanks[i]);
                }

                // Collisions with the play area
                mTanks[tankIndex].CollideWithPlayArea(mPlayArea);
                mTanks[tankIndex].CheckBulletCollisionsWithPlayArea(mPlayArea);
            }
        }

        public void Draw(SpriteBatch pSpriteBatch)
        {
            pSpriteBatch.Draw(mPixelTexture, mPlayAreaOutline, Color.Black);
            pSpriteBatch.Draw(mPixelTexture, mPlayArea, GROUND_COLOUR);

            TrackSystem.GetInstance().Draw(pSpriteBatch);

            //Draws the tanks (on top of tracks but below particles)
            foreach (Tank t in mTanks)
            {
                t.Draw(pSpriteBatch);
            }

            ParticleManager.Instance().Draw(pSpriteBatch);

            //Draws the bullets
            foreach (Tank t in mTanks)
            {
                t.DrawBullets(pSpriteBatch, m_BulletTexture);
            }

            //Draws the walls
            foreach (RectWall w in mWalls)
            {
                w.DrawOutlines(pSpriteBatch);
            }
            foreach (RectWall w in mWalls)
            {
                w.Draw(pSpriteBatch);
            }
        }
    }
}
