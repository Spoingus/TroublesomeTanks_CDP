using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tankontroller.Controller
{
    //--------------------------------------------------------------------------------
    // PortNumLabel
    //
    // This class is used to display the port number on the screen.
    // It contains the following:
    //  - Texture2D[] numTextures: An array of textures for each number
    //  - Vector2 numPosition: The position to draw the number
    //  - int numWidth / int numHeight: The width / height of the number
    //  - Draw: Draws the number to the screen
    //--------------------------------------------------------------------------------
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
