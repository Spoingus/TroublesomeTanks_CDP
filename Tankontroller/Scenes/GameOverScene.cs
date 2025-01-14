using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Tankontroller.Scenes
{
    //-------------------------------------------------------------------------------------------------
    // GameOverScene
    //
    // This class is used to display the game over screen. The game over screen displays the avatars and
    // health bars of the players, with the winner in the centre of the screen and the losers in a row
    // at the bottom of the screen.
    //
    // The class contains a background texture, a sprite batch, a rectangle to draw the background, the
    // number of seconds left to display the game over screen, the list of players, and the index of the
    // winner. The class provides methods to reposition the GUIs of the players, update the game over
    // screen, and draw the game over screen.
    //-------------------------------------------------------------------------------------------------
    public class GameOverScene : IScene
    {
        private List<Player> mPlayers;
        Texture2D mBackgroundTexture = null;
        SpriteBatch mSpriteBatch = null;
        Rectangle mRectangle;
        float mSecondsLeft;
        int mWinner;
        public GameOverScene(Texture2D pBackgroundTexture, List<Player> pPlayers, int pWinner)
        {
            Tankontroller game = (Tankontroller)Tankontroller.Instance();
            mBackgroundTexture = pBackgroundTexture;
            mSpriteBatch = new SpriteBatch(game.GDM().GraphicsDevice);
            int screenWidth = game.GDM().GraphicsDevice.Viewport.Width;
            int screenHeight = game.GDM().GraphicsDevice.Viewport.Height;
            int height = screenHeight / 2;
            int width = (int)(mBackgroundTexture.Width * (float)height / mBackgroundTexture.Height);
            int x = (screenWidth - width) / 2;
            int y = (screenHeight - height) / 2;
            mRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
            mSecondsLeft = DGS.Instance.GetFloat("SECONDS_TO_DISPLAY_GAMEOVER_SCREEN");
            game.ReplaceCurrentMusicInstance("Music/Music_start", true);
            mPlayers = pPlayers;
            mWinner = pWinner;
            RepositionGUIs();
        }
        public void RepositionGUIs()
        {
            if (mWinner != -1)
            {
                RepositionAsWinner();
            }
            RepositionAsLosers();
        }
        //Repositions the winner GUI into the centre of the screen
        public void RepositionAsWinner()
        {
            Tankontroller game = (Tankontroller)Tankontroller.Instance();
            float textureHeightOverWidth = (float)254 / (float)540;
            int textureWidth = game.GDM().GraphicsDevice.Viewport.Width / 2;
            int textureHeight = (int)(textureWidth * textureHeightOverWidth);
            int centreX = game.GDM().GraphicsDevice.Viewport.Width / 2 - textureWidth / 2;
            int centreY = textureHeight / 2;// game.GDM().GraphicsDevice.Viewport.Height / 2 - textureHeight / 2;
            Rectangle newRectangle = new Rectangle(centreX, centreY, textureWidth, textureHeight);
            mPlayers[mWinner].GUI.Reposition(newRectangle);
        }
        //Repositions the loser GUIs into a row at the bottom of the screen
        public void RepositionAsLosers()
        {
            int offset = 0;
            int loserCount = mPlayers.Count;
            if (mWinner != -1)
            {
                loserCount -= 1;
            }
            for (int i = 0; i < mPlayers.Count; i++)
            {
                if (i != mWinner)
                {
                    Tankontroller game = (Tankontroller)Tankontroller.Instance();
                    float textureHeightOverWidth = (float)254 / (float)540;
                    int textureWidth = game.GDM().GraphicsDevice.Viewport.Width / 2;
                    int textureHeight = (int)(textureWidth * textureHeightOverWidth);
                    int centreX = offset;
                    int centreY = game.GDM().GraphicsDevice.Viewport.Height / 2 + textureHeight / 2;
                    textureWidth = textureWidth / mPlayers.Count;
                     textureHeight = (int)(textureWidth * textureHeightOverWidth);
                    Rectangle newRectangle = new Rectangle(centreX, centreY, textureWidth, textureHeight);
                    mPlayers[i].GUI.Reposition(newRectangle);
                    offset += textureWidth;
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
