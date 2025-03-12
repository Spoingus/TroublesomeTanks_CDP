using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tankontroller.Managers;
using Tankontroller.World.Particles;
using Tankontroller.World.Pickups;

public enum PickupType
{
    HEALTH,
    EMP,
    MINE,
    BOUNCY
}

namespace Tankontroller.World
{
    public class TheWorld
    {
        private static readonly Texture2D mPixelTexture = Tankontroller.Instance().CM().Load<Texture2D>("block");
        private static readonly Texture2D m_BulletTexture = Tankontroller.Instance().CM().Load<Texture2D>("circle");
        private static readonly Color GROUND_COLOUR = DGS.Instance.GetColour("COLOUR_GROUND");
        private static readonly bool PICKUP_SPAWN = DGS.Instance.GetBool("PICKUPS_ON");
        private static readonly float PICKUP_SPAWN_TIME = DGS.Instance.GetFloat("PICKUP_SPAWN_RATE");

        private Rectangle mPlayArea;
        private Rectangle mPlayAreaOutline;
        private List<Tank> mTanks = new List<Tank>();
        private List<RectWall> mWalls;
        private List<Vector2> mPickupSpawnPositions = new List<Vector2>();
        private List<Pickup> mPickups = new List<Pickup>();
        private float mPickupSpawnTimer = PICKUP_SPAWN_TIME;

        public Rectangle PlayArea { get { return mPlayArea; } }

        public TheWorld(Rectangle pPlayArea, List<RectWall> pWalls, List<Tank> pTanks, List<Vector2> pPickupSpawnPositions)
        {
            mWalls = pWalls;
            mTanks = pTanks;
            mPlayArea = pPlayArea;
            mPickupSpawnPositions = pPickupSpawnPositions;
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

        public void AddPickup()
        {
            mPickupSpawnTimer = PICKUP_SPAWN_TIME;
            if (PICKUP_SPAWN)
            {
                int randPos = new Random().Next(0, mPickupSpawnPositions.Count());
                //Checks for any pickups at this position to prevent spawn overlap
                foreach (Pickup p in mPickups)
                {
                    if (p.m_Position == mPickupSpawnPositions[randPos])
                    {
                        return;
                    }
                }
                int randPickup = new Random().Next(0, Enum.GetNames(typeof(PickupType)).Length);
                if ((PickupType)randPickup == PickupType.HEALTH)
                {
                    HealthPickup mHealthPickup = new HealthPickup(mPickupSpawnPositions[randPos]);
                    mPickups.Add(mHealthPickup);
                }
                else if ((PickupType)randPickup == PickupType.EMP)
                {
                    EMPPickup mEMPPickup = new EMPPickup(mPickupSpawnPositions[randPos]);
                    mPickups.Add(mEMPPickup);
                }
                else if ((PickupType)randPickup == PickupType.MINE)
                {
                    MinePickup mMinePickup = new MinePickup(mPickupSpawnPositions[randPos]);
                    mPickups.Add(mMinePickup);
                }
                else if ((PickupType)randPickup == PickupType.BOUNCY)
                {
                    BouncyBulletPickup mBouncyBulletPickup = new BouncyBulletPickup(mPickupSpawnPositions[randPos]);
                    mPickups.Add(mBouncyBulletPickup);
                }
            }
        }

        public void Update(float pSeconds)
        {
            mPickupSpawnTimer -= pSeconds;
            if(mPickupSpawnTimer <= 0) { AddPickup(); }
            Particles.ParticleManager.Instance().Update(pSeconds);

            // Check collisions for each tank
            for (int tankIndex = 0; tankIndex < mTanks.Count; tankIndex++)
            {
                mTanks[tankIndex].Update(pSeconds);

                mTanks[tankIndex].CheckBullets(mTanks, mPlayArea, mWalls);

                //test pickup collision
                foreach (Pickup p in mPickups)
                {
                    if (p.PickUpCollision(mTanks[tankIndex]))
                    {
                        mPickups.Remove(p);
                        break;
                    }
                }

                // Wall collisions
                foreach (RectWall wall in mWalls)
                {
                    Rectangle wallRect = wall.Rectangle;

                    // tank collision using collision manager
                    if (CollisionManager.Collide(mTanks[tankIndex], wallRect, false))
                        mTanks[tankIndex].PutBack();
                }

                // Collisions with other tanks
                for (int i = 0; i < mTanks.Count; i++)
                {
                    if (tankIndex == i) // Skip collision with self
                    {
                        continue;
                    }
                    // tank against tanks using collision manager
                    if (CollisionManager.Collide(mTanks[tankIndex], mTanks[i]))
                        mTanks[tankIndex].PutBack();
                }

                // Collisions with the play area
                if (CollisionManager.Collide(mTanks[tankIndex], mPlayArea, true)) // True tp check inside the play area
                    mTanks[tankIndex].PutBack();
            }
        }

        public void Draw(SpriteBatch pSpriteBatch)
        {
            pSpriteBatch.Draw(mPixelTexture, mPlayAreaOutline, Color.Black);
            pSpriteBatch.Draw(mPixelTexture, mPlayArea, GROUND_COLOUR);

            TrackSystem.GetInstance().Draw(pSpriteBatch);
            ParticleManager.Instance().Draw(pSpriteBatch);

            //Draws the tanks (on top of tracks but below particles)
            foreach (Tank t in mTanks)
            {
                t.DrawBullets(pSpriteBatch, m_BulletTexture);
            }

            //Draws the tanks
            foreach (Tank t in mTanks)
            {
                t.Draw(pSpriteBatch);
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

            foreach (Pickup p in mPickups)
            {
                p.Draw(pSpriteBatch);
            }
        }
    }
}
