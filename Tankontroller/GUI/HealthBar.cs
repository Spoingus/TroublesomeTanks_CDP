using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tankontroller.World;

namespace Tankontroller.GUI
{
    //-------------------------------------------------------------------------------------------------
    // HealthBar
    //
    // This class is used to represent the health bar of a tank. It displays the health bar in the game.
    //
    // It contains the following member variables:
    // - A black and white heart texture
    // - A colour heart texture
    // - A white pixel texture
    // - A rectangle representing the position of the health bar
    // - An array of rectangles representing the position of the health bar's hearts
    // - A tank
    //
    // It contains the following methods:
    // - A constructor to initialise the health bar
    // - A method to reposition the health bar
    // - A method to draw the health bar
    //-------------------------------------------------------------------------------------------------
    public class HealthBar
    {
        private Texture2D mHeartBlackAndWhite;
        private Texture2D mHeartColour;
        private Texture2D mWhitePixel;
        private Rectangle mBoundsRectangle;
        private Rectangle[] mRectangles = new Rectangle[DGS.Instance.GetInt("MAX_TANK_HEALTH")];
        private Tank mTank;

        public HealthBar(Texture2D pWhitePixel, Texture2D pHeartBlackAndWhite, Texture2D pHeartColour, Rectangle pRectangle, Tank pTank)
        {
            mHeartBlackAndWhite = pHeartBlackAndWhite;
            mHeartColour = pHeartColour;
            mWhitePixel = pWhitePixel;
            mBoundsRectangle = pRectangle;
            PrepareRectangles();
            mTank = pTank;

        }

        private void PrepareRectangles()
        {
            int paddingWidth = 5;
            int width = (mBoundsRectangle.Width - paddingWidth * (DGS.Instance.GetInt("MAX_TANK_HEALTH") + 1)) / DGS.Instance.GetInt("MAX_TANK_HEALTH");
            float heightRatio = (float)mHeartBlackAndWhite.Height / mHeartBlackAndWhite.Width;
            int height = (int)(width * heightRatio);
            if (height > mBoundsRectangle.Height)
            {
                height = mBoundsRectangle.Height;
                width = (int)(height / heightRatio);
            }
            int left = mBoundsRectangle.Left + paddingWidth;
            int top = mBoundsRectangle.Top;
            if (height < mBoundsRectangle.Height)
            {
                top += (mBoundsRectangle.Height - height) / 2;
            }
            for (int i = 0; i < DGS.Instance.GetInt("MAX_TANK_HEALTH"); i++)
            {
                Rectangle rectangle = new Rectangle(left, top, width, height);
                mRectangles[i] = rectangle;
                left += paddingWidth + width;
            }


        }

        public void Reposition(Rectangle pRectangle)
        {
            mBoundsRectangle = pRectangle;
            PrepareRectangles();
        }

        public void DrawBounds(SpriteBatch pSpriteBatch)
        {
            Color boundColour = Color.White;
            boundColour.A = (byte)0.5f;
            pSpriteBatch.Draw(mWhitePixel, mBoundsRectangle, boundColour);
        }

        public void Draw(SpriteBatch pSpriteBatch)
        {
            //DrawBounds(pSpriteBatch);
            for (int i = 0; i < DGS.Instance.GetInt("MAX_TANK_HEALTH"); i++)
            {
                pSpriteBatch.Draw(mHeartBlackAndWhite, mRectangles[i], Color.White);
                if (i < mTank.Health())
                {
                    pSpriteBatch.Draw(mHeartColour, mRectangles[i], Color.Red);
                }
            }

        }
    }
}
