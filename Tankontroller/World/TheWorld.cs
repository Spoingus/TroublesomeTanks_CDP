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
                m_Tanks[i].CheckBulletCollisionsWithPlayArea(pRectangle);
            }
        }

        public void Update(float pSeconds)
        {
            Particles.ParticleManager.Instance().Update(pSeconds);

            // Check collisions for each tank
            for (int tankIndex = 0; tankIndex < m_Tanks.Count; tankIndex++)
            {
                m_Tanks[tankIndex].Update(pSeconds);

                // Wall collisions
                foreach (RectWall wall in Walls)
                {
                    // bullet collision
                    m_Tanks[tankIndex].CheckBulletCollisions(wall);
                    // tank collision
                    m_Tanks[tankIndex].Collide(wall);
                }

                // Collisions with other tanks
                for (int i = 0; i < m_Tanks.Count; i++)
                {
                    if (tankIndex == i) // Skip collision with self
                    {
                        continue;
                    }
                    // bullet collision
                    m_Tanks[tankIndex].CheckBulletCollisions(m_Tanks[i]);
                    // tank against tanks
                    m_Tanks[tankIndex].Collide(m_Tanks[i]);
                }
            }
        }
    }
}
