using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Tankontroller.Controller;

namespace Tankontroller.GUI
{
    //-------------------------------------------------------------------------------------------------
    // AvatarPicker
    //
    // This class is used to represent the avatar picker in the game. It displays the avatars and colours
    // that the player can choose from.
    //
    // It contains the following member variables:
    //
    // - A rectangle representing the position of the avatar picker
    // - A list of avatars
    // - A list of colours
    // - A list of rectangles representing the position of the selection
    // - A white pixel, circle, A button, back button, rotate left button, rotate right button and back text texture
    // - A rectangle representing the position of the A button, back button, rotate left button, rotate right button and back text
    // - A vector representing the centre of the avatar picker
    // - A float representing the radius of the avatar and avatar picker
    // - A player
    // - A rectangle representing the position of the avatar
    // - A float representing the countdown
    // - A float representing the A button countdown
    // - A boolean representing whether the A button is shown
    //
    // It contains the following methods:
    //
    // - A constructor to initialise the avatar picker
    // - A method to add a player
    // - A method to check if the player exists
    // - A method to get the player
    // - A method to check if the player is ready
    // - A method to remove the player
    // - A method to prepare the draw variables, avatars, colours, selection rectangles and buttons
    // - A method to draw the avatars, colours, selection, A button, bounds and the avatar picker
    // - A method to reposition the avatar picker
    // - A method to update the avatar picker
    //-------------------------------------------------------------------------------------------------

    public class AvatarPicker
    {
        private Rectangle mBoundsRectangle;
        List<Avatar> mAvatars;
        List<Avatar> mColours;
        List<Rectangle> mSelectionRectangles;
        Texture2D mWhitePixel;
        Texture2D mCircle;
        Texture2D mAButtonTexture;
        Texture2D mBackButtonTexture;
        Texture2D mRotateLeftTexture;
        Texture2D mRotateRightTexture;
        Texture2D mBackTextTexture;
        Rectangle mAButtonRectangle;
        Rectangle mBackButtonRectangle;
        Rectangle mRotateLeftButton;
        Rectangle mRotateRightButton;
        Rectangle mBackTextRectangle;
        Vector2 mCentre;
        float mRadius;
        float mAvatarRadius;
        Player mPlayer;
        Rectangle mAvatarRectangle;
        float mCountDown;
        float mAButtonCountDown;
        bool mShowAButton;

        public Rectangle Rect { get { return mBoundsRectangle; } }

        public AvatarPicker(Rectangle pRectangle)
        {
            mBoundsRectangle = pRectangle;
            //prepareColours();
            mPlayer = null;
            Tankontroller game = (Tankontroller)Tankontroller.Instance();
            mWhitePixel = game.CM().Load<Texture2D>("white_pixel");
            mCircle = game.CM().Load<Texture2D>("circle");
            prepareDrawVariables();
            prepareAvatars();
            prepareColours("engineer");
            prepareSelectionRectangles(mAvatars.Count);
            prepareButtons();
            mShowAButton = true;
        }

