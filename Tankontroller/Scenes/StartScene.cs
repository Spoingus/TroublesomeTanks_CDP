using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tankontroller.Controller;
using Tankontroller.GUI;


namespace Tankontroller.Scenes
{
    //--------------------------------------------------------------------------------------------------
    // StartScene
    //
    // This class is used to display the start screen. The start screen displays the game title and two
    // buttons: one to start the game and one to exit the game. The class contains a background texture,
    // a sprite batch, a rectangle to draw the background, a list of buttons, and the number of seconds
    // left to display the start screen.
    // The class provides methods to update and draw the start screen.
    //--------------------------------------------------------------------------------------------------
    public class StartScene : IScene
    {
        IGame gameInstance = Tankontroller.Instance();
        Tankontroller tControllerInstance = (Tankontroller)Tankontroller.Instance();
        ButtonList mButtonList = null;
        Texture2D mBackgroundTexture = null;
        Rectangle mBackgroundRectangle;

        Texture2D mForgroundTexture = null;

        Texture2D mTitleTexture = null;
        Rectangle mTitleRectangle;

        SpriteBatch mSpriteBatch = null;
<<<<<<< HEAD
        
        float mSecondsLeft;
        public StartScene() {
            mSpriteBatch = new SpriteBatch(tControllerInstance.GDM().GraphicsDevice);
            int screenWidth = tControllerInstance.GDM().GraphicsDevice.Viewport.Width;
            int screenHeight = tControllerInstance.GDM().GraphicsDevice.Viewport.Height;

            mBackgroundTexture = tControllerInstance.CM().Load<Texture2D>("background_01");         
=======

        float mSecondsLeft;
        public StartScene()
        {
            Tankontroller game = (Tankontroller)Tankontroller.Instance();

            mSpriteBatch = new SpriteBatch(game.GDM().GraphicsDevice);
            
            int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            mBackgroundTexture = game.CM().Load<Texture2D>("background_01");
>>>>>>> origin/master
            mBackgroundRectangle = new Rectangle(0, 0, screenWidth, screenHeight);

            mForgroundTexture = tControllerInstance.CM().Load<Texture2D>("menu_white");

            mTitleTexture = tControllerInstance.CM().Load<Texture2D>("menu_title");
            mTitleRectangle = new Rectangle((screenWidth / 2) - (644 / 2), (screenHeight / 2) - (128 / 2), 644, 128);

            mButtonList = new ButtonList();
<<<<<<< HEAD

            //Start Game Button
            Texture2D startGameButtonTexture = tControllerInstance.CM().Load<Texture2D>("menu_play_white");
            Texture2D startGameButtonTexturePressed = tControllerInstance.CM().Load<Texture2D>("menu_play_dark");

            Rectangle startGameButtonRectangle = 
=======

            Texture2D startGameButtonTexture = game.CM().Load<Texture2D>("menu_play_white");
            Texture2D startGameButtonTexturePressed = game.CM().Load<Texture2D>("menu_play_dark");

            //Makes the start game button
            Rectangle startGameButtonRectangle =
>>>>>>> origin/master
                new Rectangle(
                    ((int)((screenWidth - startGameButtonTexture.Width) / 2) - (int)(startGameButtonTexture.Width * 0.75f)),
                    (screenHeight) / 2 + startGameButtonTexture.Height,
                    startGameButtonTexture.Width,
                    startGameButtonTexture.Height);

            Button startGameButton = new Button(startGameButtonTexture, startGameButtonTexturePressed, startGameButtonRectangle, Color.Red, StartGame);
            startGameButton.Selected = true;
            mButtonList.Add(startGameButton);

<<<<<<< HEAD
            //Exit game button
            Texture2D exitGameButtonTexture = tControllerInstance.CM().Load<Texture2D>("menu_quit_white");
            Texture2D exitGameButtonTexturePressed = tControllerInstance.CM().Load<Texture2D>("menu_quit_dark");
            
=======
            //Makes the exit game button
            Texture2D exitGameButtonTexture = game.CM().Load<Texture2D>("menu_quit_white");
            Texture2D exitGameButtonTexturePressed = game.CM().Load<Texture2D>("menu_quit_dark");

>>>>>>> origin/master
            Rectangle exitGameButtonRectangle =
                new Rectangle((screenWidth - exitGameButtonTexture.Width) / 2 + (int)(startGameButtonTexture.Width * 0.75f),
                    (screenHeight) / 2 + exitGameButtonTexture.Width,
                    exitGameButtonTexture.Width,
                    exitGameButtonTexture.Height);
            Button exitGameButton = new Button(exitGameButtonTexture, exitGameButtonTexturePressed, exitGameButtonRectangle, Color.Red, ExitGame);
            exitGameButton.Selected = false;
            mButtonList.Add(exitGameButton);
            mSecondsLeft = 0.1f;
            tControllerInstance.ReplaceCurrentMusicInstance("Music/Music_start", true);
        }

        //Exits the game
        private void ExitGame()
        {
            gameInstance.SM().Transition(null);
        }

        //Starts the game
        private void StartGame()
        {
            gameInstance.SM().Transition(new PlayerSelectionScene(), false);
        }

        public void Update(float pSeconds)
        {
            Escape();

<<<<<<< HEAD
            if (DGS.Instance.GetBool("HAVE_CONTROLLER") && (detectControllerTask == null || detectControllerTask.IsCompleted))
            {
                detectControllerTask = Task.Run(async () => await gameInstance.DetectControllers());
            }
=======
            IGame game = Tankontroller.Instance();
            game.DetectControllers();
>>>>>>> origin/master

            foreach (IController controller in gameInstance.GetControllers())
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
