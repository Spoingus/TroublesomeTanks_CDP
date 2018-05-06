using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tankontroller.Scenes
{
    public class Button
    {
        public Texture2D Texture { get; private set; }

        public Texture2D TexturePressed { get; private set; }
        public Color SelectedColour { get; private set; }
        public Rectangle Rect { get; private set; }
        public bool Selected { get; set; }
        public delegate void Action();
        private Action doButton;

        public Button(Texture2D pTexture, Texture2D pTexturePressed, Rectangle pRect, Color pColour, Action pDoButton)
        {
            Texture = pTexture;
            TexturePressed = pTexturePressed;
            Rect = pRect;
            SelectedColour = pColour;
            doButton = pDoButton;
        }

        public void SelectButton(Vector2 pTouch)
        {
            if (pTouch.X > Rect.Left && pTouch.X < Rect.Right &&
                pTouch.Y > Rect.Top && pTouch.Y < Rect.Bottom)
            {
                Selected = true;
            }
            else
            {
                Selected = false;
            }
        }

        public bool PressButton()
        {
            if (doButton != null)
            {

                doButton();
                return true;

            }
            return false;
        }

        public bool PressButton(Vector2 pTouch)
        {
            if (pTouch.X > Rect.Left && pTouch.X < Rect.Right &&
                pTouch.Y > Rect.Top && pTouch.Y < Rect.Bottom)
            {
                if (doButton != null)
                {

                    doButton();
                    return true;

                }
            }
            return false;
        }
    }

}