        private void prepareButtons()
        {
            Tankontroller game = (Tankontroller)Tankontroller.Instance();
            mAButtonTexture = game.CM().Load<Texture2D>("fire");
            float widthRatio = (float)mAButtonTexture.Width / mAButtonTexture.Height;
            int height = 200;
            int width = (int)(height * widthRatio);
            mAButtonRectangle = new Rectangle(mBoundsRectangle.X + (mBoundsRectangle.Width - width) / 2, mBoundsRectangle.Y + (mBoundsRectangle.Height - height) / 2, width, height);

            height = 60;
            width = (int)(height * widthRatio);
            mBackButtonTexture = game.CM().Load<Texture2D>("charge");
            mBackButtonRectangle = new Rectangle(mBoundsRectangle.X + (mBoundsRectangle.Width / 16) - width, mBoundsRectangle.Y + (mBoundsRectangle.Height / 8) - height, width, height);

            height = 40;
            width = (int)(height * widthRatio);
            mBackTextTexture = game.CM().Load<Texture2D>("back");
            mBackTextRectangle = new Rectangle(mBoundsRectangle.X + ((mBoundsRectangle.Width / 16) + 30) - width, mBoundsRectangle.Y + (mBoundsRectangle.Height / 8) - height, width, height / 2);

            mRotateLeftTexture = game.CM().Load<Texture2D>("turretLeft");
            mRotateLeftButton = new Rectangle(mBoundsRectangle.X + ((mBoundsRectangle.Width / 10) * 3) - width + 20, mBoundsRectangle.Y + (mBoundsRectangle.Height / 8) - height, width, height);

            mRotateRightTexture = game.CM().Load<Texture2D>("turretRight");
            mRotateRightButton = new Rectangle(mBoundsRectangle.X + ((mBoundsRectangle.Width / 10) * 7) - width + 20, mBoundsRectangle.Y + (mBoundsRectangle.Height / 8) - height, width, height);
        }
        public void AddPlayer(Player pPlayer)
        {
            mPlayer = pPlayer;
            mPlayer.SelectionIndex = 0;
            Avatar avatar = mAvatars[mPlayer.SelectionIndex];
            string name = avatar.GetName();

            avatar = new Avatar(mWhitePixel, name, mAvatarRectangle, Color.White);
            mPlayer.AddAvatar(avatar);
        }
        public bool HasPlayer()
        {
            if (mPlayer != null)
            {
                return true;
            }
            return false;
        }
        public Player GetPlayer()
        {
            return mPlayer;
        }
        public bool Ready()
        {
            if (HasPlayer())
            {
                if (mPlayer.ColourSet)
                {
                    return true;
                }
            }
            return false;
        }
        public void RemovePlayer()
        {
            mPlayer = null;
        }
        private void prepareDrawVariables()
        {
            mCentre = new Vector2(mBoundsRectangle.X + mBoundsRectangle.Width / 2, mBoundsRectangle.Y + mBoundsRectangle.Height / 2);
            mAvatarRadius = 70;
            mRadius = mBoundsRectangle.Height / 2 - mAvatarRadius;
            prepareCentreAvatar();
        }
        private void prepareCentreAvatar()
        {
            float middleAvatarRadius = 100;
            if (HasPlayer())
            {
                if (mPlayer.ColourSet)
                {
                    middleAvatarRadius = mBoundsRectangle.Height * 0.45f;
                }

            }
            mAvatarRectangle = new Rectangle((int)(mCentre.X - middleAvatarRadius), (int)(mCentre.Y - middleAvatarRadius), (int)(middleAvatarRadius * 2), (int)(middleAvatarRadius * 2));
        }
        private Avatar prepareAvatar(string pAvatarName, Rectangle pRectangle, Color pColour)
        {
            Tankontroller game = (Tankontroller)Tankontroller.Instance();
            Avatar avatar = new Avatar(mWhitePixel, pAvatarName, pRectangle, pColour);
            return avatar;
        }
        private void prepareAvatars()
        {
            Tankontroller game = (Tankontroller)Tankontroller.Instance();
            int screenWidth = mBoundsRectangle.Width;
            int screenHeight = mBoundsRectangle.Height;
            mAvatars = new List<Avatar>();
            List<string> avatarStrings = new List<string>
            {
                "engineer",
                "robo",
                "winterjack",
                "yeti"
            };
            // need to add a bunch of different avatars here
            float anglePerAvatar = (float)(Math.PI * 2 / avatarStrings.Count);

            float centreX = mCentre.X;
            float centreY = mCentre.Y;
            float topLeftAngle = (float)Math.PI * 5 / 4;
            float bottomRightAngle = (float)Math.PI / 4;
            for (int i = 0; i < avatarStrings.Count; i++)
            {
                float angle = i * anglePerAvatar;
                float x = centreX + (float)Math.Cos(angle) * mRadius;
                float y = centreY + (float)Math.Sin(angle) * mRadius;
                float topLeftX = x + (float)Math.Cos(topLeftAngle) * mAvatarRadius;
                float topLeftY = y + (float)Math.Sin(topLeftAngle) * mAvatarRadius;
                float bottomRightX = x + (float)Math.Cos(bottomRightAngle) * mAvatarRadius;
                float bottomRightY = y + (float)Math.Sin(bottomRightAngle) * mAvatarRadius;
                float width = bottomRightX - topLeftX;
                float height = bottomRightY - topLeftY;
                Rectangle rectangle = new Rectangle((int)topLeftX, (int)topLeftY, (int)width, (int)height);
                string avatarString = avatarStrings[i];
                Avatar avatar = prepareAvatar(avatarString, rectangle, Color.White);
                mAvatars.Add(avatar);
            }
        }
        private void prepareSelectionRectangles(int selectionCount)
        {
            Tankontroller game = (Tankontroller)Tankontroller.Instance();
            int screenWidth = mBoundsRectangle.Width;
            int screenHeight = mBoundsRectangle.Height;
            mSelectionRectangles = new List<Rectangle>();
            // need to add a bunch of different avatars here
            float anglePerAvatar = (float)(Math.PI * 2 / selectionCount);

            float centreX = mCentre.X;
            float centreY = mCentre.Y;
            float topLeftAngle = (float)Math.PI * 5 / 4;
            float bottomRightAngle = (float)Math.PI / 4;
            for (int i = 0; i < selectionCount; i++)
            {
                float angle = i * anglePerAvatar;
                float x = centreX + (float)Math.Cos(angle) * mRadius;
                float y = centreY + (float)Math.Sin(angle) * mRadius;
                float topLeftX = x + (float)Math.Cos(topLeftAngle) * mAvatarRadius;
                float topLeftY = y + (float)Math.Sin(topLeftAngle) * mAvatarRadius;
                float bottomRightX = x + (float)Math.Cos(bottomRightAngle) * mAvatarRadius;
                float bottomRightY = y + (float)Math.Sin(bottomRightAngle) * mAvatarRadius;
                float width = bottomRightX - topLeftX;
                float height = bottomRightY - topLeftY;
                Rectangle rectangle = new Rectangle((int)topLeftX, (int)topLeftY, (int)width, (int)height);
                mSelectionRectangles.Add(rectangle);
            }
        }
        private void prepareColours(string pName)
        {
            int screenWidth = mBoundsRectangle.Width;
            int screenHeight = mBoundsRectangle.Height;
            mColours = new List<Avatar>();
            List<Color> colours = new List<Color>();
            colours.Add(Color.Red);
            colours.Add(Color.Lime);
            colours.Add(Color.DarkGreen);
            colours.Add(Color.MonoGameOrange);
            colours.Add(Color.Blue);
            colours.Add(Color.Yellow);
            colours.Add(Color.NavajoWhite);
            colours.Add(Color.DarkSlateGray);
            colours.Add(Color.SaddleBrown);
            colours.Add(Color.Indigo);
            colours.Add(Color.Magenta);
            colours.Add(Color.Aqua);

            // need to add a bunch of different colours here
            float anglePerAvatar = (float)(Math.PI * 2 / colours.Count);

            float centreX = mCentre.X;
            float centreY = mCentre.Y;
            float topLeftAngle = (float)Math.PI * 5 / 4;
            float bottomRightAngle = (float)Math.PI / 4;
            for (int i = 0; i < colours.Count; i++)
            {
                float angle = i * anglePerAvatar;
                float x = centreX + (float)Math.Cos(angle) * mRadius;
                float y = centreY + (float)Math.Sin(angle) * mRadius;
                float topLeftX = x + (float)Math.Cos(topLeftAngle) * mAvatarRadius;
                float topLeftY = y + (float)Math.Sin(topLeftAngle) * mAvatarRadius;
                float bottomRightX = x + (float)Math.Cos(bottomRightAngle) * mAvatarRadius;
                float bottomRightY = y + (float)Math.Sin(bottomRightAngle) * mAvatarRadius;
                float width = bottomRightX - topLeftX;
                float height = bottomRightY - topLeftY;
                Rectangle rectangle = new Rectangle((int)topLeftX, (int)topLeftY, (int)width, (int)height);
                Color colour = colours[i];
                string avatarString = pName;
                Avatar avatar = prepareAvatar(avatarString, rectangle, colour);
                mColours.Add(avatar);
            }
        }


