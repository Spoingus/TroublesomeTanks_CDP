using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tankontroller.World;

namespace Tankontroller.GUI
{
    public class Avatar
    {
        private Texture2D[] mBlackAndWhiteLayer = new Texture2D[5];
        private Texture2D[] mColourLayer = new Texture2D[5];
        private Texture2D mWhitePixel;
        private Rectangle mBoundsRectangle;
        private Rectangle[] mDrawRectangles = new Rectangle[5];
        private Color mColour;
        private string mName;

        public Avatar(Texture2D pWhitePixel, Texture2D pBlackAndWhiteLayer, Texture2D pColourLayer, Rectangle pRectangle, Color pColour)
        {
            mBlackAndWhiteLayer[0] = pBlackAndWhiteLayer;
            mColourLayer[0] = pColourLayer;
            mBoundsRectangle = pRectangle;
            mColour = pColour;
            mWhitePixel = pWhitePixel;
            PrepareDrawRectangles();
        }

        public Avatar(Texture2D pWhitePixel, string pName, Rectangle pRectangle, Color pColour)
        {
            Tankontroller game = (Tankontroller)Tankontroller.Instance();
            for (int i = 0; i < mBlackAndWhiteLayer.Length; i++)
            {
                string blackAndWhiteFileName = "avatars/avatar_" + pName + "_bw_0" + (i + 1);
                string colourFileName = "avatars/avatar_" + pName + "_colour_0" + (i + 1);
                mBlackAndWhiteLayer[i] = game.CM().Load<Texture2D>(blackAndWhiteFileName);
                mColourLayer[i] = game.CM().Load<Texture2D>(colourFileName);
            }
            mBoundsRectangle = pRectangle;
            mColour = pColour;
            mWhitePixel = pWhitePixel;
            mName = pName;
            PrepareDrawRectangles();
        }

        private void PrepareDrawRectangles()
        {
            int padding = 5;
            int maxAvatarWidth = mBoundsRectangle.Width - 2 * padding;
            int maxAvatarHeight = mBoundsRectangle.Height - 2 * padding;
            for (int i = 0; i < mDrawRectangles.Length; i++)
            {
                float avatarRatio = (float)mBlackAndWhiteLayer[i].Width / mBlackAndWhiteLayer[i].Height;
                int avatarHeight = maxAvatarHeight;
                int avatarWidth = (int)(avatarHeight * avatarRatio);
                if (avatarWidth > maxAvatarWidth)
                {
                    avatarWidth = maxAvatarWidth;
                    avatarHeight = (int)(avatarWidth / avatarRatio);
                }
                int avatarLeft = mBoundsRectangle.Left + (mBoundsRectangle.Width - avatarWidth) / 2;
                int avatarTop = mBoundsRectangle.Top + (mBoundsRectangle.Height - avatarHeight) / 2;
                mDrawRectangles[i] = new Rectangle(avatarLeft, avatarTop, avatarWidth, avatarHeight);
            }
        }
        public string GetName()
        {
            return mName;
        }
        public Color GetColour() { return mColour; }
        public void SetColour(Color pColour) { mColour = pColour; }

        public void Reposition(Rectangle pRectangle)
        {
            mBoundsRectangle = pRectangle;
            PrepareDrawRectangles();
        }

        public void DrawBounds(SpriteBatch pSpriteBatch)
        {
            Color boundColour = mColour;
            boundColour.A = (byte)0.5f;
            pSpriteBatch.Draw(mWhitePixel, mBoundsRectangle, boundColour);
        }

        public void Draw(SpriteBatch pSpriteBatch, bool pAlive, int pIndex)
        {
            //DrawBounds(pSpriteBatch);
            if (pAlive)
            {
                pSpriteBatch.Draw(mBlackAndWhiteLayer[pIndex], mDrawRectangles[pIndex], Color.White);
                pSpriteBatch.Draw(mColourLayer[pIndex], mDrawRectangles[pIndex], mColour);
            }
            else
            {
                pSpriteBatch.Draw(mBlackAndWhiteLayer[pIndex], mDrawRectangles[pIndex], Color.Red);
                pSpriteBatch.Draw(mColourLayer[pIndex], mDrawRectangles[pIndex], Color.Red);
            }
        }
    }
}