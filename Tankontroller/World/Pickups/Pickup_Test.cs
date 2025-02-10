using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tankontroller.Managers;

namespace Tankontroller.World.Pickups
{
    internal class Pickup_Test : Pickup
    {
        public Pickup_Test()
            : base(Tankontroller.Instance().CM().Load<Texture2D>("circle"), new Rectangle(400, 500, 10, 10))
        {
        }

        public override void PickUpCollision(Tank tank)
        {
            //Check if the tank has collided with the pickup using the collision manager
            if (CollisionManager.Collide(tank, m_Pickup_Rect, false))
            {
                //If the tank has collided with the pickup, set the pickup to inactive
                m_Active = false;
            }
        }
    }
}
