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
    //-------------------------------------------------------------------------------------------------
    public class AvatarPicker
    {
        private Rectangle mBoundsRectangle;
        List<Avatar> mAvatars;
        List<Avatar> mColours;
        List<Rectangle> mSelectionRectangles;
        Texture2D mWhitePixel;
        Texture2D mCircle;
        Texture2D mJoinButtonTexture;
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
        bool mShowJoinButton;

        public Rectangle Rect { get { return mBoundsRectangle; } }

        public AvatarPicker(Rectangle pRectangle)
        {
            mBoundsRectangle = pRectangle;
            mPlayer = null;
            mShowJoinButton = true;

            // Load Textures
            Tankontroller game = (Tankontroller)Tankontroller.Instance();
            mWhitePixel = game.CM().Load<Texture2D>("white_pixel");
            mCircle = game.CM().Load<Texture2D>("circle");
            mJoinButtonTexture = game.CM().Load<Texture2D>("fire");
            mBackButtonTexture = game.CM().Load<Texture2D>("charge");
            mBackTextTexture = game.CM().Load<Texture2D>("back");
            mRotateLeftTexture = game.CM().Load<Texture2D>("turretLeft");
            mRotateRightTexture = game.CM().Load<Texture2D>("turretRight");
            PrepareButtons();

            // Prepare Draw Variables
            mCentre = new Vector2(mBoundsRectangle.X + mBoundsRectangle.Width / 2, mBoundsRectangle.Y + mBoundsRectangle.Height / 2);
            mAvatarRadius = 70;
            mRadius = mBoundsRectangle.Height / 2 - mAvatarRadius;
            PrepareCentreAvatar();
            SetUpAvatarSelection();
            PrepareColourSelection("engineer");
            mSelectionRectangles = GetSelectionRectangles(mAvatars.Count);
        }

        public void AddPlayer(Player pPlayer)
        {
            mPlayer = pPlayer;
            mPlayer.SelectionIndex = 0;
            string name = mAvatars[mPlayer.SelectionIndex].GetName();
            Avatar avatar = new Avatar(mWhitePixel, name, mAvatarRectangle, Color.White);
            mPlayer.AddAvatar(avatar);
        }

        public bool Ready() { return HasPlayer() && mPlayer.AvatarSet && mPlayer.ColourSet; }
        public bool HasPlayer() { return mPlayer != null; }
        public Player GetPlayer() { return mPlayer; }
        public void RemovePlayer() { mPlayer = null; }

        public void Reposition(Rectangle pRectangle)
        {
            mBoundsRectangle = pRectangle;
        }

        private void PrepareButtons()
        {
            float widthRatio = (float)mJoinButtonTexture.Width / mJoinButtonTexture.Height;
            int height = 200;
            int width = (int)(height * widthRatio);
            mAButtonRectangle = new Rectangle(mBoundsRectangle.X + (mBoundsRectangle.Width - width) / 2, mBoundsRectangle.Y + (mBoundsRectangle.Height - height) / 2, width, height);

            height = 60;
            width = (int)(height * widthRatio);
            mBackButtonRectangle = new Rectangle(mBoundsRectangle.X + (mBoundsRectangle.Width / 16) - width, mBoundsRectangle.Y + (mBoundsRectangle.Height / 8) - height, width, height);

            height = 40;
            width = (int)(height * widthRatio);
            mBackTextRectangle = new Rectangle(mBoundsRectangle.X + ((mBoundsRectangle.Width / 16) + 30) - width, mBoundsRectangle.Y + (mBoundsRectangle.Height / 8) - height, width, height / 2);

            mRotateLeftButton = new Rectangle(mBoundsRectangle.X + ((mBoundsRectangle.Width / 10) * 3) - width + 20, mBoundsRectangle.Y + (mBoundsRectangle.Height / 8) - height, width, height);

            mRotateRightButton = new Rectangle(mBoundsRectangle.X + ((mBoundsRectangle.Width / 10) * 7) - width + 20, mBoundsRectangle.Y + (mBoundsRectangle.Height / 8) - height, width, height);
        }
        private void PrepareCentreAvatar()
        {
            float middleAvatarRadius = 100;
            if (HasPlayer() && mPlayer.ColourSet)
            {
                middleAvatarRadius = mBoundsRectangle.Height * 0.45f;
            }
            mAvatarRectangle = new Rectangle((int)(mCentre.X - middleAvatarRadius), (int)(mCentre.Y - middleAvatarRadius), (int)(middleAvatarRadius * 2), (int)(middleAvatarRadius * 2));
        }

        private List<Rectangle> GetSelectionRectangles(int pSelectionCount)
        {
            var output = new List<Rectangle>();
            float anglePerAvatar = (float)(Math.PI * 2 / pSelectionCount);
            float centreX = mCentre.X;
            float centreY = mCentre.Y;
            float topLeftAngle = (float)Math.PI * 5 / 4;
            float bottomRightAngle = (float)Math.PI / 4;
            for (int i = 0; i < pSelectionCount; i++)
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
                output.Add(rectangle);
            }
            return output;
        }

        private void SetUpAvatarSelection()
        {
            mAvatars = new List<Avatar>();
            List<string> avatarStrings = new List<string>
            {
                "engineer",
                "robo",
                "winterjack",
                "yeti"
            };

            var rectangles = GetSelectionRectangles(avatarStrings.Count);
            for (int i = 0; i < avatarStrings.Count; i++)
            {
                Avatar avatar = new Avatar(mWhitePixel, avatarStrings[i], rectangles[i], Color.White);
                mAvatars.Add(avatar);
            }
        }
        private void PrepareColourSelection(string pAvatarString)
        {
            int screenWidth = mBoundsRectangle.Width;
            int screenHeight = mBoundsRectangle.Height;
            mColours = new List<Avatar>();
            List<Color> colours = new List<Color>
            {
                Color.Red,
                Color.Lime,
                Color.DarkGreen,
                Color.MonoGameOrange,
                Color.Blue,
                Color.Yellow,
                Color.NavajoWhite,
                Color.DarkSlateGray,
                Color.SaddleBrown,
                Color.Indigo,
                Color.Magenta,
                Color.Aqua
            };

            var rectangles = GetSelectionRectangles(colours.Count);
            for (int i = 0; i < colours.Count; i++)
            {
                Avatar avatar = new Avatar(mWhitePixel, pAvatarString, rectangles[i], colours[i]);
                mColours.Add(avatar);
            }
        }

        public void DrawAvatars(SpriteBatch pSpriteBatch)
        {
            foreach (Avatar avatar in mAvatars)
            {
                avatar.Draw(pSpriteBatch, true, 0);
            }
        }
        public void DrawColours(SpriteBatch pSpriteBatch)
        {
            foreach (Avatar avatar in mColours)
            {
                avatar.Draw(pSpriteBatch, true, 0);
            }
        }
        public void DrawSelection(SpriteBatch pSpriteBatch)
        {
            pSpriteBatch.Draw(mCircle, mSelectionRectangles[mPlayer.SelectionIndex], Color.White);
        }

        public void DrawJoinButton(SpriteBatch pSpriteBatch)
        {
            if (mShowJoinButton)
            {
                pSpriteBatch.Draw(mJoinButtonTexture, mAButtonRectangle, Color.White);
            }
        }

        public void Draw(SpriteBatch pSpriteBatch)
        {
            if (HasPlayer())
            {
                mPlayer.Avatar.Draw(pSpriteBatch, true, 0);
                if (!mPlayer.AvatarSet)
                {
                    DrawSelection(pSpriteBatch);
                    DrawAvatars(pSpriteBatch);
                }
                else if (!mPlayer.ColourSet)
                {
                    DrawSelection(pSpriteBatch);
                    DrawColours(pSpriteBatch);
                }
                pSpriteBatch.Draw(mRotateLeftTexture, mRotateLeftButton, Color.White);
                pSpriteBatch.Draw(mRotateRightTexture, mRotateRightButton, Color.White);
                pSpriteBatch.Draw(mBackButtonTexture, mBackButtonRectangle, Color.White);
                pSpriteBatch.Draw(mBackTextTexture, mBackTextRectangle, Color.White);
            }
            else
            {
                DrawJoinButton(pSpriteBatch);
            }
        }

        public void Update(float pSeconds)
        {
            IGame game = Tankontroller.Instance();
            mCountDown -= pSeconds;
            if (HasPlayer())
            {
                mShowJoinButton = false;
                IController controller = mPlayer.Controller;
                controller.UpdateController();

                if (controller.IsPressed(Control.FIRE) && !controller.WasPressed(Control.FIRE))
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
                        PrepareColourSelection(name);
                        mSelectionRectangles = GetSelectionRectangles(mColours.Count);
                    }
                    else if (!mPlayer.ColourSet)
                    {
                        // add a colour to the player
                        mPlayer.SetColour();
                        mPlayer.Controller.SetColour(mPlayer.Colour);
                        mPlayer.SelectionIndex = 0;
                        PrepareCentreAvatar();
                        mPlayer.Avatar.Reposition(mAvatarRectangle);
                    }
                }
                if (controller.IsPressed(Control.RECHARGE) && !controller.WasPressed(Control.RECHARGE))
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
                            PrepareCentreAvatar();
                            mPlayer.Avatar.Reposition(mAvatarRectangle);
                        }
                        else if (mPlayer.AvatarSet)
                        {
                            mPlayer.RemoveAvatar();
                            Avatar avatar = mAvatars[mPlayer.SelectionIndex];
                            string name = avatar.GetName();

                            avatar = new Avatar(mWhitePixel, name, mAvatarRectangle, Color.White);
                            mPlayer.AddAvatar(avatar);
                            mSelectionRectangles = GetSelectionRectangles(mAvatars.Count);
                        }
                        else
                        {
                            mPlayer = null;
                            return;
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
            }
            else
            {
                mAButtonCountDown -= pSeconds;
                if (mAButtonCountDown <= 0)
                {
                    if (mShowJoinButton)
                    {
                        mAButtonCountDown = 0.25f;
                    }
                    else
                    {
                        mAButtonCountDown = 1f;
                    }
                    mShowJoinButton = !mShowJoinButton;
                }
            }
        }
    }
}
