using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tankontroller.Controller;
using Tankontroller.World;

namespace Tankontroller.GUI
{
    //-------------------------------------------------------------------------------------------------
    // TeamGUI
    //
    // This class is used to draw and update the GUI for a player. It calls the draw method on the
    // the players Avatar and healthbar.
    //-------------------------------------------------------------------------------------------------
    public class TeamGUI
    {
        private Rectangle m_Rectangle;
        private HealthBar m_HealthBar;
        private Avatar m_Avatar;
        private PowerBar[] m_PowerBars = new PowerBar[8];
        private JackIcon[] m_JackIcons = new JackIcon[8];
        private PortNumLabel[] m_PortNumLabels = new PortNumLabel[8];
        private static Texture2D[] m_PortNumbers = new Texture2D[8];
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
           Avatar pAvatar,
           Rectangle pRectangle,
           IController pController,
           Color pColor)
        {
            m_Rectangle = pRectangle;
            m_Color = pColor;
            mController = pController;
            m_Avatar = pAvatar;
            m_Avatar.Reposition(GetAvatarRect());
            m_HealthBar = new HealthBar(Tank.MAX_HEALTH, GetHealthBarRect());

            int screenWidth = Tankontroller.Instance().GDM().GraphicsDevice.Viewport.Width;
            int screenHeight = Tankontroller.Instance().GDM().GraphicsDevice.Viewport.Height;

            int powerBarWidth = screenWidth / 4 /* 25% of screen width */ * 9 / 19 /* Just Under Three quarters */ / 8;
            // This is also used as BOTH width and height for square icon and label textures
            int powerBarHeight = screenHeight / 100 * 12;

            int powerBar_yValueOffset = -10 + screenWidth / 100 * 2;
            int jackIcons_yValueOffset = powerBar_yValueOffset + (int)(powerBarHeight * 1.01f) - powerBarWidth;
            int labels_yValueOffset = powerBar_yValueOffset + (int)(powerBarWidth * 1.01f);

            bool isOnLeft = true;
            int xValue = screenWidth / 100 * 1;
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

        private Rectangle GetAvatarRect()
        {
            int avatarWidth = (int)(m_Rectangle.Width * 0.4);
            int avatarHeight = m_Rectangle.Height;
            return new Rectangle(m_Rectangle.X, m_Rectangle.Y, avatarWidth, avatarHeight);
        }
        private Rectangle GetHealthBarRect()
        {
            int healthBarWidth = (int)(m_Rectangle.Width * 0.6);
            int healthBarHeight = (int)(m_Rectangle.Height * 0.25);
            int healthBarLeft = m_Rectangle.Left + m_Rectangle.Width - healthBarWidth;
            int healthBarTop = m_Rectangle.Top + m_Rectangle.Height - healthBarHeight;
            return new Rectangle(healthBarLeft, healthBarTop, healthBarWidth, healthBarHeight);
        }

        public void DrawHealthBar(SpriteBatch pSpriteBatch, int pHealth)
        {
            m_HealthBar.Draw(pSpriteBatch, pHealth);
        }

        public void Reposition(Rectangle pRectangle)
        {
            m_Avatar.Reposition(GetAvatarRect());
            m_HealthBar.Reposition(GetHealthBarRect());
        }

        public void RepositionForGameOver(Rectangle pRectangle)
        {
            Rectangle healthRect = new Rectangle(new Point(pRectangle.X + (pRectangle.Height / 3), pRectangle.Y + (pRectangle.Height - 15)), new Point(pRectangle.Width, pRectangle.Height / 4));
            m_HealthBar.Reposition(healthRect);
            m_Avatar.Reposition(pRectangle);
        }

        public void DrawAvatar(SpriteBatch pSpriteBatch, int pHealth)
        {
            int avatarIndex = Tank.MAX_HEALTH - pHealth; // index 0 is full health, index 4 is no health
            m_Avatar.Draw(pSpriteBatch, pHealth > 0, avatarIndex);
        }

        public void Draw(SpriteBatch pSpriteBatch, int pHealth)
        {
            DrawAvatar(pSpriteBatch, pHealth);
            DrawHealthBar(pSpriteBatch, pHealth);
            for (int j = 0; j < 7; j++)
            {
                int port = PortMapping.GetPortForPlayer(j);
                float currentCharge = mController.GetJackCharge(port);
                Control currentControl = mController.GetJackControl(port);
                bool hasCharge = currentControl == Control.FIRE ? currentCharge >= Player.BULLET_CHARGE_DEPLETION : currentCharge > 0;

                m_PowerBars[j].Draw(pSpriteBatch, currentCharge, hasCharge);
                m_JackIcons[j].Draw(pSpriteBatch, currentControl);
                m_PortNumLabels[j].Draw(pSpriteBatch, j, m_Color);
            }
        }
    }
}