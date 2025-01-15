using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Tankontroller.Scenes
{
    //-------------------------------------------------------------------------------------------------
    // This class is used to display the game over screen after the gameScene ends. It repositions the
    // GUIs of each player. The winner in the centre and the losers below them.
    //-------------------------------------------------------------------------------------------------
    public class GameOverScene : IScene
    {
        Tankontroller tankControllerInstance = (Tankontroller)Tankontroller.Instance();
        private List<Player> mPlayers;
        Texture2D mBackgroundTexture = null;
        SpriteBatch mSpriteBatch = null;
        Rectangle mRectangle;
        float mSecondsLeft;
        int mWinner;
        public GameOverScene(Texture2D pBackgroundTexture, List<Player> pPlayers, int pWinner)
        {
            mBackgroundTexture = pBackgroundTexture;
            mSpriteBatch = new SpriteBatch(tankControllerInstance.GDM().GraphicsDevice);
            int screenWidth = tankControllerInstance.GDM().GraphicsDevice.Viewport.Width;
            int screenHeight = tankControllerInstance.GDM().GraphicsDevice.Viewport.Height;
            int height = screenHeight / 2;
            int width = (int)(mBackgroundTexture.Width * (float)height / mBackgroundTexture.Height);
            int x = (screenWidth - width) / 2;
            int y = (screenHeight - height) / 2;
            mRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
            mSecondsLeft = DGS.Instance.GetFloat("SECONDS_TO_DISPLAY_GAMEOVER_SCREEN");
            tankControllerInstance.ReplaceCurrentMusicInstance("Music/Music_start", true);
            mPlayers = pPlayers;
            mWinner = pWinner;
            RepositionGUIs();
        }
        //Repositions the GUIs of each player
        public void RepositionGUIs()
        {
            int loserCount = mPlayers.Count;
            if (mWinner != -1)
            {
                loserCount -= 1;
            }
            int textureWidth = tankControllerInstance.GDM().GraphicsDevice.Viewport.Width / 2;
            int textureHeight = (int)(textureWidth * ((float)254 / (float)540));
            int centreXOffset = 0;
            for (int i = 0; i < mPlayers.Count; i++)
            {
                if (i != mWinner) //Repositon the losers
                {
                    int centreY = tankControllerInstance.GDM().GraphicsDevice.Viewport.Height / 2 + textureHeight / 2;
                    int loserTextureWidth = textureWidth / mPlayers.Count;
                    int loserTextureHeight = (int)(loserTextureWidth * ((float)254 / (float)540));
                    Rectangle newRectangle = new Rectangle(centreXOffset, centreY, loserTextureWidth, loserTextureHeight);
                    mPlayers[i].GUI.Reposition(newRectangle);
                    centreXOffset += loserTextureWidth;
                }
                else //Reposition the winner
                {
                    int centreX = tankControllerInstance.GDM().GraphicsDevice.Viewport.Width / 2 - textureWidth / 2;
                    int centreY = textureHeight / 2;
                    Rectangle newRectangle = new Rectangle(centreX, centreY, textureWidth, textureHeight);
                    mPlayers[mWinner].GUI.Reposition(newRectangle);
                }
            }
        }
        public void Update(float pSeconds)
        {
            mSecondsLeft -= pSeconds;
            if (mSecondsLeft <= 0.0f)
            {
                IGame game = Tankontroller.Instance();
                game.SM().Transition(null);
            }
        }
        public void Draw(float pSeconds)
        {
            Tankontroller.Instance().GDM().GraphicsDevice.Clear(Color.Black);
            mSpriteBatch.Begin();

            mSpriteBatch.Draw(mBackgroundTexture, mRectangle, Color.White);
            for (int i = 0; i < mPlayers.Count; i++)
            {
                mPlayers[i].GUI.DrawAvatar(mSpriteBatch);
                mPlayers[i].GUI.DrawHealthBar(mSpriteBatch);
            }
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
