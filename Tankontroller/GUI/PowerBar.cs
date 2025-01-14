using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tankontroller.GUI
{
    //-------------------------------------------------------------------------------------------------
    // PowerBar
    //
    // This class is used to represent the power bar of a tank. It displays the power bar in the game.
    //
    // It contains the following member variables:
    // - A border texture
    // - A power texture
    // - A position of the power bar
    // - A width of the power bar
    // - A height of the power bar
    //
    // It contains the following methods:
    // - A constructor to initialise the power bar
    // - A method to draw the power bar
    //-------------------------------------------------------------------------------------------------
    public class PowerBar
    {
        private static Texture2D powerBarBorder, powerBarPower;
        private Vector2 powerBarPosition;
        private int powerBarWidth, powerBarHeight;

        public static void SetupStaticTextures(Texture2D pPowerBarBorder, Texture2D pPowerBarPower)
        {
            powerBarBorder = pPowerBarBorder;
            powerBarPower = pPowerBarPower;
        }

        public PowerBar(Vector2 pPos, int pWidth, int pHeight)
        {
            powerBarPosition = pPos;
            powerBarWidth = pWidth;
            powerBarHeight = pHeight;
        }

        public void Draw(SpriteBatch pSpriteBatch, float pCharge, bool enoughCharge)
        {

            pSpriteBatch.Draw(powerBarBorder, new Rectangle((int)powerBarPosition.X, (int)powerBarPosition.Y, powerBarWidth, powerBarHeight), null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0f);

            if (pCharge > 0)
            {
                int height = (int)(powerBarHeight / 100f * (100 / DGS.Instance.GetFloat("MAX_CHARGE") * pCharge));
                if (enoughCharge)
                {
                    pSpriteBatch.Draw(powerBarPower, new Rectangle((int)powerBarPosition.X, (int)powerBarPosition.Y + powerBarHeight - height, powerBarWidth, height), null, Color.LightBlue, 0f, new Vector2(0, 0), SpriteEffects.None, 0f);
                }
                else
                {
                    pSpriteBatch.Draw(powerBarPower, new Rectangle((int)powerBarPosition.X, (int)powerBarPosition.Y, powerBarWidth, powerBarHeight), null, Color.Red, 0f, new Vector2(0, 0), SpriteEffects.None, 0f);
                    pSpriteBatch.Draw(powerBarPower, new Rectangle((int)powerBarPosition.X, (int)powerBarPosition.Y + powerBarHeight - height, powerBarWidth, height), null, Color.LightBlue, 0f, new Vector2(0, 0), SpriteEffects.None, 0f);
                }
                //pSpriteBatch.DrawString(gameFont, pCharge, new Vector2(665, 40), Color.Wheat, 0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0f);
            }

            else if (pCharge == 0)
            {
                pSpriteBatch.Draw(powerBarPower, new Rectangle((int)powerBarPosition.X, (int)powerBarPosition.Y, powerBarWidth, powerBarHeight), null, Color.Red, 0f, new Vector2(0, 0), SpriteEffects.None, 0f);
                //pSpriteBatch.DrawString(gameFont, "No Power!", new Vector2(625, 40), Color.Red, 0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0f);

            }
        }

    }
}
