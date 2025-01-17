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
    // this class is a container for the game state
    public class TheWorld
    {
        private Random rand = new Random();

        private List<Tank> m_Tanks = new List<Tank>();

        public void AddTank(Tank pTank)
        {
            m_Tanks.Add(pTank);
        }
        public Tank GetTank(int pTankIndex)
        {
            if (m_Tanks.Count > pTankIndex)
            {
                return m_Tanks[pTankIndex];
            }
            return null;
        }

        public List<RectWall> Walls { get; private set; }

        public TheWorld(Rectangle playArea, List<RectWall> pWalls)
        {
            Walls = pWalls;
        }

        public void CollideAllTheThingsWithPlayArea(Rectangle pRectangle)
        {
            for (int i = 0; i < m_Tanks.Count; i++)
            {
                m_Tanks[i].CollideWithPlayArea(pRectangle);
            }

            for (int tankIndex = 0; tankIndex < m_Tanks.Count; tankIndex++)
            {
                List<Bullet> bulletList = m_Tanks[tankIndex].GetBulletList();
                for (int i = bulletList.Count - 1; i >= 0; --i)
                {
                    if (bulletList[i].CollideWithPlayArea(pRectangle))
                    {
                        bulletList.RemoveAt(i);
                    }
                }
            }
        }

        public void Update(float pSeconds)
        {
            Particles.ParticleManager.Instance().Update(pSeconds);

            // Check bullet collision
            for (int listIndex = 0; listIndex < m_Tanks.Count; listIndex++)
            {
                List<Bullet> bulletList = m_Tanks[listIndex].GetBulletList();
                for (int i = bulletList.Count - 1; i >= 0; --i)
                {
                    bulletList[i].Update(pSeconds);

                    bool collided = false;
                    foreach (RectWall wall in Walls)
                    {
                        if (bulletList[i].Collide(wall))
                        {
                            collided = true;
                            break;
                        }
                    }
                    for (int tankIndex = 0; tankIndex < m_Tanks.Count && !collided; tankIndex++)
                    {
                        if (listIndex == tankIndex) // Skip collision with self
                        {
                            continue;
                        }
                        if (bulletList[i].Collide(m_Tanks[tankIndex]))
                        {
                            m_Tanks[tankIndex].TakeDamage();
                            int clang = rand.Next(1, 4);
                            string tankClang = "Sounds/Tank_Clang" + clang;
                            Microsoft.Xna.Framework.Audio.SoundEffectInstance tankClangSound = Tankontroller.Instance().GetSoundManager().GetSoundEffectInstance(tankClang);
                            tankClangSound.Play();
                            collided = true;
                            break;
                        }

                    }
                    if (collided)
                    {
                        bulletList.RemoveAt(i);
                    }

                }
            }

            foreach (RectWall w in Walls)
            {
                foreach (Tank t in m_Tanks)
                {
                    if (w.Collide(t))
                    {
                        t.PutBack();
                    }
                }
            }

            for (int i = 0; i < m_Tanks.Count; i++)
            {
                for (int j = i + 1; j < m_Tanks.Count; j++)
                {
                    if (m_Tanks[i].Collide(m_Tanks[j]))
                    {
                        m_Tanks[i].PutBack();
                        m_Tanks[j].PutBack();
                    }
                }
            }
        }
    }
}
