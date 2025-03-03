using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Tankontroller.Controller;
using Tankontroller.GUI;

namespace Tankontroller.Scenes
{
    public class LevelSelectionScene : IScene
    {
        private Texture2D mBackgroundTexture;
        private Rectangle mBackgroundRectangle;
        private ButtonList mButtonList;
        private List<string> mMapFiles;
        private Tankontroller mGameInstance;

        public LevelSelectionScene()
        {
            mGameInstance = (Tankontroller)Tankontroller.Instance();
            spriteBatch = new SpriteBatch(mGameInstance.GDM().GraphicsDevice);
            int screenWidth = mGameInstance.GDM().GraphicsDevice.Viewport.Width;
            int screenHeight = mGameInstance.GDM().GraphicsDevice.Viewport.Height;

            mBackgroundTexture = mGameInstance.CM().Load<Texture2D>("background_01");
            mBackgroundRectangle = new Rectangle(0, 0, screenWidth, screenHeight);

            mButtonList = new ButtonList();
            mMapFiles = new List<string> { "Maps/1-3_player_map.json", "Maps/4_player_map.json" };

            for (int i = 0; i < mMapFiles.Count; i++)
            {
                string mapFile = mMapFiles[i];
                Texture2D buttonTexture = mGameInstance.CM().Load<Texture2D>("menu_play_white");
                Texture2D buttonTexturePressed = mGameInstance.CM().Load<Texture2D>("menu_play_dark");
                Rectangle buttonRectangle = new Rectangle((screenWidth - buttonTexture.Width) / 2, (screenHeight / 2) + (i * (buttonTexture.Height + 10)), buttonTexture.Width, buttonTexture.Height);
                Button mapButton = new Button(buttonTexture, buttonTexturePressed, buttonRectangle, Color.Red, () => SelectMap(mapFile));
                mButtonList.Add(mapButton);
            }
        }

        private void SelectMap(string mapFile)
        {
            mGameInstance.SM().Transition(new PlayerSelectionScene(mapFile), true);
        }

        public override void Update(float pSeconds)
        {
            Escape();
            mGameInstance.DetectControllers();

            foreach (IController controller in mGameInstance.GetControllers())
            {
                controller.UpdateController();

                if (controller.IsPressed(Control.LEFT_TRACK_FORWARDS) || controller.IsPressed(Control.TURRET_LEFT))
                {
                    mButtonList.SelectPreviousButton();
                }
                if (controller.IsPressed(Control.LEFT_TRACK_BACKWARDS) || controller.IsPressed(Control.TURRET_RIGHT))
                {
                    mButtonList.SelectNextButton();
                }
                if (controller.IsPressed(Control.FIRE) || controller.IsPressed(Control.RECHARGE))
                {
                    mButtonList.PressSelectedButton();
                }
            }
        }

        public override void Draw(float pSeconds)
        {
            mGameInstance.GDM().GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.Draw(mBackgroundTexture, mBackgroundRectangle, Color.White);
            mButtonList.Draw(spriteBatch);
            spriteBatch.End();
        }

        public override void Escape()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                mGameInstance.SM().Transition(null);
            }
        }
    }
}