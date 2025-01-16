using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Tankontroller.Controller;
using Tankontroller.World;

namespace Tankontroller.GUI
{
    //-------------------------------------------------------------------------------------------------
    // TeamGUI
    //
    // This class is used to represent the GUI of a team.
    //
    // It contains the following member variables:
    // - A white pixel texture
    // - A rectangle representing the position of the GUI
    // - A health bar
    // - An avatar
    // - An array of power bars, jack icons and port number labels
    // - A player
    // - A tank
    // - A frame
    // - A colour
    //
    // It contains the following methods:
    // - A method to set up the static textures
    // - A constructor to initialise the GUI
    // - A method to get the tank
    // - A method to draw the health bar
    // - A method to reposition the GUI
    // - A method to draw the avatar
    // - A method to draw the GUI
    //-------------------------------------------------------------------------------------------------
    public class TeamGUI
    {
        private Texture2D m_WhitePixel;
        private Rectangle m_Rectangle;
        private HealthBar m_HealthBar;
        private Avatar m_Avatar;
        private PowerBar[] m_PowerBars = new PowerBar[8];
        private JackIcon[] m_JackIcons = new JackIcon[8];
        private PortNumLabel[] m_PortNumLabels = new PortNumLabel[8];
        private static Texture2D[] m_PortNumbers = new Texture2D[8];
        private int mHealth;
        private IController mController;
        private Vector2 Frame { get; set; }
        private Color m_Color { get; set; }

        public static void SetupStaticTextures(
                Texture2D pPortNumber1,
                Texture2D pPortNumber2,
                Texture2D pPortNumber3,
                Texture2D pPortNumber4,
                Texture2D pPortNumber5,
                Texture2D pPortNumber6,
                Texture2D pPortNumber7,
                Texture2D pPortNumber8
            )
        {
            m_PortNumbers[0] = pPortNumber1;
            m_PortNumbers[1] = pPortNumber2;
            m_PortNumbers[2] = pPortNumber3;
            m_PortNumbers[3] = pPortNumber4;
            m_PortNumbers[4] = pPortNumber5;
            m_PortNumbers[5] = pPortNumber6;
            m_PortNumbers[6] = pPortNumber7;
            m_PortNumbers[7] = pPortNumber8;
        }

        public TeamGUI(
            Texture2D pWhitePixel,
            Texture2D pHealthBarBlackAndWhiteLayer,
            Texture2D pHealthBarColourLayer,
            Texture2D pAvatarBlackAndWhiteLayer,
            Texture2D pAvatarColourLayer,
            Rectangle pRectangle,
            IController pController,
            int pHealth,
            Color pColor)
        {
            m_WhitePixel = pWhitePixel;
            m_Rectangle = pRectangle;
            m_Color = pColor;
            mController = pController;
            mHealth = pHealth;
            PrepareAvatar(pAvatarBlackAndWhiteLayer, pAvatarColourLayer);
            PrepareHealthBar(pHealthBarBlackAndWhiteLayer, pHealthBarColourLayer);

            int powerBarWidth = DGS.Instance.GetInt("SCREENWIDTH") / 4 /* 25% of screen width */ * 9 / 19 /* Just Under Three quarters */ / 8; // This is also used as BOTH width and height for square icon and label textures
            int powerBarHeight = DGS.Instance.GetInt("SCREENHEIGHT") / 100 * 12;

            int powerBar_yValueOffset = -10 + DGS.Instance.GetInt("SCREENWIDTH") / 100 * 2;
            int jackIcons_yValueOffset = powerBar_yValueOffset + (int)(powerBarHeight * 1.01f) - powerBarWidth;
            int labels_yValueOffset = powerBar_yValueOffset + (int)(powerBarWidth * 1.01f);

            bool isOnLeft = true;
            int xValue = DGS.Instance.GetInt("SCREENWIDTH") / 100 * 1;
            int xIncrement = Convert.ToInt32(powerBarWidth * 1.2);
            int xOffset = isOnLeft ? pRectangle.X + pRectangle.Width - 8 * xIncrement - powerBarWidth : pRectangle.X;
            for (int j = 0; j < 7; j++)
            {
                m_PowerBars[j] = new PowerBar(new Vector2(xOffset + xValue, powerBar_yValueOffset), powerBarWidth, powerBarHeight);
                m_JackIcons[j] = new JackIcon(new Vector2(xOffset + xValue, jackIcons_yValueOffset), powerBarWidth, powerBarWidth);
                m_PortNumLabels[j] = new PortNumLabel(m_PortNumbers, new Vector2(xOffset + xValue, labels_yValueOffset), powerBarWidth, powerBarWidth);
                xValue += xIncrement;
            }
        }
        public TeamGUI(
           Texture2D pWhitePixel,
           Texture2D pHealthBarBlackAndWhiteLayer,
           Texture2D pHealthBarColourLayer,
           Avatar pAvatar,
           Rectangle pRectangle,
           IController pController,
           int pHealth,
           Color pColor)
        {
            m_WhitePixel = pWhitePixel;
            m_Rectangle = pRectangle;
            m_Color = pColor;
            mController = pController;
            mHealth = pHealth;
            PrepareAvatar(pAvatar);
            PrepareHealthBar(pHealthBarBlackAndWhiteLayer, pHealthBarColourLayer);

            int powerBarWidth = DGS.Instance.GetInt("SCREENWIDTH") / 4 /* 25% of screen width */ * 9 / 19 /* Just Under Three quarters */ / 8; // This is also used as BOTH width and height for square icon and label textures
            int powerBarHeight = DGS.Instance.GetInt("SCREENHEIGHT") / 100 * 12;

            int powerBar_yValueOffset = -10 + DGS.Instance.GetInt("SCREENWIDTH") / 100 * 2;
            int jackIcons_yValueOffset = powerBar_yValueOffset + (int)(powerBarHeight * 1.01f) - powerBarWidth;
            int labels_yValueOffset = powerBar_yValueOffset + (int)(powerBarWidth * 1.01f);

            bool isOnLeft = true;
            int xValue = DGS.Instance.GetInt("SCREENWIDTH") / 100 * 1;
            int xIncrement = Convert.ToInt32(powerBarWidth * 1.2);
            int xOffset = isOnLeft ? pRectangle.X + pRectangle.Width - 8 * xIncrement - powerBarWidth : pRectangle.X;
            for (int j = 0; j < 7; j++)
            {
                m_PowerBars[j] = new PowerBar(new Vector2(xOffset + xValue, powerBar_yValueOffset), powerBarWidth, powerBarHeight);
                m_JackIcons[j] = new JackIcon(new Vector2(xOffset + xValue, jackIcons_yValueOffset), powerBarWidth, powerBarWidth);
                m_PortNumLabels[j] = new PortNumLabel(m_PortNumbers, new Vector2(xOffset + xValue, labels_yValueOffset), powerBarWidth, powerBarWidth);
                xValue += xIncrement;
            }
        }

