using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tankontroller.Controller;
using Tankontroller.World;

namespace Tankontroller.GUI
{
    public class TeamGUI
    {
        private Texture2D m_WhitePixel;
        private Rectangle m_Rectangle;
        private HealthBar m_HealthBar;
        private Avatar m_Avatar;
        private PowerBar[] m_PowerBars = new PowerBar[8];
        private JackIcon[] m_JackIcons = new JackIcon[8];
        private PortNumLabel[] m_PortNumLabels = new PortNumLabel[8];
        private IController m_Controller;
        private static Texture2D[] m_PortNumbers = new Texture2D[8];
        private static Texture2D m_PowerBarBorderTexture;
        private static Texture2D m_PowerBarPowerTexture;
        private Tank m_Tank;
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
            Tank pTank,
            IController pController,
            Color pColor)
        {
            m_WhitePixel = pWhitePixel;
            m_Rectangle = pRectangle;
            m_Color = pColor;
            m_Controller = pController;
            m_Tank = pTank;
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
           Tank pTank,
           IController pController,
           Color pColor)
        {
            m_WhitePixel = pWhitePixel;
            m_Rectangle = pRectangle;
            m_Color = pColor;
            m_Controller = pController;
            m_Tank = pTank;
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
            m_HealthBar = new HealthBar(m_WhitePixel, pHealthBarBlackAndWhiteLayer, pHealthBarColourLayer, healthBarRectangle, m_Tank);
        }

        public Tank GetTank()
        {
            return m_Tank;
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
            int health = m_Tank.Health();
            if (health == 5)
            {
                avatarIndex = 0;
            }
            else if (health == 4)
            {
                avatarIndex = 1;
            }
            else if (health == 3)
            {
                avatarIndex = 2;
            }
            else if (health == 2)
            {
                avatarIndex = 3;
            }
            else if (health == 1)
            {
                avatarIndex = 4;
            }
            m_Avatar.Draw(pSpriteBatch, m_Tank.Health() > 0, avatarIndex);
        }

        public void Draw(SpriteBatch pSpriteBatch)
        {
            DrawAvatar(pSpriteBatch);
            DrawHealthBar(pSpriteBatch);

            for (int j = 0; j < 7; j++)
            {
                if (m_Controller.GetJackControl(PortMapping.getPortForPlayer(j)) == Control.FIRE)
                {
                    if (m_Controller.GetJackCharge(PortMapping.getPortForPlayer(j)) >= DGS.Instance.GetFloat("BULLET_CHARGE_DEPLETION"))
                    {
                        m_PowerBars[j].Draw(pSpriteBatch, m_Controller.GetJackCharge(PortMapping.getPortForPlayer(j)), true);
                    }
                    else
                    {
                        m_PowerBars[j].Draw(pSpriteBatch, m_Controller.GetJackCharge(PortMapping.getPortForPlayer(j)), false);
                    }
                }
                else
                {
                    m_PowerBars[j].Draw(pSpriteBatch, m_Controller.GetJackCharge(PortMapping.getPortForPlayer(j)), m_Controller.GetJackCharge(PortMapping.getPortForPlayer(j)) > 0);
                }
                m_JackIcons[j].Draw(pSpriteBatch, m_Controller.GetJackControl(PortMapping.getPortForPlayer(j)));
                m_PortNumLabels[j].Draw(pSpriteBatch, j, m_Color);
            }
        }
    }
}
