using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tankontroller.Scenes
{
    //-------------------------------------------------------------------------------------------------
    // TransitionScene
    //
    // This class is used to display a transition screen. The transition screen displays the previous
    // scene and the next scene. The class contains the previous texture, the next texture, a sprite
    // batch, a rectangle to draw the textures, the number of seconds left to display the transition
    // screen, the previous scene, the next scene, the previous position, the next position, the velocity,
    // and the acceleration.
    //
    // The class provides methods to generate the previous texture, generate the next texture,
    // update the transition screen, and draw the transition screen.
    //-------------------------------------------------------------------------------------------------
    public class TransitionScene : IScene
    {
        IGame gameInstance = Tankontroller.Instance();
        IGame tankControllerInstance = Tankontroller.Instance();
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
            
            mPreviousScene = pPreviousScene;
            mNextScene = pNextScene;
            mSpriteBatch = new SpriteBatch(gameInstance.GDM().GraphicsDevice);
            
            mRectangle = new Rectangle(0, 0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            mSecondsLeft = DGS.Instance.GetInt("SECONDS_TO_DISPLAY_FLASH_SCREEN");
            GeneratePreviousTexture();
            GenerateNextTexture();
            mPreviousPosition = new Vector2(0, 0);
            mVelocity = new Vector2(0, 0);
            mAcceleration = new Vector2(0, 1);
            mNextPosition = new Vector2(0, -DGS.Instance.GetInt("SCREENHEIGHT"));
        }

        public void GeneratePreviousTexture()
        {
           
            GraphicsDevice graphicsDevice = tankControllerInstance.GDM().GraphicsDevice;
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
            GraphicsDevice graphicsDevice = gameInstance.GDM().GraphicsDevice;
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
                IGame gameInstance = Tankontroller.Instance();
                gameInstance.SM().Pop();

                if (mNextScene != gameInstance.SM().Top)
                {
                    gameInstance.SM().Push(mNextScene);
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
        public void Escape()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Tankontroller.Instance().SM().Transition(null);
            }
        }
    }
}