        private void PrepareAvatar(Texture2D pAvatarBlackAndWhiteLayer, Texture2D pAvatarColourLayer)
        {
            int avatarWidth = (int)(m_Rectangle.Width * 0.4);
            int avatarHeight = m_Rectangle.Height;
            Rectangle avatarRectangle = new Rectangle(m_Rectangle.X, m_Rectangle.Y, avatarWidth, avatarHeight);
            m_Avatar = new Avatar(m_WhitePixel, pAvatarBlackAndWhiteLayer, pAvatarColourLayer, avatarRectangle, m_Color);
        }
        private void PrepareAvatar(Avatar pAvatar)
        {
            int avatarWidth = (int)(m_Rectangle.Width * 0.4);
            int avatarHeight = m_Rectangle.Height;
            Rectangle avatarRectangle = new Rectangle(m_Rectangle.X, m_Rectangle.Y, avatarWidth, avatarHeight);
            m_Avatar = pAvatar;
            m_Avatar.Reposition(avatarRectangle);
        }
        private void PrepareHealthBar(Texture2D pHealthBarBlackAndWhiteLayer, Texture2D pHealthBarColourLayer)
        {
            int healthBarWidth = (int)(m_Rectangle.Width * 0.6);
            int healthBarHeight = (int)(m_Rectangle.Height * 0.25);
            int healthBarLeft = m_Rectangle.Left + m_Rectangle.Width - healthBarWidth;
            int healthBarTop = m_Rectangle.Top + m_Rectangle.Height - healthBarHeight;
            Rectangle healthBarRectangle = new Rectangle(healthBarLeft, healthBarTop, healthBarWidth, healthBarHeight);
            m_HealthBar = new HealthBar(m_WhitePixel, pHealthBarBlackAndWhiteLayer, pHealthBarColourLayer, healthBarRectangle, mHealth);
        }

        public void DrawHealthBar(SpriteBatch pSpriteBatch)
        {
            m_HealthBar.Draw(pSpriteBatch);
        }

        public void Reposition(Rectangle pRectangle)
        {
            Rectangle healthRect = new Rectangle(new Point(pRectangle.X + (pRectangle.Height / 3), pRectangle.Y + (pRectangle.Height - 15)), new Point(pRectangle.Width, pRectangle.Height / 4));
            m_HealthBar.Reposition(healthRect);
            m_Avatar.Reposition(pRectangle);
        }

        public void DrawAvatar(SpriteBatch pSpriteBatch)
        {
            int avatarIndex = 4;
            if (mHealth == 5)
            {
                avatarIndex = 0;
            }
            else if (mHealth == 4)
            {
                avatarIndex = 1;
            }
            else if (mHealth == 3)
            {
                avatarIndex = 2;
            }
            else if (mHealth == 2)
            {
                avatarIndex = 3;
            }
            else if (mHealth == 1)
            {
                avatarIndex = 4;
            }
            m_Avatar.Draw(pSpriteBatch, mHealth > 0, avatarIndex);
        }

        public void Draw(SpriteBatch pSpriteBatch)
        {
            DrawAvatar(pSpriteBatch);
            DrawHealthBar(pSpriteBatch);
            for (int j = 0; j < 7; j++)
            {
                if (mController.GetJackControl(PortMapping.GetPortForPlayer(j)) == Control.FIRE)
                {
                    if (mController.GetJackCharge(PortMapping.GetPortForPlayer(j)) >= DGS.Instance.GetFloat("BULLET_CHARGE_DEPLETION"))
                    {
                        m_PowerBars[j].Draw(pSpriteBatch, mController.GetJackCharge(PortMapping.GetPortForPlayer(j)), true);
                    }
                    else
                    {
                        m_PowerBars[j].Draw(pSpriteBatch, mController.GetJackCharge(PortMapping.GetPortForPlayer(j)), false);
                    }
                }
                else
                {
                    m_PowerBars[j].Draw(pSpriteBatch, mController.GetJackCharge(PortMapping.GetPortForPlayer(j)), mController.GetJackCharge(PortMapping.GetPortForPlayer(j)) > 0);
                }
                m_JackIcons[j].Draw(pSpriteBatch, mController.GetJackControl(PortMapping.GetPortForPlayer(j)));
                m_PortNumLabels[j].Draw(pSpriteBatch, j, m_Color);
            }
        }
    }
}