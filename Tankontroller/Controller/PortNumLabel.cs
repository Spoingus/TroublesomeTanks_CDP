using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tankontroller.Controller
{
    public class PortNumLabel
    {
        private Texture2D[] numTextures;
        private Vector2 numPosition;
        private int numWidth, numHeight;

        public PortNumLabel(Texture2D[] pNumTextures, Vector2 pPos, int pWidth, int pHeight)
        {
            numTextures = pNumTextures;
            numPosition = pPos;
            numWidth = pWidth;
            numHeight = pHeight;
        }

        public void Draw(SpriteBatch pSpriteBatch, int pNumToDisplay, Color pColor)
        {
            pSpriteBatch.Draw(numTextures[pNumToDisplay], new Rectangle((int)numPosition.X, (int)numPosition.Y, numWidth, numHeight), pColor);

        }

    }
}
