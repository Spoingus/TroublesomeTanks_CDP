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

using Tankontroller.GUI;
using Tankontroller.Controller;


namespace Tankontroller.Scenes
{

    public class PlayerSelectionScene : IScene
    {
        Texture2D mBackgroundTexture = null;
        Rectangle mBackgroundRectangle;
        Texture2D[] mCountDownTextures = new Texture2D[6];
        Rectangle[] mCountDownRectangles = new Rectangle[6];
        SpriteBatch mSpriteBatch = null;
        List<AvatarPicker> mAvatarPickers;
        float mCountdown;
        bool mPlayersWereReady;

        public PlayerSelectionScene()
        {
            Tankontroller game = (Tankontroller)Tankontroller.Instance();

            mSpriteBatch = new SpriteBatch(game.GDM().GraphicsDevice);
            int screenWidth = game.GDM().GraphicsDevice.Viewport.Width;
            int screenHeight = game.GDM().GraphicsDevice.Viewport.Height;

            mBackgroundTexture = game.CM().Load<Texture2D>("background_06");
            mBackgroundRectangle = new Rectangle(0, 0, screenWidth, screenHeight);

            game.ReplaceCurrentMusicInstance("Music/Music_start", true);
            prepareAvatarPickers();
            mPlayersWereReady = false;
            prepareCountDown();


        }
        private void prepareCountDown()
        {
            Tankontroller game = (Tankontroller)Tankontroller.Instance();
            int screenWidth = game.GDM().GraphicsDevice.Viewport.Width;
            int screenHeight = game.GDM().GraphicsDevice.Viewport.Height;
            mCountDownTextures[0] = game.CM().Load<Texture2D>("countdown/five");
            mCountDownTextures[1] = game.CM().Load<Texture2D>("countdown/four");
            mCountDownTextures[2] = game.CM().Load<Texture2D>("countdown/three");
            mCountDownTextures[3] = game.CM().Load<Texture2D>("countdown/two");
            mCountDownTextures[4] = game.CM().Load<Texture2D>("countdown/one");
            mCountDownTextures[5] = game.CM().Load<Texture2D>("countdown/ready");

            for (int i = 0; i < mCountDownTextures.Length; i++)
            {
                Texture2D texture = mCountDownTextures[i];
                float widthRatio = (float)texture.Width / texture.Height;
                int height = 200;
                int width = (int)(height * widthRatio);
                Rectangle rectangle = new Rectangle((screenWidth - width) / 2, (screenHeight - height) / 2, width, height);
                mCountDownRectangles[i] = rectangle;
            }

        }
        private void prepareAvatarPickers() // this is quite slow, I assume because it is loading all the textures;
        {
            Tankontroller game = (Tankontroller)Tankontroller.Instance();

            int screenWidth = game.GDM().GraphicsDevice.Viewport.Width;
            int screenHeight = game.GDM().GraphicsDevice.Viewport.Height;
            int halfWidth = screenWidth / 2;
            int halfHeight = screenHeight / 2;
            mAvatarPickers = new List<AvatarPicker>();
            int left = 0;
            int top = 0;
            Rectangle rectangle = new Rectangle(left, top, halfWidth, halfHeight);
            mAvatarPickers.Add(new AvatarPicker(rectangle));
            left += halfWidth;
            rectangle = new Rectangle(left, top, halfWidth, halfHeight);
            mAvatarPickers.Add(new AvatarPicker(rectangle));
            left -= halfWidth;
            top += halfHeight;
            rectangle = new Rectangle(left, top, halfWidth, halfHeight);
            mAvatarPickers.Add(new AvatarPicker(rectangle));
            left += halfWidth;
            rectangle = new Rectangle(left, top, halfWidth, halfHeight);
            mAvatarPickers.Add(new AvatarPicker(rectangle));
        }



        private void ExitGame()
        {
            IGame game = Tankontroller.Instance();
            game.SM().Transition(null);
        }

        private List<Player> getReadyPlayers()
        {
            List<Player> players = new List<Player>();
            foreach (AvatarPicker avatarPicker in mAvatarPickers)
            {
                if (avatarPicker.HasPlayer())
                {
                    players.Add(avatarPicker.GetPlayer());
                }
            }
            return players;
        }

        private void StartGame()
        {
            IGame game = Tankontroller.Instance();
            game.SM().Transition(new GameScene(getReadyPlayers()), true);
        }

        private Player makeNewPlayer(IController pController)
        {
            Player player = new Player(pController);
            AvatarPicker avatarPicker = getEmptyAvatarPicker();
            avatarPicker.AddPlayer(player);
            return player;
        }

