using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tankontroller.World.Pickups
{
    public class EMPPickup : Pickup
    {
        private static readonly Texture2D mEMPTexture = Tankontroller.Instance().CM().Load<Texture2D>("battery");

        public EMPPickup(Vector2 position) : base(Tankontroller.Instance().CM().Load<Texture2D>("circle"), new Rectangle(400, 500, 40, 40))
        {
            m_Pickup_Rect = new Rectangle((int)position.X, (int)position.Y, 40, 40);
        }

        public override void Draw(SpriteBatch pSpriteBatch)
        {
            if (m_Active) {
                pSpriteBatch.Draw(mEMPTexture, m_Pickup_Rect, Color.White);
            }
        }

        public override void PickUpCollision(Tank tank)
        {
            if (Collide(tank))
            {
                //tank.SetBulletType(BulletType.BOUNCY_EMP);
                m_Active = false;
            }
        }
    }
}
