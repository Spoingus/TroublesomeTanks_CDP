using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tankontroller.Controller;
using Tankontroller.World;

namespace Tankontroller.GUI
{
    public class AvatarPicker
    {
        private Rectangle mBoundsRectangle;
        List<Avatar> mAvatars;
        List<Avatar> mColours;
        List<Rectangle> mSelectionRectangles;
        Texture2D mWhitePixel;
        Texture2D mCircle;
        Texture2D mAButtonTexture;
        Rectangle mAButtonRectangle;
        Vector2 mCentre;
        float mRadius;
        float mAvatarRadius;
        Player mPlayer;
        Rectangle mAvatarRectangle;
        float mCountDown;
        float mAButtonCountDown;
        bool mShowAButton;


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
            prepareSelectionRectangles();
            prepareAButton();
            mShowAButton = true;
        }

        private void prepareAButton()
        {
            Tankontroller game = (Tankontroller)Tankontroller.Instance();
            mAButtonTexture = game.CM().Load<Texture2D>("countdown/a_button");
            float widthRatio = (float)mAButtonTexture.Width / mAButtonTexture.Height;
            int height = 200;
            int width = (int)(height * widthRatio);
            mAButtonRectangle = new Rectangle(mBoundsRectangle.X + (mBoundsRectangle.Width - width) / 2, mBoundsRectangle.Y + (mBoundsRectangle.Height - height) / 2, width, height);
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
            TroublesomeTanks game = (TroublesomeTanks)TroublesomeTanks.Instance();
            Avatar avatar = new Avatar(mWhitePixel, pAvatarName, pRectangle, pColour);
            return avatar;
        }
        private void prepareAvatars()
        {
            TroublesomeTanks game = (TroublesomeTanks)TroublesomeTanks.Instance();
            int screenWidth = mBoundsRectangle.Width;
            int screenHeight = mBoundsRectangle.Height;
            mAvatars = new List<Avatar>();
            List<string> avatarStrings = new List<string>();
            avatarStrings.Add("engineer");
            avatarStrings.Add("robo");
            avatarStrings.Add("winterjack");
            avatarStrings.Add("yeti");
            avatarStrings.Add("engineer");
            avatarStrings.Add("robo");
            avatarStrings.Add("winterjack");
            avatarStrings.Add("yeti");
            avatarStrings.Add("engineer");
            avatarStrings.Add("robo");
            avatarStrings.Add("winterjack");
            avatarStrings.Add("yeti");
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
        private void prepareSelectionRectangles()
        {
            TroublesomeTanks game = (TroublesomeTanks)TroublesomeTanks.Instance();
            int screenWidth = mBoundsRectangle.Width;
            int screenHeight = mBoundsRectangle.Height;
            mSelectionRectangles = new List<Rectangle>();
            // need to add a bunch of different avatars here
            float anglePerAvatar = (float)(Math.PI * 2 / DGS.NUMBER_OF_AVATARS);

            float centreX = mCentre.X;
            float centreY = mCentre.Y;
            float topLeftAngle = (float)Math.PI * 5 / 4;
            float bottomRightAngle = (float)Math.PI / 4;
            for (int i = 0; i < DGS.NUMBER_OF_AVATARS; i++)
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
            TroublesomeTanks game = (TroublesomeTanks)TroublesomeTanks.Instance();
            int screenWidth = mBoundsRectangle.Width;
            int screenHeight = mBoundsRectangle.Height;
            mColours = new List<Avatar>();
            List<Color> colours = new List<Color>();
            Color colorCreator = new Color(143, 205, 157, 255);
            colours.Add(colorCreator);
            colorCreator = new Color(180, 226, 239, 255);
            colours.Add(colorCreator);
            colorCreator = new Color(248, 247, 139, 255);
            colours.Add(colorCreator);
            colorCreator = new Color(238, 167, 201, 255);
            colours.Add(colorCreator);
            colours.Add(Color.Firebrick);
            colours.Add(Color.Gainsboro);
            colours.Add(Color.Honeydew);
            colours.Add(Color.IndianRed);
            colours.Add(Color.Khaki);
            colours.Add(Color.Lavender);
            colours.Add(Color.Magenta);
            colours.Add(Color.NavajoWhite);
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
            }
            else
            {
                //draw a button
                DrawAButton(pSpriteBatch);
            }

        }
        public void Update(float pSeconds)
        {
            IGame game = TroublesomeTanks.Instance();
            mCountDown -= pSeconds;
            if (HasPlayer())
            {
                mShowAButton = false;
                IController controller = mPlayer.Controller;
                controller.UpdateController(pSeconds);

                if (!controller.IsPressed(Control.POWER3))
                {
                    if (controller.WasPressed(Control.POWER3)) // this is pressing the A
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
                if (!controller.IsPressed(Control.POWER2))
                {
                    if (controller.WasPressed(Control.POWER2)) // this is pressing the B
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
                            }
                            else
                            {
                                mPlayer = null;
                            }
                        }
                    }
                }
                if (controller.IsPressed(Control.TRACKS_LEFT))
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
                else if (controller.IsPressed(Control.TRACKS_RIGHT))
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
