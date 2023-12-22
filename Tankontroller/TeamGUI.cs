using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tankontroller.World;

namespace Tankontroller
{
    public class TeamGUI
    {
        private HealthBar m_HealthBar;
        private PowerBar[] m_PowerBars = new PowerBar[8];
        private JackIcon[] m_JackIcons = new JackIcon[8];
        private PortNumLabel[] m_PortNumLabels = new PortNumLabel[8];
        private IController m_Controller;
        private static Texture2D[] m_PortNumbers = new Texture2D[8];
        private static Texture2D m_PowerBarBorderTexture;
        private static Texture2D m_PowerBarPowerTexture;
        private Tank mTank;
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
            Texture2D pHealthBar1,
            Texture2D pHealthBar2,
            Texture2D pHealthBar3,
            Texture2D pHealthBar4,
            Texture2D pHealthBar5,
            Texture2D pHealthBar6,
            Rectangle pRectangle,
            Tank pTank,
            bool isOnLeft,
            IController pController,
            Color pColor)
        {
            m_Color = pColor;
            m_Controller = pController;
            m_HealthBar = new HealthBar(pHealthBar1, pHealthBar2, pHealthBar3, pHealthBar4, pHealthBar5, pHealthBar6, pRectangle, pTank);

            int powerBarWidth = (DGS.Instance.GetInt("SCREENWIDTH") / 4) /* 25% of screen width */ * 9 / 19 /* Just Under Three quarters */ / 8; // This is also used as BOTH width and height for square icon and label textures
            int powerBarHeight = DGS.Instance.GetInt("SCREENHEIGHT") / 100 * 12;

            int powerBar_yValueOffset = -10+ DGS.Instance.GetInt("SCREENWIDTH") / 100 * 2;
            int jackIcons_yValueOffset = powerBar_yValueOffset + (int)(powerBarHeight * 1.01f) - powerBarWidth;
            int labels_yValueOffset = powerBar_yValueOffset + (int)(powerBarWidth * 1.01f);

            mTank = pTank;

            int xValue = DGS.Instance.GetInt("SCREENWIDTH") / 100 * 1;
            int xIncrement = Convert.ToInt32(powerBarWidth * 1.2);
            int xOffset = isOnLeft ? pRectangle.X + pRectangle.Width - (8 * xIncrement) - powerBarWidth : pRectangle.X;
            for (int j = 0; j < 7; j++)
            {
                m_PowerBars[j] = new PowerBar(new Vector2(xOffset + xValue, powerBar_yValueOffset), powerBarWidth, powerBarHeight);
                m_JackIcons[j] = new JackIcon(new Vector2(xOffset + xValue, jackIcons_yValueOffset), powerBarWidth, powerBarWidth);
                m_PortNumLabels[j] = new PortNumLabel(m_PortNumbers, new Vector2(xOffset + xValue, labels_yValueOffset), powerBarWidth, powerBarWidth);
                xValue += xIncrement;
            }
        }

        public Tank GetTank()
        {
            return mTank;
        }

        public void DrawHealthBar(SpriteBatch pSpriteBatch)
        {
            m_HealthBar.Draw(pSpriteBatch);
        }

        public void Reposition(Rectangle pRectangle)
        {
            m_HealthBar.Reposition(pRectangle);
        }

        public void Draw(SpriteBatch pSpriteBatch)
        {
            DrawHealthBar(pSpriteBatch);

            for (int j = 0; j < 7; j++)
            {
                if(m_Controller.GetJackControl(PortMapping.getPortForPlayer(j)) == Control.FIRE)
                {
                    if(m_Controller.GetJackCharge(PortMapping.getPortForPlayer(j)) >= DGS.Instance.GetFloat("BULLET_CHARGE_DEPLETION"))
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
