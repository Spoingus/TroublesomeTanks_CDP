using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tankontroller.Scenes
{
    class ButtonList
    {
        List<Button> mButtons = null; //List of buttons
        int currentSelectedButtonIndex = 0; //Index of the currently selected button

        public ButtonList() //Constructor
        {
            mButtons = new List<Button>(); //Initialise the list
        }
        public void Add(Button pButton) //Add a button to the list
        {
            mButtons.Add(pButton);
        }

        public void SelectNextButton() //Select the next button
        {
            Console.WriteLine("NextButton start: " + currentSelectedButtonIndex); //Debug
            int nextSelectedButtonIndex = currentSelectedButtonIndex + 1; //Get the index of the next button
            if (nextSelectedButtonIndex >= mButtons.Count) //If the index is out of range
            {
                nextSelectedButtonIndex = 0; //Set the index to the first button
            }
            mButtons[nextSelectedButtonIndex].Selected = true; //Select the next button
            mButtons[currentSelectedButtonIndex].Selected = false; //Deselect the current button
            currentSelectedButtonIndex = nextSelectedButtonIndex; //Set the current button index to the next button
            Console.WriteLine("NextButton finish: " + currentSelectedButtonIndex); //Debug

        } // End of SelectNextButton
        public void SelectPreviousButton() //Select previous button
        {
            Console.WriteLine("PreviousButton start: " + currentSelectedButtonIndex); //Debug
            int previousSelectedButtonIndex = currentSelectedButtonIndex - 1; //Get the index of the previous button
            if (previousSelectedButtonIndex < 0) //If the index is out of range
            {
                previousSelectedButtonIndex = mButtons.Count - 1; //Set the index to the last button
            }
            mButtons[previousSelectedButtonIndex].Selected = true; //Select the previous button
            mButtons[currentSelectedButtonIndex].Selected = false; //Deselect the current button
            currentSelectedButtonIndex = previousSelectedButtonIndex; //Set the current button index to the previous button
            Console.WriteLine("PreviousButton finish: " + currentSelectedButtonIndex); //Debug
        }

        public void PressSelectedButton() //Press the selected button
        {
            mButtons[currentSelectedButtonIndex].PressButton();
        }

        public void Draw(SpriteBatch pSpriteBatch) //Draw the buttons
        {
            foreach (Button button in mButtons) //For each button
            {
                Color buttonColour = Color.White; //Set the button colour
                if (button.Selected) //If the button is selected
                {
                    buttonColour = button.SelectedColour; //Set the button colour to the selected colour
                }
                pSpriteBatch.Draw(button.Texture, button.Rect, buttonColour); //Draw the button
            }
        }
    }
}
