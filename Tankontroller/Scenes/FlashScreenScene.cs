using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tankontroller.Scenes
{
    //-------------------------------------------------------------------------------------------------
    // This is the opening scene that is displayed. It has the Spooky Elephant logo. Renders a texture
    // with a time limit.
    //-------------------------------------------------------------------------------------------------
    public class FlashScreenScene : IScene
    {
        Texture2D mLogoTexture = null;
        SpriteBatch mSpriteBatch = null;
        Rectangle mRectangle;
        float mSecondsLeft;
        public FlashScreenScene()
        {
            IGame game = Tankontroller.Instance();
            mLogoTexture = game.CM().Load<Texture2D>("selogo");
            mSpriteBatch = new SpriteBatch(game.GDM().GraphicsDevice);
            int screenWidth = game.GDM().GraphicsDevice.Viewport.Width;
            int screenHeight = game.GDM().GraphicsDevice.Viewport.Height;
            int height = screenHeight / 2;
            int width = (int)(mLogoTexture.Width * (float)height / mLogoTexture.Height);
            int x = (screenWidth - width) / 2;
            int y = (screenHeight - height) / 2;
            mRectangle = new Rectangle(x, y, width, height);
            mSecondsLeft = DGS.Instance.GetFloat("SECONDS_TO_DISPLAY_FLASH_SCREEN");
        }

        public void Update(float pSeconds)
        {
            mSecondsLeft -= pSeconds;
            if (mSecondsLeft <= 0.0f)
            {
                IGame game = Tankontroller.Instance();
                game.SM().Transition(new StartScene());
            }
        }

        public void Draw(float pSeconds)
        {
            Tankontroller.Instance().GDM().GraphicsDevice.Clear(Color.Black);
            mSpriteBatch.Begin();

            mSpriteBatch.Draw(mLogoTexture, mRectangle, Color.White);

            mSpriteBatch.End();
        }
        
        public void Escape()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Tankontroller.Instance().SM().Transition(null);
            }

        }
    }
}