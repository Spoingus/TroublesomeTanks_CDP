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
        private Rectangle m_Rectangle;
        private Rectangle m_OutlineRectangle;
        private Texture2D m_Texture;

        public RectWall(Texture2D pTexture, Rectangle pRectangle)
        {
            m_Rectangle = pRectangle;
            m_OutlineRectangle = new Rectangle(pRectangle.X - 2, pRectangle.Y - 2, pRectangle.Width + 4, pRectangle.Height + 4);
            m_Texture = pTexture;
        }

        public void Draw(SpriteBatch pSpriteBatch)
        {
            pSpriteBatch.Draw(m_Texture, m_Rectangle, DGS.Instance.GetColour("COLOUR_WALLS"));
        }

        public void DrawOutlines(SpriteBatch pSpriteBatch)
        {
            pSpriteBatch.Draw(m_Texture, m_OutlineRectangle, Color.Black);
        }

        public bool Collide(Bullet pBullet, out Vector2 pCollisionNormal)
        {
            Vector2 bulletPos = pBullet.Position;
            if ((bulletPos.X >= m_Rectangle.Left) &&
                (bulletPos.X <= m_Rectangle.Right) &&
                (bulletPos.Y <= m_Rectangle.Bottom) &&
                (bulletPos.Y >= m_Rectangle.Top))
            {
                float difference = Math.Abs(bulletPos.X - m_Rectangle.Left);
                pCollisionNormal = new Vector2(-1, 0);
                if(difference > Math.Abs(bulletPos.X - m_Rectangle.Right))
                {
                    difference = Math.Abs(bulletPos.X - m_Rectangle.Right);
                    pCollisionNormal = new Vector2(1, 0);
                }
                if(difference > Math.Abs(bulletPos.Y - m_Rectangle.Top))
                {
                    difference = Math.Abs(bulletPos.Y - m_Rectangle.Top);
                    pCollisionNormal = new Vector2(0, -1);
                }
                if(difference > Math.Abs(bulletPos.Y - m_Rectangle.Bottom))
                {
                    pCollisionNormal = new Vector2(0, 1);
                }
                return true;
            }
            pCollisionNormal = Vector2.Zero;
            return false;
        }

        public bool Collide(Tank pTank)
        {
            Vector2[] tankCorners = new Vector2[4];
            pTank.GetCorners(tankCorners);

            foreach(Vector2 corner in tankCorners)
            {
                if ((corner.X >= m_Rectangle.Left) &&
                    (corner.X <= m_Rectangle.Right) &&
                    (corner.Y <= m_Rectangle.Bottom) &&
                    (corner.Y >= m_Rectangle.Top))
                {
                    return true;
                } 
            }
            return false;
        }
    }
}