        public void DrawAvatars(SpriteBatch pSpriteBatch)
        {
            foreach (Avatar avatar in mAvatars)
            {
                avatar.Draw(pSpriteBatch, true, 0);
            }
            pSpriteBatch.Draw(mRotateLeftTexture, mRotateLeftButton, Color.White);
            pSpriteBatch.Draw(mRotateRightTexture, mRotateRightButton, Color.White);
        }

        public void DrawColours(SpriteBatch pSpriteBatch)
        {
            foreach (Avatar avatar in mColours)
            {
                avatar.Draw(pSpriteBatch, true, 0);
            }
            pSpriteBatch.Draw(mRotateLeftTexture, mRotateLeftButton, Color.White);
            pSpriteBatch.Draw(mRotateRightTexture, mRotateRightButton, Color.White);
        }
        public void DrawSelection(SpriteBatch pSpriteBatch)
        {
            if (HasPlayer())
            {
                pSpriteBatch.Draw(mCircle, mSelectionRectangles[mPlayer.SelectionIndex], Color.White);
            }

        }

        public void DrawAButton(SpriteBatch pSpriteBatch)
        {
            if (mShowAButton)
            {
                pSpriteBatch.Draw(mAButtonTexture, mAButtonRectangle, Color.White);
            }

        }


