using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tankontroller.Controller;
using Tankontroller.GUI;

namespace Tankontroller.Scenes
{
    public class StartScene : IScene
    {
        private static readonly bool SHOW_LIST_ON_MAIN_MENU = DGS.Instance.GetBool("SHOW_LIST_ON_MAIN_MENU");
        IGame mGameInstance = Tankontroller.Instance();

        ButtonList mButtonList = null;
        Texture2D mForgroundTexture = null;
        Texture2D mBackgroundTexture = null;
        Rectangle mBackgroundRectangle;
        Texture2D mTitleTexture = null;
        Rectangle mTitleRectangle;
        Rectangle mControllerInfoRect;
        private float secondsLeft;

        private string defaultMapFile = "Maps/1-3_player_map.json"; // Default map file


        public StartScene()
        {
            spriteBatch = new SpriteBatch(mGameInstance.GDM().GraphicsDevice);
            int screenWidth = mGameInstance.GDM().GraphicsDevice.Viewport.Width;
            int screenHeight = mGameInstance.GDM().GraphicsDevice.Viewport.Height;

            mBackgroundTexture = mGameInstance.CM().Load<Texture2D>("background_01");
            mBackgroundRectangle = new Rectangle(0, 0, screenWidth, screenHeight);

            mForgroundTexture = mGameInstance.CM().Load<Texture2D>("menu_white");
            mTitleTexture = mGameInstance.CM().Load<Texture2D>("menu_title");
            mTitleRectangle = new Rectangle((screenWidth / 2) - (644 / 2), (screenHeight / 2) - (128 / 2), 644, 128);

            mControllerInfoRect = new Rectangle(0, 0, screenWidth / 5, screenHeight);

            mButtonList = new ButtonList();


            //Start Game Button
            Texture2D startGameButtonTexture = mGameInstance.CM().Load<Texture2D>("menu_play_white");
            Texture2D startGameButtonTexturePressed = mGameInstance.CM().Load<Texture2D>("menu_play_dark");

            Rectangle startGameButtonRectangle =
                new Rectangle(
                    ((int)((screenWidth - startGameButtonTexture.Width) / 2) - (int)(startGameButtonTexture.Width * 0.75f)),
                    (screenHeight) / 2 + startGameButtonTexture.Height,
                    startGameButtonTexture.Width,
                    startGameButtonTexture.Height);

            Button startGameButton = new Button(startGameButtonTexture, startGameButtonTexturePressed, startGameButtonRectangle, Color.Red, StartGame);
            startGameButton.Selected = true;
            mButtonList.Add(startGameButton);

            //Makes the exit game button
            Texture2D exitGameButtonTexture = mGameInstance.CM().Load<Texture2D>("menu_quit_white");
            Texture2D exitGameButtonTexturePressed = mGameInstance.CM().Load<Texture2D>("menu_quit_dark");

            Rectangle exitGameButtonRectangle =
                new Rectangle((screenWidth - exitGameButtonTexture.Width) / 2 + (int)(startGameButtonTexture.Width * 0.75f),
                    (screenHeight) / 2 + exitGameButtonTexture.Width,
                    exitGameButtonTexture.Width,
                    exitGameButtonTexture.Height);
            Button exitGameButton = new Button(exitGameButtonTexture, exitGameButtonTexturePressed, exitGameButtonRectangle, Color.Red, ExitGame);
            exitGameButton.Selected = false;
            mButtonList.Add(exitGameButton);

            // Level Selection Button
            Texture2D levelSelectionButtonTexture = tankControllerInstance.CM().Load<Texture2D>("menu_play_white");
            Texture2D levelSelectionButtonTexturePressed = tankControllerInstance.CM().Load<Texture2D>("menu_quit_dark");

            Rectangle levelSelectionButtonRectangle =
                new Rectangle(
                    ((int)((screenWidth - levelSelectionButtonTexture.Width) / 2)),
                    (screenHeight) / 2 + startGameButtonTexture.Height * 2,
                    levelSelectionButtonTexture.Width,
                    levelSelectionButtonTexture.Height);

            Button levelSelectionButton = new Button(levelSelectionButtonTexture, levelSelectionButtonTexturePressed, levelSelectionButtonRectangle, Color.Red, SelectLevel);
            levelSelectionButton.Selected = false;
            mButtonList.Add(levelSelectionButton);

            secondsLeft = 0.1f;
            mGameInstance.GetSoundManager().ReplaceCurrentMusicInstance("Music/Music_start", true);
        }

        public void SetDefaultMapFile(string mapFile)
        {
            defaultMapFile = mapFile;
        }

        private void SelectLevel()
        {
            gameInstance.SM().Transition(new LevelSelectionScene(this), false);
        }

        private void ExitGame()
        {
            mGameInstance.SM().Transition(null);
        }

        private void StartGame()
        {
            mGameInstance.SM().Transition(new PlayerSelectionScene(defaultMapFile), false);
        }

        public override void Update(float pSeconds)
        {
            Escape();
            mGameInstance.GetControllerManager().DetectControllers();

            foreach (IController controller in mGameInstance.GetControllerManager().GetControllers())
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
                        SoundEffectInstance buttonPress = mGameInstance.GetSoundManager().GetSoundEffectInstance("Sounds/Button_Push");
                        buttonPress.Play();
                        mButtonList.PressSelectedButton();
                        secondsLeft = 0.1f;
                    }
                }
            }
        }

        public override void Draw(float pSeconds)
        {
            mGameInstance.GDM().GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            Color backColour = Color.White;

            spriteBatch.Draw(mBackgroundTexture, mBackgroundRectangle, backColour);
            spriteBatch.Draw(mForgroundTexture, mBackgroundRectangle, backColour);

            spriteBatch.Draw(mTitleTexture, mTitleRectangle, backColour);

            if (SHOW_LIST_ON_MAIN_MENU) mGameInstance.GetControllerManager().Draw(spriteBatch, mControllerInfoRect);

            mButtonList.Draw(spriteBatch);

            spriteBatch.End();
        }

        public override void Escape()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                ExitGame();
            }
        }
    }
}
