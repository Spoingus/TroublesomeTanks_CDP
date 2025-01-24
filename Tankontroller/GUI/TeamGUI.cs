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
        private HealthBar m_HealthBar;
        private Avatar m_Avatar;
        private PowerBar[] m_PowerBars = new PowerBar[8];
        private JackIcon[] m_JackIcons = new JackIcon[8];
        private PortNumLabel[] m_PortNumLabels = new PortNumLabel[8];
        private IController mController;
        private Color m_Color { get; set; }
        private Tank.BulletType m_BulletType { get; set; }

        public TeamGUI(Avatar pAvatar,Rectangle pRectangle,IController pController)
        {
            mController = pController;
            m_Avatar = pAvatar;
            m_Color = pAvatar.GetColour();
            m_Avatar.Reposition(GetAvatarRect(pRectangle));
            m_HealthBar = new HealthBar(Tank.MAX_HEALTH, GetHealthBarRect(pRectangle));
            RepositionPowerBar(pRectangle);
        }

        private void RepositionPowerBar(Rectangle pRectangle)
        {
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
                m_PortNumLabels[j] = new PortNumLabel(new Vector2(xOffset + xValue, labels_yValueOffset), powerBarWidth, powerBarWidth);
                xValue += xIncrement;
            }
        }

        private Rectangle GetAvatarRect(Rectangle pRectangle)
        {
            int avatarWidth = (int)(pRectangle.Width * 0.4);
            int avatarHeight = pRectangle.Height;
            return new Rectangle(pRectangle.X, pRectangle.Y, avatarWidth, avatarHeight);
        }
        private Rectangle GetHealthBarRect(Rectangle pRectangle)
        {
            int healthBarWidth = (int)(pRectangle.Width * 0.6);
            int healthBarHeight = (int)(pRectangle.Height * 0.25);
            int healthBarLeft = pRectangle.Left + pRectangle.Width - healthBarWidth;
            int healthBarTop = pRectangle.Top + pRectangle.Height - healthBarHeight;
            return new Rectangle(healthBarLeft, healthBarTop, healthBarWidth, healthBarHeight);
        }

        public void DrawHealthBar(SpriteBatch pSpriteBatch, int pHealth)
        {
            m_HealthBar.Draw(pSpriteBatch, pHealth);
        }

        public void Reposition(Rectangle pRectangle)
        {
            m_Avatar.Reposition(GetAvatarRect(pRectangle));
            m_HealthBar.Reposition(GetHealthBarRect(pRectangle));
            RepositionPowerBar(pRectangle);
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

        public void DrawHeldBullet(SpriteBatch pSpriteBatch, Tank.BulletType pBulletType)
        {
            //TODO: fix the positons of the circle and the bullet
            Vector2 pos = new Vector2(50, 50);
            DrawCircle(pSpriteBatch, Tankontroller.Instance().CM().Load<Texture2D>("circle"), 65, pos, Color.White);
            if (pBulletType == Tank.BulletType.BOUNCY_EMP)
            {
                pSpriteBatch.Draw(Tankontroller.Instance().CM().Load<Texture2D>("battery"), pos, null, Color.White, 0, new Vector2(0, 0), 0.25f, SpriteEffects.None, 0.0f);
            }
            else
            {
                pSpriteBatch.Draw(Tankontroller.Instance().CM().Load<Texture2D>("Empty"), pos, null, Color.White, 0, new Vector2(0, 0), 0.25f, SpriteEffects.None, 0.0f);
            }
        }
        public void DrawCircle(SpriteBatch pBatch, Texture2D pTexture, int pRadius, Vector2 pPos, Color pColour)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Width = pRadius;
            rectangle.Height = pRadius;
            rectangle.X = (int)pPos.X - pRadius / 2;
            rectangle.Y = (int)pPos.Y - pRadius / 2;
            pBatch.Draw(pTexture, rectangle, pColour);
        }

        public void Draw(SpriteBatch pSpriteBatch, int pHealth, Tank.BulletType pBulletType)
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
            DrawHeldBullet(pSpriteBatch, pBulletType);
        }
    }
}