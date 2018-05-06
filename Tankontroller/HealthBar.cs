using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tankontroller.World;

namespace Tankontroller
{
    public class HealthBar
    {
        private Texture2D[] m_Textures = new Texture2D[6];
        private Rectangle m_Rectangle;
        private Tank m_Tank;

        public HealthBar(Texture2D pT0, Texture2D pT1, Texture2D pT2, Texture2D pT3, Texture2D pT4, Texture2D pT5, Rectangle pRectangle, Tank pTank)
        {
            m_Textures[0] = pT0;
            m_Textures[1] = pT1;
            m_Textures[2] = pT2;
            m_Textures[3] = pT3;
            m_Textures[4] = pT4;
            m_Textures[5] = pT5;
            m_Rectangle = pRectangle;
            m_Tank = pTank;
        }

        public void Reposition(Rectangle pRectangle)
        {
            m_Rectangle = pRectangle;
        }

       

        public void Draw(SpriteBatch pSpriteBatch)
        {
            if (m_Tank.Health() >= 0)
            {
                pSpriteBatch.Draw(m_Textures[m_Tank.Health()], m_Rectangle, Color.White);
            }
        }
    }
}
