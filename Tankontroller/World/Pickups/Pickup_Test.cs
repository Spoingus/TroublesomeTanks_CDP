using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tankontroller.World.Pickups
{
    internal class Pickup_Test : Pickup
    {
        public Pickup_Test()
            : base(Tankontroller.Instance().CM().Load<Texture2D>("circle"), new Rectangle(400, 500, 10, 10))
        {
        }

        public void Pickup_Test_Collide(Tank tank)
        {
            if (Collide(tank))
            {
                // Stop Drawing
                m_Active = false;
            }
        }
    }
}
