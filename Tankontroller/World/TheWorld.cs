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
            if(m_Tanks.Count > pTankIndex)
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

            Vector2 collisionNormal;

            for (int tankIndex = 0; tankIndex < m_Tanks.Count; tankIndex++)
            {
                List<Bullet> bulletList = m_Tanks[tankIndex].GetBulletList();
                for (int i = bulletList.Count - 1; i >= 0; --i)
                {
                    if (bulletList[i].Collide(pRectangle, out collisionNormal))
                    {
                        ExplosionInitialisationPolicy explosion = new ExplosionInitialisationPolicy(bulletList[i].Position, collisionNormal, m_Tanks[tankIndex].Colour());
                        ParticleManager.Instance().InitialiseParticles(explosion, 100);
                        bulletList.RemoveAt(i);
                    }
                }            
            }
        }

        public void Update(float pSeconds)
        {
            Particles.ParticleManager.Instance().Update(pSeconds);

            Vector2 collisionNormal;

            for (int tankIndex = 0; tankIndex < m_Tanks.Count; tankIndex++)
            {
                List<Bullet> bulletList = m_Tanks[tankIndex].GetBulletList();
                for (int i = bulletList.Count - 1; i >= 0; --i)
                {
                    bool collided = false;
                    bulletList[i].Update(pSeconds);
                    for (int tankIndex2 = 0; tankIndex2 < m_Tanks.Count; tankIndex2++)
                    {
                        if(tankIndex == tankIndex2)
                        {
                            continue;
                        }
                        if (bulletList[i].Collide(m_Tanks[tankIndex2], out collisionNormal))
                        {
                            ExplosionInitialisationPolicy explosion = new ExplosionInitialisationPolicy(bulletList[i].Position, collisionNormal, m_Tanks[tankIndex].Colour());
                            Particles.ParticleManager.Instance().InitialiseParticles(explosion, 100);
                            m_Tanks[tankIndex2].TakeDamage();
                            int clang = rand.Next(1, 4);
                            string tankClang = "Sounds/Tank_Clang" + clang;
                            Microsoft.Xna.Framework.Audio.SoundEffectInstance tankClangSound = Tankontroller.Instance().GetSoundManager().GetSoundEffectInstance(tankClang);
                            tankClangSound.Play();
                            collided = true;
                            break;
                        }
                        
                    }
                    foreach (RectWall wall in Walls)
                    {
                        if (wall.Collide(bulletList[i], out collisionNormal))
                        {
                            ExplosionInitialisationPolicy explosion = new ExplosionInitialisationPolicy(bulletList[i].Position, collisionNormal, m_Tanks[tankIndex].Colour());
                            Particles.ParticleManager.Instance().InitialiseParticles(explosion, 100);
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

            foreach(RectWall w in Walls)
            {
                foreach(Tank t in m_Tanks)
                {
                    if (w.Collide(t))
                    {
                        t.PutBack();
                    }
                }
            }

            for(int i = 0; i < m_Tanks.Count; i++)
            {
                for(int j = i+1; j < m_Tanks.Count; j++)
                {
                    if(m_Tanks[i].Collide(m_Tanks[j]))
                    {
                        m_Tanks[i].PutBack();
                        m_Tanks[j].PutBack();
                    }
                }
            }
        }
    }
}
