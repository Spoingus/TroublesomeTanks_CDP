using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tankontroller.World.Pickups
{
    public class HealthPickup : Pickup
    {
        private static readonly Texture2D mHeartBack = Tankontroller.Instance().CM().Load<Texture2D>("healthbars/heart_bw");
        private static readonly Texture2D mHeartColour = Tankontroller.Instance().CM().Load<Texture2D>("healthbars/heart_colour");

        public HealthPickup(Vector2 position) : base(Tankontroller.Instance().CM().Load<Texture2D>("circle"), new Rectangle(400, 500, 40, 40))
        {
            m_Pickup_Rect = new Rectangle((int)position.X, (int)position.Y, 40, 40);
        }

        public override void Draw(SpriteBatch pSpriteBatch)
        {
            if (m_Active) {
                pSpriteBatch.Draw(mHeartBack, m_Pickup_Rect, Color.White);
                pSpriteBatch.Draw(mHeartColour, m_Pickup_Rect, Color.Red);
            }
        }

        public override void PickUpCollision(Tank tank)
        {
            if (Collide(tank))
            {
                // Stop Drawing
                tank.Heal();
                m_Active = false;
            }
        }
    }
}
