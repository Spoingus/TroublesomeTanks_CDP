using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tankontroller.World
{
    public class RectWall
    {
        public Rectangle Rectangle { get; private set; }
        private Rectangle m_OutlineRectangle;
        private Texture2D m_Texture;

        public RectWall(Texture2D pTexture, Rectangle pRectangle)
        {
            Rectangle = pRectangle;
            m_OutlineRectangle = new Rectangle(pRectangle.X - 2, pRectangle.Y - 2, pRectangle.Width + 4, pRectangle.Height + 4);
            m_Texture = pTexture;
        }

        public void Draw(SpriteBatch pSpriteBatch)
        {
            pSpriteBatch.Draw(m_Texture, Rectangle, DGS.Instance.GetColour("COLOUR_WALLS"));
        }

        public void DrawOutlines(SpriteBatch pSpriteBatch)
        {
            pSpriteBatch.Draw(m_Texture, m_OutlineRectangle, Color.Black);
        }

        public bool Collide(Tank pTank)
        {
            Vector2[] tankCorners = new Vector2[4];
            pTank.GetCorners(tankCorners);

            foreach (Vector2 corner in tankCorners)
            {
                if (Rectangle.Contains(corner))
                {
                    return true;
                }
            }
            // Check if any of the corners of the wall are within the tank
            if (pTank.PointIsInTank(new Vector2(Rectangle.Left, Rectangle.Top)) ||
               pTank.PointIsInTank(new Vector2(Rectangle.Right, Rectangle.Top)) ||
               pTank.PointIsInTank(new Vector2(Rectangle.Left, Rectangle.Bottom)) ||
               pTank.PointIsInTank(new Vector2(Rectangle.Right, Rectangle.Bottom)))
            {
                return true;
            }
            return false;
        }
    }
}
