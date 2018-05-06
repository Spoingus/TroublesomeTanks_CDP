using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tankontroller.Scenes
{
    public class TransitionScene : IScene
    {
        RenderTarget2D mPreviousTexture = null;
        RenderTarget2D mNextTexture = null;
        SpriteBatch mSpriteBatch = null;
        Rectangle mRectangle;
        float mSecondsLeft;
        IScene mPreviousScene;
        IScene mNextScene;
        Vector2 mPreviousPosition;
        Vector2 mNextPosition;
        Vector2 mVelocity;
        Vector2 mAcceleration;
        public TransitionScene(IScene pPreviousScene, IScene pNextScene)
        {
            IGame game = Tankontroller.Instance();
            mPreviousScene = pPreviousScene;
            mNextScene = pNextScene;
            mSpriteBatch = new SpriteBatch(game.GDM().GraphicsDevice);
            
            mRectangle = new Rectangle(0, 0, DGS.SCREENWIDTH, DGS.SCREENHEIGHT);
            mSecondsLeft = DGS.SECONDS_TO_DISPLAY_FLASH_SCREEN;
            GeneratePreviousTexture();
            GenerateNextTexture();
            mPreviousPosition = new Vector2(0, 0);
            mVelocity = new Vector2(0, 0);
            mAcceleration = new Vector2(0, 1);
            mNextPosition = new Vector2(0, -DGS.SCREENHEIGHT);
        }

        public void GeneratePreviousTexture()
        {
            IGame game = Tankontroller.Instance();
            GraphicsDevice graphicsDevice = game.GDM().GraphicsDevice;
            mPreviousTexture = new RenderTarget2D(graphicsDevice, graphicsDevice.PresentationParameters.BackBufferWidth, graphicsDevice.PresentationParameters.BackBufferHeight, false, graphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
            // Set the render target
            graphicsDevice.SetRenderTarget(mPreviousTexture);

            graphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

            // Draw the scene
            graphicsDevice.Clear(Color.CornflowerBlue);

            mSpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp,
                DepthStencilState.None, RasterizerState.CullCounterClockwise);
            mPreviousScene.Draw(0);
            mSpriteBatch.End();

            // Drop the render target
            graphicsDevice.SetRenderTarget(null);
        }

        public void GenerateNextTexture()
        {
            IGame game = Tankontroller.Instance();
            GraphicsDevice graphicsDevice = game.GDM().GraphicsDevice;
            mNextTexture = new RenderTarget2D(graphicsDevice, graphicsDevice.PresentationParameters.BackBufferWidth, graphicsDevice.PresentationParameters.BackBufferHeight, false, graphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
            // Set the render target
            graphicsDevice.SetRenderTarget(mNextTexture);

            graphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

            // Draw the scene
            graphicsDevice.Clear(Color.CornflowerBlue);

            mSpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp,
                DepthStencilState.None, RasterizerState.CullCounterClockwise);
            mNextScene.Draw(0);
            mSpriteBatch.End();

            // Drop the render target
            graphicsDevice.SetRenderTarget(null);
        }

        public void Update(float pSeconds)
        {
            mSecondsLeft -= pSeconds;
            mVelocity += mAcceleration;
            mNextPosition += mVelocity;
            if (mNextPosition.Y > 0)
            {
                IGame game = Tankontroller.Instance();
                game.SM().Pop();

                if (mNextScene != game.SM().Top)
                {
                    game.SM().Push(mNextScene);
                }                

            }
        }
        public void Draw(float pSeconds)
        {
            Tankontroller.Instance().GDM().GraphicsDevice.Clear(Color.Black);
            mSpriteBatch.Begin();

            mSpriteBatch.Draw(mPreviousTexture, mRectangle, Color.White);
            mSpriteBatch.Draw(mNextTexture, mNextPosition, mRectangle, Color.White);
            mSpriteBatch.End();
        }
    }
}