        private AvatarPicker getEmptyAvatarPicker()
        {
            foreach (AvatarPicker avatarPicker in mAvatarPickers)
            {
                if (!avatarPicker.HasPlayer())
                {
                    return avatarPicker;
                }
            }
            return null;
        }
        private AvatarPicker getControllerAvatarPicker(IController pController)
        {
            foreach (AvatarPicker avatarPicker in mAvatarPickers)
            {
                if (avatarPicker.HasPlayer())
                {
                    if (avatarPicker.GetPlayer().Controller == pController)
                    {
                        return avatarPicker;
                    }
                }
            }
            return null;
        }


        private AvatarPicker getPlayerAvatarPicker(Player pPlayer)
        {
            foreach (AvatarPicker avatarPicker in mAvatarPickers)
            {
                if (avatarPicker.GetPlayer() == pPlayer)
                {
                    return avatarPicker;
                }
            }
            return null;
        }

        private bool playersReady()
        {
            int countAvatarPickerWithPlayers = 0;
            int countReadyPlayers = 0;
            foreach (AvatarPicker avatarPicker in mAvatarPickers)
            {
                if (avatarPicker.HasPlayer())
                {
                    countAvatarPickerWithPlayers++;
                    if (avatarPicker.GetPlayer().ColourSet)
                    {
                        countReadyPlayers++;
                    }
                }
            }
            if (countReadyPlayers > 1 && countReadyPlayers == countAvatarPickerWithPlayers)
            {
                return true;
            }
            return false;
        }

        private void updateAvatarPickers(float pSeconds)
        {
            foreach (AvatarPicker avatarPicker in mAvatarPickers)
            {
                avatarPicker.Update(pSeconds);
            }
        }

        public void Update(float pSeconds)
        {
            Escape();
            IGame game = Tankontroller.Instance();
            game.DetectControllers();

            updateAvatarPickers(pSeconds);
            for (int i = 0; i < game.GetControllerCount(); i++)
            {
                IController controller = game.GetController(i);
                AvatarPicker avatarPicker = getControllerAvatarPicker(controller);

                if (!controller.IsConnected() && avatarPicker != null)
                {
                    int index = mAvatarPickers.IndexOf(avatarPicker);
                    mAvatarPickers[index] = new AvatarPicker(avatarPicker.Rect);
                    return;
                }
                if (avatarPicker == null)
                {
                    controller.UpdateController();
                }
                if (controller.IsPressed(Control.FIRE))
                {
                    if (avatarPicker == null)
                    {
                        makeNewPlayer(controller);
                    }
                }
            }
            if (playersReady())
            {
                if (!mPlayersWereReady)
                {
                    mCountdown = 3f;
                    mPlayersWereReady = true;
                }
                // players ready do the countdown
                mCountdown -= pSeconds;
                if (mCountdown <= 0)
                {
                    StartGame();
                }
            }
            else
            {
                mPlayersWereReady = false;
            }
        }
        public void Escape()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Tankontroller.Instance().SM().Transition(null);
            }
        }
        private void DrawAvatarPickers(SpriteBatch pSpriteBatch)
        {
            foreach (AvatarPicker avatarPicker in mAvatarPickers)
            {
                avatarPicker.Draw(pSpriteBatch);
            }
        }
        private void DrawCountDown(SpriteBatch pSpriteBatch)
        {
            if (mPlayersWereReady)
            {
                int index = 5;
                if (mCountdown > 4)
                {
                    index = 0;
                }
                else if (mCountdown > 3)
                {
                    index = 1;
                }
                else if (mCountdown > 2)
                {
                    index = 2;
                }
                else if (mCountdown > 1)
                {
                    index = 3;
                }
                else if (mCountdown > 0)
                {
                    index = 4;
                }
                else
                {
                    index = 5;
                }
                Texture2D texture = mCountDownTextures[index];
                Rectangle rectangle = mCountDownRectangles[index];
                mSpriteBatch.Draw(texture, rectangle, Color.White);
            }
        }
        public void Draw(float pSeconds)
        {
            Tankontroller.Instance().GDM().GraphicsDevice.Clear(Color.Black);
            mSpriteBatch.Begin();
            Color backColour = Color.White;

            mSpriteBatch.Draw(mBackgroundTexture, mBackgroundRectangle, backColour);

            DrawAvatarPickers(mSpriteBatch);
            DrawCountDown(mSpriteBatch);
            mSpriteBatch.End();
        }
    }
}