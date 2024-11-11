using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;


namespace Tankontroller.Scenes
{
    //The main menu scene that is displayed once the app has launched
    public class StartScene : IScene
    {
        ButtonList mButtonList = null;
        Texture2D mBackgroundTexture = null;
        Rectangle mBackgroundRectangle;

        Texture2D mForgroundTexture = null;

        Texture2D mTitleTexture = null;
        Rectangle mTitleRectangle;

        SpriteBatch mSpriteBatch = null;
        
        float mSecondsLeft;
        public StartScene() {
            Tankontroller game = (Tankontroller)Tankontroller.Instance();
            
            mSpriteBatch = new SpriteBatch(game.GDM().GraphicsDevice);
            int screenWidth = game.GDM().GraphicsDevice.Viewport.Width;
            int screenHeight = game.GDM().GraphicsDevice.Viewport.Height;

            mBackgroundTexture = game.CM().Load<Texture2D>("background_01");         
            mBackgroundRectangle = new Rectangle(0, 0, screenWidth, screenHeight);


            mForgroundTexture = game.CM().Load<Texture2D>("menu_white");

            mTitleTexture = game.CM().Load<Texture2D>("menu_title");
            mTitleRectangle = new Rectangle((screenWidth / 2) - (644 / 2), (screenHeight / 2) - (128 / 2), 644, 128);

            mButtonList = new ButtonList();
                        
            Texture2D startGameButtonTexture = game.CM().Load<Texture2D>("menu_play_white");
            Texture2D startGameButtonTexturePressed = game.CM().Load<Texture2D>("menu_play_dark");

            //Makes the start game button
            Rectangle startGameButtonRectangle = 
                new Rectangle(
                    ((int)((screenWidth - startGameButtonTexture.Width) / 2) - (int)(startGameButtonTexture.Width * 0.75f)), 
                    (screenHeight) / 2 + startGameButtonTexture.Height, 
                    startGameButtonTexture.Width, 
                    startGameButtonTexture.Height);
            
            Button startGameButton = new Button(startGameButtonTexture,startGameButtonTexturePressed, startGameButtonRectangle, Color.Red, StartGame);
            startGameButton.Selected = true;
            mButtonList.Add(startGameButton);

            //Makes the exit game button
            Texture2D exitGameButtonTexture = game.CM().Load<Texture2D>("menu_quit_white");
            Texture2D exitGameButtonTexturePressed = game.CM().Load<Texture2D>("menu_quit_dark");
            
            Rectangle exitGameButtonRectangle =
                new Rectangle((screenWidth - exitGameButtonTexture.Width) / 2 + (int)(startGameButtonTexture.Width * 0.75f),
                    (screenHeight) / 2 + exitGameButtonTexture.Width,
                    exitGameButtonTexture.Width, 
                    exitGameButtonTexture.Height);
            Button exitGameButton = new Button(exitGameButtonTexture, exitGameButtonTexturePressed, exitGameButtonRectangle, Color.Red, ExitGame);
            exitGameButton.Selected = false;
            mButtonList.Add(exitGameButton);
            mSecondsLeft = 0.1f;
            game.ReplaceCurrentMusicInstance("Music/Music_start", true);
        }

        //Exits the game
        private void ExitGame()
        {
            IGame game = Tankontroller.Instance();
            game.SM().Transition(null);
        }

        //Starts the game
        private void StartGame()
        {
            IGame game = Tankontroller.Instance();
            game.SM().Transition(new GameScene(), false);
        }

        Task? detectControllerTask = null;
        public void Update(float pSeconds)
        {
            Escape();

            IGame game = Tankontroller.Instance();
            if (DGS.Instance.GetBool("HAVE_CONTROLLER") && (detectControllerTask == null || detectControllerTask.IsCompleted))
            {
                detectControllerTask = Task.Run(async () => await game.DetectControllers());
            }

            foreach (IController controller in game.GetControllers())
            {
                controller.UpdateController();
                mSecondsLeft -= pSeconds;

                if (controller.IsPressedWithCharge(Control.LEFT_TRACK_FORWARDS) || controller.IsPressed(Control.TURRET_LEFT))
                {
                    if (mSecondsLeft <= 0.0f)
                    {
                        //mSoundEffects["Button_Push"].Play();
                        mButtonList.SelectPreviousButton();
                        mSecondsLeft = 1.0f;
                    }
                }
                if (controller.IsPressed(Control.LEFT_TRACK_BACKWARDS) || controller.IsPressed(Control.TURRET_RIGHT))
                {
                    if (mSecondsLeft <= 0.0f)
                    {
                        //mSoundEffects["Button_Push"].Play();
                        mButtonList.SelectNextButton();
                        mSecondsLeft = 1.0f;
                    }
                }


                if (controller.IsPressed(Control.FIRE))
                {
                    if (mSecondsLeft <= 0.0f)
                    {
                        mButtonList.PressSelectedButton();
                        mSecondsLeft = 0.1f;
                    }
                }
                if (controller.IsPressed(Control.RECHARGE))
                {
                    if (mSecondsLeft <= 0.0f)
                    {
                        mButtonList.PressSelectedButton();
                        mSecondsLeft = 0.1f;
                    }
                }

            }
        }
        public void Escape()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Tankontroller.Instance().SM().Transition(null);
            }
        }

        //Draws the start screen and buttons
        public void Draw(float pSeconds)
        {
            Tankontroller.Instance().GDM().GraphicsDevice.Clear(Color.Black);
            mSpriteBatch.Begin();
            Color backColour = Color.White;
            
            mSpriteBatch.Draw(mBackgroundTexture, mBackgroundRectangle, backColour);
            mSpriteBatch.Draw(mForgroundTexture, mBackgroundRectangle, backColour);

            mSpriteBatch.Draw(mTitleTexture, mTitleRectangle, backColour);

            mButtonList.Draw(mSpriteBatch);

            mSpriteBatch.End();
        }
    }
}