        public void Reposition(Rectangle pRectangle)
        {
            mBoundsRectangle = pRectangle;
        }

        public void DrawBounds(SpriteBatch pSpriteBatch)
        {
            Color boundColour = Color.White;
            boundColour.A = (byte)0.5f;
            pSpriteBatch.Draw(mWhitePixel, mBoundsRectangle, boundColour);
        }

        public void Draw(SpriteBatch pSpriteBatch)
        {
            //DrawBounds(pSpriteBatch);
            if (mPlayer != null)
            {
                mPlayer.Avatar.Draw(pSpriteBatch, true, 0);
                if (!mPlayer.AvatarSet)
                {
                    DrawSelection(pSpriteBatch);
                    DrawAvatars(pSpriteBatch);
                }
                else
                {
                    if (!mPlayer.ColourSet)
                    {
                        DrawSelection(pSpriteBatch);
                        DrawColours(pSpriteBatch);
                    }
                }
                pSpriteBatch.Draw(mBackButtonTexture, mBackButtonRectangle, Color.White);
                pSpriteBatch.Draw(mBackTextTexture, mBackTextRectangle, Color.White);
            }
            else
            {
                //draw a button
                DrawAButton(pSpriteBatch);
            }


        }

        bool WasFirePressed = true;
        bool WasBackPressed = false;
        public void Update(float pSeconds)
        {
            IGame game = Tankontroller.Instance();
            mCountDown -= pSeconds;
            if (HasPlayer())
            {
                mShowAButton = false;
                IController controller = mPlayer.Controller;
                controller.UpdateController();

                if (controller.IsPressed(Control.FIRE))
                {
                    if (!WasFirePressed)
                    {
                        // if this controller is not already a player
                        // need to make a player here as this controller wants to play
                        if (!mPlayer.AvatarSet)
                        {
                            // add an avatar to the player
                            mPlayer.SetAvatar();
                            mPlayer.SelectionIndex = 0;
                            Color colour = mColours[mPlayer.SelectionIndex].GetColour();
                            mPlayer.AddColour(colour);
                            mPlayer.Avatar.SetColour(colour);
                            string name = mPlayer.Avatar.GetName();
                            prepareColours(name);
                            prepareSelectionRectangles(mColours.Count);
                        }
                        else if (!mPlayer.ColourSet)
                        {
                            // add a colour to the player
                            mPlayer.SetColour();
                            mPlayer.SelectionIndex = 0;
                            prepareCentreAvatar();
                            mPlayer.Avatar.Reposition(mAvatarRectangle);
                        }
                    }
                }
                if (controller.IsPressed(Control.RECHARGE))
                {
                    if (!WasBackPressed)
                    {
                        // if this controller is already a player
                        // need to remove this player
                        if (mPlayer != null)
                        {
                            mPlayer.SelectionIndex = 0;
                            if (mPlayer.ColourSet)
                            {
                                mPlayer.RemoveColour();
                                Color colour = mColours[mPlayer.SelectionIndex].GetColour();
                                mPlayer.AddColour(colour);
                                mPlayer.Avatar.SetColour(colour);
                                prepareCentreAvatar();
                                mPlayer.Avatar.Reposition(mAvatarRectangle);
                            }
                            else if (mPlayer.AvatarSet)
                            {
                                mPlayer.RemoveAvatar();
                                Avatar avatar = mAvatars[mPlayer.SelectionIndex];
                                string name = avatar.GetName();

                                avatar = new Avatar(mWhitePixel, name, mAvatarRectangle, Color.White);
                                mPlayer.AddAvatar(avatar);
                                prepareSelectionRectangles(mAvatars.Count);
                            }
                            else
                            {
                                mPlayer = null;
                                WasFirePressed = true;
                                return;
                            }
                        }
                    }
                }
                if (controller.IsPressed(Control.TURRET_RIGHT))
                {
                    // if controller is already a player
                    // cycle through the options
                    if (mPlayer != null)
                    {
                        if (mCountDown <= 0)
                        {
                            mCountDown = 0.2f;
                            mPlayer.SelectionIndex--;
                            if (mPlayer.SelectionIndex < 0)
                            {
                                if (!mPlayer.AvatarSet)
                                {
                                    mPlayer.SelectionIndex = mAvatars.Count - 1;
                                }
                                else if (!mPlayer.ColourSet)
                                {
                                    mPlayer.SelectionIndex = mColours.Count - 1;
                                }
                            }
                            if (!mPlayer.AvatarSet)
                            {
                                Avatar avatar = mAvatars[mPlayer.SelectionIndex];
                                string name = avatar.GetName();

                                avatar = new Avatar(mWhitePixel, name, mAvatarRectangle, Color.White);
                                mPlayer.AddAvatar(avatar);
                            }
                            else if (!mPlayer.ColourSet)
                            {
                                Color colour = mColours[mPlayer.SelectionIndex].GetColour();
                                mPlayer.AddColour(colour);
                                mPlayer.Avatar.SetColour(colour);
                            }
                        }
                    }
                }
                else if (controller.IsPressed(Control.TURRET_LEFT))
                {
                    // if controller is already a player
                    // cycle through the options
                    if (mPlayer != null)
                    {
                        if (mCountDown <= 0)
                        {
                            mCountDown = 0.2f;
                            mPlayer.SelectionIndex++;

                            if (!mPlayer.AvatarSet)
                            {
                                if (mPlayer.SelectionIndex >= mAvatars.Count)
                                {
                                    mPlayer.SelectionIndex = 0;
                                }
                            }
                            else if (!mPlayer.ColourSet)
                            {
                                if (mPlayer.SelectionIndex >= mColours.Count)
                                {
                                    mPlayer.SelectionIndex = 0;
                                }
                            }
                        }
                        if (!mPlayer.AvatarSet)
                        {
                            Avatar avatar = mAvatars[mPlayer.SelectionIndex];
                            string name = avatar.GetName();

                            avatar = new Avatar(mWhitePixel, name, mAvatarRectangle, Color.White);
                            mPlayer.AddAvatar(avatar);
                        }
                        else if (!mPlayer.ColourSet)
                        {
                            Color colour = mColours[mPlayer.SelectionIndex].GetColour();
                            mPlayer.AddColour(colour);
                            mPlayer.Avatar.SetColour(colour);
                        }
                    }
                }
                WasFirePressed = controller.IsPressed(Control.FIRE);
                WasBackPressed = controller.IsPressed(Control.RECHARGE);
            }
            else
            {
                mAButtonCountDown -= pSeconds;
                if (mAButtonCountDown <= 0)
                {
                    if (mShowAButton)
                    {
                        mAButtonCountDown = 0.25f;
                    }
                    else
                    {
                        mAButtonCountDown = 1f;
                    }
                    mShowAButton = !mShowAButton;
                }
            }
        }
    }
}
