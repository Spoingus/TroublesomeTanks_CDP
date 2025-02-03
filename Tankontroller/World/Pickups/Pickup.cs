using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tankontroller.World.Pickups
{
    public abstract class Pickup
    {
        private Rectangle m_Pickup_Rect;
        private Texture2D m_Texture;
        protected bool m_Active = true;

        protected Pickup(Texture2D pTexture, Rectangle pRectangle) { m_Pickup_Rect = pRectangle; m_Texture = pTexture; }

        public void Draw(SpriteBatch pSpriteBatch)
        {
            if (m_Active)
                pSpriteBatch.Draw(m_Texture, m_Pickup_Rect, Color.DeepPink);
        }

        public bool Collide(Tank pTank)
        {
            if (!m_Active)
            {
                return false;
            }
            Vector2[] tankCorners = new Vector2[4];
            pTank.GetCorners(tankCorners);

            foreach (Vector2 corner in tankCorners)
            {
                if (m_Pickup_Rect.Contains(corner))
                {
                    return true;
                }
            }
            if (pTank.PointIsInTank(new Vector2(m_Pickup_Rect.Left, m_Pickup_Rect.Top)) ||
            pTank.PointIsInTank(new Vector2(m_Pickup_Rect.Right, m_Pickup_Rect.Top)) ||
            pTank.PointIsInTank(new Vector2(m_Pickup_Rect.Left, m_Pickup_Rect.Bottom)) ||
               pTank.PointIsInTank(new Vector2(m_Pickup_Rect.Right, m_Pickup_Rect.Bottom)))
            {
                return true;
            }
            return false;
        }
    }
}
