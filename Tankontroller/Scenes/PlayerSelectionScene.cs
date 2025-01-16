using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Tankontroller.Controller;
using Tankontroller.GUI;


namespace Tankontroller.Scenes
{
    //-------------------------------------------------------------------------------------------------
    // PlayerSelectionScene
    //
    // This class is used to display the player selection screen.
    // The player selection screen displays the avatars of the players, and allows the players to
    // select their avatars and colours.
    //
    // The class contains a background texture, a sprite batch, a rectangle to draw the background, the list of
    // avatar pickers, the countdown textures, the countdown rectangles, the countdown, and a boolean to
    // check if the players were ready.
    //
    // The class provides methods to:
    //  - exit the game
    //  - get the ready players
    //  - start the game
    //  - make a new player
    //  - get an empty avatar picker
    //  - get the controller avatar picker
    //  - get the player avatar picker
    //  - check if the players are ready
    //  - update the avatar pickers
    //  - draw the avatar pickers and countdown.
    //-------------------------------------------------------------------------------------------------
    public class PlayerSelectionScene : IScene
    {
        IGame gameInstance = Tankontroller.Instance();
        Tankontroller tankControllerInstance = (Tankontroller)Tankontroller.Instance();
        Texture2D mBackgroundTexture = null;
        Rectangle mBackgroundRectangle;
        Texture2D[] mCountDownTextures = new Texture2D[6];
        Rectangle[] mCountDownRectangles = new Rectangle[6];
        List<AvatarPicker> mAvatarPickers;
        float mCountdown;
        bool mPlayersWereReady;

        public PlayerSelectionScene()
        {
            spriteBatch = new SpriteBatch(tankControllerInstance.GDM().GraphicsDevice);
            int screenWidth = tankControllerInstance.GDM().GraphicsDevice.Viewport.Width;
            int screenHeight = tankControllerInstance.GDM().GraphicsDevice.Viewport.Height;

            mBackgroundTexture = tankControllerInstance.CM().Load<Texture2D>("background_06");
            mBackgroundRectangle = new Rectangle(0, 0, screenWidth, screenHeight);

            tankControllerInstance.ReplaceCurrentMusicInstance("Music/Music_start", true);
            prepareAvatarPickers();
            mPlayersWereReady = false;
            prepareCountDown();


        }
        private void prepareCountDown()
        {
            int screenWidth = tankControllerInstance.GDM().GraphicsDevice.Viewport.Width;
            int screenHeight = tankControllerInstance.GDM().GraphicsDevice.Viewport.Height;
            mCountDownTextures[0] = tankControllerInstance.CM().Load<Texture2D>("countdown/five");
            mCountDownTextures[1] = tankControllerInstance.CM().Load<Texture2D>("countdown/four");
            mCountDownTextures[2] = tankControllerInstance.CM().Load<Texture2D>("countdown/three");
            mCountDownTextures[3] = tankControllerInstance.CM().Load<Texture2D>("countdown/two");
            mCountDownTextures[4] = tankControllerInstance.CM().Load<Texture2D>("countdown/one");
            mCountDownTextures[5] = tankControllerInstance.CM().Load<Texture2D>("countdown/ready");

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
            int screenWidth = tankControllerInstance.GDM().GraphicsDevice.Viewport.Width;
            int screenHeight = tankControllerInstance.GDM().GraphicsDevice.Viewport.Height;
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

        //private void ExitGame()
        //{
        //    IGame game = Tankontroller.Instance();
        //    game.SM().Transition(null);
        //}

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
            gameInstance.SM().Transition(new GameScene(getReadyPlayers()), true);
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


        //private AvatarPicker getPlayerAvatarPicker(Player pPlayer)
        //{
        //    foreach (AvatarPicker avatarPicker in mAvatarPickers)
        //    {
        //        if (avatarPicker.GetPlayer() == pPlayer)
        //        {
        //            return avatarPicker;
        //        }
        //    }
        //    return null;
        //}

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
            gameInstance.DetectControllers();

            updateAvatarPickers(pSeconds);
            for (int i = 0; i < gameInstance.GetControllerCount(); i++)
            {
                IController controller = gameInstance.GetController(i);
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
                spriteBatch.Draw(texture, rectangle, Color.White);
            }
        }
        public void Draw(float pSeconds)
        {
            Tankontroller.Instance().GDM().GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            Color backColour = Color.White;

            spriteBatch.Draw(mBackgroundTexture, mBackgroundRectangle, backColour);

            DrawAvatarPickers(spriteBatch);
            DrawCountDown(spriteBatch);
            spriteBatch.End();
        }
        public SpriteBatch spriteBatch { get; set; }
    }
}