using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tankontroller.Controller;
using Tankontroller.GUI;


namespace Tankontroller.Scenes
{
    //--------------------------------------------------------------------------------------------------
    // This is the main menu scene with the games title, play button and the exit button. This class
    // is responsible for creating the buttons and dealing with any controller inputs detected on the
    // main menu.
    //--------------------------------------------------------------------------------------------------
    public class StartScene : IScene
    {
        IGame gameInstance = Tankontroller.Instance();
        Tankontroller tankControllerInstance = (Tankontroller)Tankontroller.Instance();
        ButtonList mButtonList = null;
        Texture2D mBackgroundTexture = null;
        Rectangle mBackgroundRectangle;
        Texture2D mForgroundTexture = null;
        Texture2D mTitleTexture = null;
        Rectangle mTitleRectangle;
        private float secondsLeft;
        public StartScene()
        {
            spriteBatch = new SpriteBatch(tankControllerInstance.GDM().GraphicsDevice);
            int screenWidth = tankControllerInstance.GDM().GraphicsDevice.Viewport.Width;
            int screenHeight = tankControllerInstance.GDM().GraphicsDevice.Viewport.Height;

            mBackgroundTexture = tankControllerInstance.CM().Load<Texture2D>("background_01");
            mBackgroundRectangle = new Rectangle(0, 0, screenWidth, screenHeight);

            mForgroundTexture = tankControllerInstance.CM().Load<Texture2D>("menu_white");
            mTitleTexture = tankControllerInstance.CM().Load<Texture2D>("menu_title");
            mTitleRectangle = new Rectangle((screenWidth / 2) - (644 / 2), (screenHeight / 2) - (128 / 2), 644, 128);

            mButtonList = new ButtonList();

            //Start Game Button
            Texture2D startGameButtonTexture = tankControllerInstance.CM().Load<Texture2D>("menu_play_white");
            Texture2D startGameButtonTexturePressed = tankControllerInstance.CM().Load<Texture2D>("menu_play_dark");

            Rectangle startGameButtonRectangle =
                new Rectangle(
                    ((int)((screenWidth - startGameButtonTexture.Width) / 2) - (int)(startGameButtonTexture.Width * 0.75f)),
                    (screenHeight) / 2 + startGameButtonTexture.Height,
                    startGameButtonTexture.Width,
                    startGameButtonTexture.Height);

            Button startGameButton = new Button(startGameButtonTexture, startGameButtonTexturePressed, startGameButtonRectangle, Color.Red, SelectLevel);
            startGameButton.Selected = true;
            mButtonList.Add(startGameButton);


            //Makes the exit game button
            Texture2D exitGameButtonTexture = tankControllerInstance.CM().Load<Texture2D>("menu_quit_white");
            Texture2D exitGameButtonTexturePressed = tankControllerInstance.CM().Load<Texture2D>("menu_quit_dark");

            Rectangle exitGameButtonRectangle =
                new Rectangle((screenWidth - exitGameButtonTexture.Width) / 2 + (int)(startGameButtonTexture.Width * 0.75f),
                    (screenHeight) / 2 + exitGameButtonTexture.Width,
                    exitGameButtonTexture.Width,
                    exitGameButtonTexture.Height);
            Button exitGameButton = new Button(exitGameButtonTexture, exitGameButtonTexturePressed, exitGameButtonRectangle, Color.Red, ExitGame);
            exitGameButton.Selected = false;
            mButtonList.Add(exitGameButton);
            secondsLeft = 0.1f;
            tankControllerInstance.ReplaceCurrentMusicInstance("Music/Music_start", true);

            // Level Selection Button
            /*Texture2D levelSelectionButtonTexture = tankControllerInstance.CM().Load<Texture2D>("menu_play_white");
            Texture2D levelSelectionButtonTexturePressed = tankControllerInstance.CM().Load<Texture2D>("menu_quit_dark");

            Rectangle levelSelectionButtonRectangle =
                new Rectangle(
                    ((int)((screenWidth - levelSelectionButtonTexture.Width) / 2)),
                    (screenHeight) / 2 + startGameButtonTexture.Height * 2,
                    levelSelectionButtonTexture.Width,
                    levelSelectionButtonTexture.Height);

            Button levelSelectionButton = new Button(levelSelectionButtonTexture, levelSelectionButtonTexturePressed, levelSelectionButtonRectangle, Color.Red, SelectLevel);
            levelSelectionButton.Selected = false;
            mButtonList.Add(levelSelectionButton);*/

        }
        
        private void SelectLevel()
        {
            gameInstance.SM().Transition(new LevelSelectionScene(), false);
        }

        //Exits the game
        private void ExitGame()
        {
            gameInstance.SM().Transition(null);
        }
        //Starts the game
        private void StartGame()
        {
            //load level select scene

            string defaultMapFile = "Maps/1-3_player_map.json"; // Specify the default map file to use
            gameInstance.SM().Transition(new PlayerSelectionScene(defaultMapFile), false);
        }
        
        public override void Update(float pSeconds)
        {
            Escape();
            gameInstance.DetectControllers();

            foreach (IController controller in gameInstance.GetControllers())
            {
                controller.UpdateController();
                secondsLeft -= pSeconds;

                if (controller.IsPressedWithCharge(Control.LEFT_TRACK_FORWARDS) || controller.IsPressed(Control.TURRET_LEFT))
                {
                    if (secondsLeft <= 0.0f)
                    {
                        mButtonList.SelectPreviousButton();
                        secondsLeft = 0.5f;
                    }
                }
                if (controller.IsPressed(Control.LEFT_TRACK_BACKWARDS) || controller.IsPressed(Control.TURRET_RIGHT))
                {
                    if (secondsLeft <= 0.0f)
                    {
                        mButtonList.SelectNextButton();
                        secondsLeft = 0.5f;
                    }
                }

                if (controller.IsPressed(Control.FIRE))
                {
                    if (secondsLeft <= 0.0f)
                    {
                        SoundEffectInstance buttonPress = Tankontroller.Instance().GetSoundManager().GetSoundEffectInstance("Sounds/Button_Push");
                        buttonPress.Play();
                        mButtonList.PressSelectedButton();
                        secondsLeft = 0.1f;
                    }
                }
                if (controller.IsPressed(Control.RECHARGE))
                {
                    if (secondsLeft <= 0.0f)
                    {
                        SoundEffectInstance buttonPress = Tankontroller.Instance().GetSoundManager().GetSoundEffectInstance("Sounds/Button_Push");
                        buttonPress.Play();
                        mButtonList.PressSelectedButton();
                        secondsLeft = 0.1f;
                    }
                }

            }
        }
        //Draws the start screen and buttons
        public override void Draw(float pSeconds)
        {
            Tankontroller.Instance().GDM().GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            Color backColour = Color.White;

            spriteBatch.Draw(mBackgroundTexture, mBackgroundRectangle, backColour);
            spriteBatch.Draw(mForgroundTexture, mBackgroundRectangle, backColour);

            spriteBatch.Draw(mTitleTexture, mTitleRectangle, backColour);

            mButtonList.Draw(spriteBatch);

            spriteBatch.End();
        }
        public override void Escape()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Tankontroller.Instance().SM().Transition(null);
            }
        }
    }
}
