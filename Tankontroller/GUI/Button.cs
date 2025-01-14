using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tankontroller.GUI
{
    public class Button
    {
        public Texture2D Texture { get; private set; } //Texture for the button

        public Texture2D TexturePressed { get; private set; } //Texture for the button when pressed
        public Color SelectedColour { get; private set; } //Colour of the button when selected
        public Rectangle Rect { get; private set; } //Rectangle for the button
        public bool Selected { get; set; } //If the button is selected
        public delegate void Action(); //Delegate for the button action
        private Action doButton; //Action for the button

        public Button(Texture2D pTexture, Texture2D pTexturePressed, Rectangle pRect, Color pColour, Action pDoButton) // Constructor
        {
            //Set the button properties
            Texture = pTexture;
            TexturePressed = pTexturePressed;
            Rect = pRect;
            SelectedColour = pColour;
            doButton = pDoButton;
        } // End of Constructor

        public void SelectButton(Vector2 pTouch) //Select the button
        {
            if (pTouch.X > Rect.Left && pTouch.X < Rect.Right &&
                pTouch.Y > Rect.Top && pTouch.Y < Rect.Bottom) //If the touch is within the button
            {
                Selected = true;
            }
            else
            {
                Selected = false;
            }
        }

        public bool PressButton() //Press the button without a touch
        {
            if (doButton != null) //If the button has an action
            {

                doButton(); //Do the action
                return true;

            }
            return false;
        }

        public bool PressButton(Vector2 pTouch) //Press the button with a touch
        {
            if (pTouch.X > Rect.Left && pTouch.X < Rect.Right &&
                pTouch.Y > Rect.Top && pTouch.Y < Rect.Bottom) //If the touch is within the button
            {
                if (doButton != null) //If the button has an action
                {

                    doButton(); //Do the action
                    return true;

                }
            }
            return false;
        }
    }

}
