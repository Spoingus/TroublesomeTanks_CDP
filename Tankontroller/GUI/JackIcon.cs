using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tankontroller.Controller;

namespace Tankontroller.GUI
{
    class JackIcon
    {
        private static Texture2D[] icons;
        private Vector2 iconPosition;
        private int iconWidth, iconHeight;

        public static void SetupStaticTextures(Texture2D pIcon1, Texture2D pIcon2, Texture2D pIcon3, Texture2D pIcon4, Texture2D pIcon5, Texture2D pIcon6, Texture2D pIcon7, Texture2D pIcon8, Texture2D pIcon9)
        {
            icons = new Texture2D[9];
            icons[0] = pIcon1;
            icons[1] = pIcon2;
            icons[2] = pIcon3;
            icons[3] = pIcon4;
            icons[4] = pIcon5;
            icons[5] = pIcon6;
            icons[6] = pIcon7;
            icons[7] = pIcon8;
            icons[8] = pIcon9;
        }

        public JackIcon(Vector2 pPos, int pWidth, int pHeight)
        {
            iconPosition = pPos;
            iconWidth = pWidth;
            iconHeight = pHeight;
        }

        public void Draw(SpriteBatch pSpriteBatch, Control pControl)
        {

            Texture2D icon = icons[6];

            if (pControl == Control.LEFT_TRACK_FORWARDS)
                icon = icons[0];
            else if (pControl == Control.LEFT_TRACK_BACKWARDS)
                icon = icons[1];
            else if (pControl == Control.RIGHT_TRACK_FORWARDS)
                icon = icons[2];
            else if (pControl == Control.RIGHT_TRACK_BACKWARDS)
                icon = icons[3];
            else if (pControl == Control.FIRE)
                icon = icons[4];
            else if (pControl == Control.RECHARGE)
                icon = icons[5];
            else if (pControl == Control.NONE)
                icon = icons[6];
            else if (pControl == Control.TURRET_LEFT)
                icon = icons[7];
            else if (pControl == Control.TURRET_RIGHT)
                icon = icons[8];

            pSpriteBatch.Draw(icon, new Rectangle((int)iconPosition.X, (int)iconPosition.Y, iconWidth, iconHeight), Color.White);
        }
    }
}
