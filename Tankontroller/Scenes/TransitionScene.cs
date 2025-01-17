using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tankontroller.Scenes
{
    //-------------------------------------------------------------------------------------------------
    // This class is used to transition one scene into another scene. It is a drop down
    // transition. An example of how it functions is the main menu where the main menu texture over the
    // intro texture and slowly replaces it, frame by frame, with a larger part of the main menu texture
    // Once it has been fully replaced, the scene changes to the main menu via the scene manager.
    //-------------------------------------------------------------------------------------------------
    public class TransitionScene : IScene
    {
        IGame gameInstance = Tankontroller.Instance();
        RenderTarget2D mPreviousTexture = null;
        RenderTarget2D mNextTexture = null;
        Rectangle mRectangle;
        IScene mPreviousScene;
        IScene mNextScene;
        Vector2 mNextPosition = new Vector2(0, -DGS.Instance.GetInt("SCREENHEIGHT"));
        Vector2 mVelocity = new Vector2(0, 0);
        Vector2 mAcceleration = new Vector2(0, 1);

        public TransitionScene(IScene pPreviousScene, IScene pNextScene)
        {
            mPreviousScene = pPreviousScene;
            mNextScene = pNextScene;
            spriteBatch = new SpriteBatch(gameInstance.GDM().GraphicsDevice);
            mRectangle = new Rectangle(0, 0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            GeneratePreviousTexture();
            GenerateNextTexture();
        }

        public void GeneratePreviousTexture()
        {
            GraphicsDevice graphicsDevice = gameInstance.GDM().GraphicsDevice;
            mPreviousTexture = new RenderTarget2D(graphicsDevice, graphicsDevice.PresentationParameters.BackBufferWidth, graphicsDevice.PresentationParameters.BackBufferHeight, false, graphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
            graphicsDevice.SetRenderTarget(mPreviousTexture);
            graphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp,
                DepthStencilState.None, RasterizerState.CullCounterClockwise);
            mPreviousScene.Draw(0);
            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);
        }

        public void GenerateNextTexture()
        {
            GraphicsDevice graphicsDevice = gameInstance.GDM().GraphicsDevice;
            mNextTexture = new RenderTarget2D(graphicsDevice, graphicsDevice.PresentationParameters.BackBufferWidth, graphicsDevice.PresentationParameters.BackBufferHeight, false, graphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
            graphicsDevice.SetRenderTarget(mNextTexture);
            graphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp,
                DepthStencilState.None, RasterizerState.CullCounterClockwise);
            mNextScene.Draw(0);
            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);
        }

        public override void Update(float pSeconds)
        {
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
        public override void Draw(float pSeconds)
        {
            Tankontroller.Instance().GDM().GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            spriteBatch.Draw(mPreviousTexture, mRectangle, Color.White);
            spriteBatch.Draw(mNextTexture, mNextPosition, mRectangle, Color.White);
            spriteBatch.End();
        }
    }
}
