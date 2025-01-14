using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tankontroller.Scenes
{
    //-------------------------------------------------------------------------------------------------
    // FlashScreenScene
    //
    // This class is used to display a flash screen. The flash screen displays a logo for a certain
    // number of seconds, and then transitions to the start scene.
    //
    // The class contains a logo texture, a sprite batch, a rectangle to draw the logo, and the number
    // of seconds left to display the flash screen. The class provides methods to update and draw the
    // flash screen.
    //-------------------------------------------------------------------------------------------------
    public class FlashScreenScene : IScene
    {
        IGame game = Tankontroller.Instance();
        Texture2D mLogoTexture = null; // The logo texture
        SpriteBatch mSpriteBatch = null; // The sprite batch
        Rectangle mRectangle; // The rectangle to draw the logo
        float mSecondsLeft; // The seconds left to display the flash screen
        public FlashScreenScene() // Constructor
        {
            mLogoTexture = game.CM().Load<Texture2D>("selogo"); // Load the logo texture
            mSpriteBatch = new SpriteBatch(game.GDM().GraphicsDevice); // Create the sprite batch
            int screenWidth = game.GDM().GraphicsDevice.Viewport.Width; // Get the screen width
            int screenHeight = game.GDM().GraphicsDevice.Viewport.Height; // Get the screen height
            int height = screenHeight / 2; // Set the height of the logo
            int width = (int)(mLogoTexture.Width * (float)height / mLogoTexture.Height); // Set the width of the logo
            int x = (screenWidth - width) / 2; // Set the x position of the logo
            int y = (screenHeight - height) / 2; // Set the y position of the logo
            mRectangle = new Rectangle(x, y, width, height); // Set the rectangle to draw the logo
            mSecondsLeft = DGS.Instance.GetInt("SECONDS_TO_DISPLAY_FLASH_SCREEN"); // Set the seconds left to display the flash screen
        }

        public void Update(float pSeconds) // Update the flash screen
        {
            mSecondsLeft -= pSeconds; // Decrease the seconds left
            if (mSecondsLeft <= 0.0f) // If the seconds left is less than or equal to 0
            {
                 // Get the game instance
                game.SM().Transition(new StartScene()); // Transition to the start scene
            }
        }
        public void Draw(float pSeconds) // Draw the flash screen
        {
            Tankontroller.Instance().GDM().GraphicsDevice.Clear(Color.Black); // Clear the screen
            mSpriteBatch.Begin(); // Begin the sprite batch

            mSpriteBatch.Draw(mLogoTexture, mRectangle, Color.White); // Draw the logo

            mSpriteBatch.End(); // End the sprite batch
        } // End of Draw
    }
}
