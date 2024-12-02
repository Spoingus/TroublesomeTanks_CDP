using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tankontroller.GUI
{
    //-------------------------------------------------------------------------------------------------
    // Avatar
    //
    // This class is used to represent the avatar of a player. It displays the player's avatar in the game.
    // It contains the following:
    // - A list of black and white / colour layers of the avatar
    // - A white pixel texture
    // - A rectangle representing the position of the avatar
    // - An array of rectangles representing the position of the avatar's layers
    // - A colour representing the player's colour
    // - A name representing the player's name
    // - A method to draw the avatar
    // - A method to draw the bounds of the avatar
    // - A method to reposition the avatar
    //-------------------------------------------------------------------------------------------------

    public class Avatar
    {
        private Texture2D[] m_BlackAndWhiteLayer = new Texture2D[5];
        private Texture2D[] m_ColourLayer = new Texture2D[5];
        private Texture2D m_WhitePixel;
        private Rectangle m_Rectangle;
        private Rectangle[] m_DrawRectangles = new Rectangle[5];
        private Color m_Colour;
        private string m_Name;

        public Avatar(Texture2D pWhitePixel, Texture2D pBlackAndWhiteLayer, Texture2D pColourLayer, Rectangle pRectangle, Color pColour)
        {
            m_BlackAndWhiteLayer[0] = pBlackAndWhiteLayer;
            m_ColourLayer[0] = pColourLayer;
            m_Rectangle = pRectangle;
            m_Colour = pColour;
            m_WhitePixel = pWhitePixel;
            PrepareDrawRectangles();
        }

        public Avatar(Texture2D pWhitePixel, string pName, Rectangle pRectangle, Color pColour)
        {
            Tankontroller game = (Tankontroller)Tankontroller.Instance();
            for (int i = 0; i < m_BlackAndWhiteLayer.Length; i++)
            {
                string blackAndWhiteFileName = "avatars/avatar_" + pName + "_bw_0" + (i + 1);
                string colourFileName = "avatars/avatar_" + pName + "_colour_0" + (i + 1);
                m_BlackAndWhiteLayer[i] = game.CM().Load<Texture2D>(blackAndWhiteFileName);
                m_ColourLayer[i] = game.CM().Load<Texture2D>(colourFileName);
            }
            m_Rectangle = pRectangle;
            m_Colour = pColour;
            m_WhitePixel = pWhitePixel;
            m_Name = pName;
            PrepareDrawRectangles();
        }

        private void PrepareDrawRectangles()
        {
            int padding = 5;
            int maxAvatarWidth = m_Rectangle.Width - 2 * padding;
            int maxAvatarHeight = m_Rectangle.Height - 2 * padding;
            for (int i = 0; i < m_DrawRectangles.Length; i++)
            {
                float avatarRatio = (float)m_BlackAndWhiteLayer[i].Width / m_BlackAndWhiteLayer[i].Height;
                int avatarHeight = maxAvatarHeight;
                int avatarWidth = (int)(avatarHeight * avatarRatio);
                if (avatarWidth > maxAvatarWidth)
                {
                    avatarWidth = maxAvatarWidth;
                    avatarHeight = (int)(avatarWidth / avatarRatio);
                }
                int avatarLeft = m_Rectangle.Left + (m_Rectangle.Width - avatarWidth) / 2;
                int avatarTop = m_Rectangle.Top + (m_Rectangle.Height - avatarHeight) / 2;
                m_DrawRectangles[i] = new Rectangle(avatarLeft, avatarTop, avatarWidth, avatarHeight);
            }
        }
        public string GetName()
        {
            return m_Name;
        }
        public Color GetColour() { return m_Colour; }
        public void SetColour(Color pColour) { m_Colour = pColour; }

        public void Reposition(Rectangle pRectangle)
        {
            m_Rectangle = pRectangle;
            PrepareDrawRectangles();
        }

        public void DrawBounds(SpriteBatch pSpriteBatch)
        {
            Color boundColour = m_Colour;
            boundColour.A = (byte)0.5f;
            pSpriteBatch.Draw(m_WhitePixel, m_Rectangle, boundColour);
        }

        public void Draw(SpriteBatch pSpriteBatch, bool pAlive, int pIndex)
        {
            //DrawBounds(pSpriteBatch);
            if (pAlive)
            {
                pSpriteBatch.Draw(m_BlackAndWhiteLayer[pIndex], m_DrawRectangles[pIndex], Color.White);
                pSpriteBatch.Draw(m_ColourLayer[pIndex], m_DrawRectangles[pIndex], m_Colour);
            }
            else
            {
                pSpriteBatch.Draw(m_BlackAndWhiteLayer[pIndex], m_DrawRectangles[pIndex], Color.Red);
                pSpriteBatch.Draw(m_ColourLayer[pIndex], m_DrawRectangles[pIndex], Color.Red);
            }
        }
    }
}