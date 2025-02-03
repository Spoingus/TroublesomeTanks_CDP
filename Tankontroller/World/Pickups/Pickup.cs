using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tankontroller.World.Pickups
{
    public abstract class Pickup
    {
        protected Rectangle m_Pickup_Rect;
        protected Texture2D m_Texture;
        protected bool m_Active = true;

        protected Pickup(Texture2D pTexture, Rectangle pRectangle) { m_Pickup_Rect = pRectangle; m_Texture = pTexture; }

        public void Draw(SpriteBatch pSpriteBatch)
        {
            if (m_Active)
                pSpriteBatch.Draw(m_Texture, m_Pickup_Rect, Color.DeepPink);
        }
    }
}
