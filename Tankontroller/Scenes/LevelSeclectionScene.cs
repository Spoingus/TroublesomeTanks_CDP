using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Tankontroller.Controller;
using Tankontroller.GUI;
using static Tankontroller.MapManager;

namespace Tankontroller.Scenes
{
    public class LevelSelectionScene : IScene
    {
        private static readonly Texture2D mBackgroundTexture = Tankontroller.Instance().CM().Load<Texture2D>("background_01");
        private Rectangle mBackgroundRectangle;
        private ButtonList mButtonList;
        private List<string> mMapFiles;
        private Tankontroller mGameInstance;
        private int mCurrentScrollPosition;
        private float secondsLeft;
        private StartScene mStartScene;

        public LevelSelectionScene(StartScene startScene)
        {
            mStartScene = startScene;
            mGameInstance = (Tankontroller)Tankontroller.Instance();
            spriteBatch = new SpriteBatch(mGameInstance.GDM().GraphicsDevice);
            int screenWidth = mGameInstance.GDM().GraphicsDevice.Viewport.Width;
            int screenHeight = mGameInstance.GDM().GraphicsDevice.Viewport.Height;

            mBackgroundRectangle = new Rectangle(0, 0, screenWidth, screenHeight);

            mButtonList = new ButtonList();

            string mapsDirectory = Path.Combine(Environment.CurrentDirectory, "Maps");
            if (!Directory.Exists(mapsDirectory))
            {
                Directory.CreateDirectory(mapsDirectory);
            }
            string[] filePaths = Directory.GetFiles(mapsDirectory, "*.json", SearchOption.AllDirectories);
            for (int i = 0; i < filePaths.Length; i++)
            {
                filePaths[i] = filePaths[i].Replace(mapsDirectory + "\\", "");
            }
            mMapFiles = new List<string>(filePaths);

            mCurrentScrollPosition = 0;
            secondsLeft = 0.1f;
        }

        private void SelectMap(string mapFile)
        {
            mStartScene.SetDefaultMapFile(mapFile);
            mGameInstance.SM().Transition(mStartScene, true);
        }

        public override void Update(float pSeconds)
        {
            Escape();
            mGameInstance.GetControllerManager().DetectControllers();

            foreach (IController controller in mGameInstance.GetControllerManager().GetControllers())
            {
                controller.UpdateController();
                secondsLeft -= pSeconds;

                if (controller.IsPressed(Control.LEFT_TRACK_FORWARDS) || controller.IsPressed(Control.TURRET_LEFT))
                {
                    if (secondsLeft <= 0.0f)
                    {
                        mCurrentScrollPosition = (mCurrentScrollPosition - 1 + mMapFiles.Count) % mMapFiles.Count;
                        secondsLeft = 0.5f;
                    }
                }
                if (controller.IsPressed(Control.LEFT_TRACK_BACKWARDS) || controller.IsPressed(Control.TURRET_RIGHT))
                {
                    if (secondsLeft <= 0.0f)
                    {
                        mCurrentScrollPosition = (mCurrentScrollPosition + 1) % mMapFiles.Count;
                        secondsLeft = 0.5f;
                    }
                }
                if (controller.IsPressed(Control.FIRE) || controller.IsPressed(Control.RECHARGE))
                {
                    if (secondsLeft <= 0.0f)
                    {
                        SelectMap(mMapFiles[mCurrentScrollPosition]);
                        secondsLeft = 0.1f;
                    }
                }
            }
        }

        public override void Draw(float pSeconds)
        {
            mGameInstance.GDM().GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.Draw(mBackgroundTexture, mBackgroundRectangle, Color.White);
            spriteBatch.End();

            int screenWidth = mGameInstance.GDM().GraphicsDevice.Viewport.Width;
            int screenHeight = mGameInstance.GDM().GraphicsDevice.Viewport.Height;
            int thumbnailWidth = 320;
            int thumbnailHeight = 180;
            int centreThumbWidth = 640;
            int centreThumbHeight = 360;
            int spacing = 20;

            // Calculate the indices of the previous, current, and next thumbnails
            int prevIndex = (mCurrentScrollPosition - 1 + mMapFiles.Count) % mMapFiles.Count;
            int nextIndex = (mCurrentScrollPosition + 1) % mMapFiles.Count;

            // Calculate positions and sizes for the thumbnails
            Rectangle prevRect = new Rectangle(
                (screenWidth / 2) - (thumbnailWidth / 2) - thumbnailWidth,
                (screenHeight / 2) - (thumbnailHeight / 2),
                thumbnailWidth,
                thumbnailHeight
            );

            Rectangle currentRect = new Rectangle(
                (screenWidth / 2) - (centreThumbWidth / 2),
                (screenHeight / 2) - (centreThumbHeight / 2),
                centreThumbWidth,
                centreThumbHeight
            );

            Rectangle nextRect = new Rectangle(
                (screenWidth / 2) - (thumbnailWidth / 2) + thumbnailWidth,
                (screenHeight / 2) - (thumbnailHeight / 2),
                thumbnailWidth,
                thumbnailHeight
            );

            // Draw the previous and next thumbnails first
            DrawThumbnail(mMapFiles[prevIndex], prevRect);
            DrawThumbnail(mMapFiles[nextIndex], nextRect);

            // Draw the current thumbnail last, in the center of the screen
            DrawThumbnail(mMapFiles[mCurrentScrollPosition], currentRect);
        }

        void DrawThumbnail(string pMapFile, Rectangle pRectangle)
        {
            string thumbnailFile = pMapFile.Replace(".json", "_thumbnail.png");
            if (File.Exists(thumbnailFile))
            {
                using (FileStream fileStream = new FileStream(thumbnailFile, FileMode.Open))
                {
                    spriteBatch.Begin();
                    Texture2D thumbnailTexture = Texture2D.FromStream(mGameInstance.GDM().GraphicsDevice, fileStream);
                    spriteBatch.Draw(thumbnailTexture, pRectangle, Color.White);
                    spriteBatch.End();
                }
            }
            else
            {
                MakeThumbnailTextureFromMapFile(pMapFile);
                DrawThumbnail(pMapFile, pRectangle);
            }
        }

        void MakeThumbnailTextureFromMapFile(string pMapFile)
        {
            string mapContent = File.ReadAllText("Maps\\" + pMapFile);
            MapData mapData = JsonSerializer.Deserialize<MapData>(mapContent);

            int thumbnailWidth = 640;
            int thumbnailHeight = 360;
            RenderTarget2D renderTarget = new RenderTarget2D(mGameInstance.GDM().GraphicsDevice, thumbnailWidth, thumbnailHeight);

            mGameInstance.GDM().GraphicsDevice.SetRenderTarget(renderTarget);
            mGameInstance.GDM().GraphicsDevice.Clear(Color.Transparent);

            int screenWidth = Tankontroller.Instance().GDM().GraphicsDevice.Viewport.Width;
            int screenHeight = Tankontroller.Instance().GDM().GraphicsDevice.Viewport.Height;
            Rectangle playArea = new Rectangle(screenWidth * 2 / 100, screenHeight * 2 / 100, screenWidth * 96 / 100, screenHeight * 96 / 100);

            spriteBatch.Begin();

            // Draw black outline
            Rectangle outlineRect = new Rectangle(0, 0, thumbnailWidth, thumbnailHeight);
            spriteBatch.Draw(mGameInstance.CM().Load<Texture2D>("block"), outlineRect, Color.Black);

            // Draw inner rectangle with map background color
            Rectangle innerRect = new Rectangle(2, 2, thumbnailWidth - 4, thumbnailHeight - 4);
            spriteBatch.Draw(mGameInstance.CM().Load<Texture2D>("block"), innerRect, DGS.Instance.GetColour("COLOUR_GROUND"));

            // Draw outlines for walls
            foreach (var wall in mapData.Walls)
            {
                Rectangle wallRect = new Rectangle(
                    (int)(playArea.X + (playArea.Width * (float.Parse(wall.Position[0]) / 100))),
                    (int)(playArea.Y + (playArea.Height * (float.Parse(wall.Position[1]) / 100))),
                    (int)(playArea.Width * (float.Parse(wall.Size[0]) / 100)),
                    (int)(playArea.Height * (float.Parse(wall.Size[1]) / 100))
                );
                DrawOutline(wallRect, wall.Texture);
            }

            // Draw outlines for tanks
            foreach (var tank in mapData.Tanks)
            {
                Rectangle tankRect = new Rectangle(
                    (int)(playArea.X + (playArea.Width * (float.Parse(tank.Position[0]) / 100))),
                    (int)(playArea.Y + (playArea.Height * (float.Parse(tank.Position[1]) / 100))),
                    10, 10
                );
                tankRect.X -= tankRect.Width / 2;
                tankRect.Y -= tankRect.Height / 2;
                DrawOutline(tankRect, "block");
            }

            // Draw outlines for pickups
            foreach (var pickup in mapData.Pickups)
            {
                Rectangle pickupRect = new Rectangle(
                    (int)(playArea.X + (playArea.Width * (float.Parse(pickup.Position[0]) / 100))),
                    (int)(playArea.Y + (playArea.Height * (float.Parse(pickup.Position[1]) / 100))),
                    10, 10
                );
                pickupRect.X -= pickupRect.Width / 2;
                pickupRect.Y -= pickupRect.Height / 2;
                DrawOutline(pickupRect, "circle");
            }

            spriteBatch.End();

            spriteBatch.Begin();

            // Draw walls
            foreach (var wall in mapData.Walls)
            {
                Rectangle wallRect = new Rectangle(
                    (int)(playArea.X + (playArea.Width * (float.Parse(wall.Position[0]) / 100))),
                    (int)(playArea.Y + (playArea.Height * (float.Parse(wall.Position[1]) / 100))),
                    (int)(playArea.Width * (float.Parse(wall.Size[0]) / 100)),
                    (int)(playArea.Height * (float.Parse(wall.Size[1]) / 100))
                );
                spriteBatch.Draw(mGameInstance.CM().Load<Texture2D>(wall.Texture), wallRect, DGS.Instance.GetColour("COLOUR_WALLS"));
            }

            // Draw tanks
            foreach (var tank in mapData.Tanks)
            {
                Rectangle tankRect = new Rectangle(
                    (int)(playArea.X + (playArea.Width * (float.Parse(tank.Position[0]) / 100))),
                    (int)(playArea.Y + (playArea.Height * (float.Parse(tank.Position[1]) / 100))),
                    9, 9
                );
                tankRect.X -= tankRect.Width / 2;
                tankRect.Y -= tankRect.Height / 2;
                spriteBatch.Draw(mGameInstance.CM().Load<Texture2D>("block"), tankRect, Color.Blue);
            }

            // Draw pickups
            foreach (var pickup in mapData.Pickups)
            {
                Rectangle pickupRect = new Rectangle(
                    (int)(playArea.X + (playArea.Width * (float.Parse(pickup.Position[0]) / 100))),
                    (int)(playArea.Y + (playArea.Height * (float.Parse(pickup.Position[1]) / 100))),
                    9, 9
                );
                pickupRect.X -= pickupRect.Width / 2;
                pickupRect.Y -= pickupRect.Height / 2;
                spriteBatch.Draw(mGameInstance.CM().Load<Texture2D>("circle"), pickupRect, Color.Red);
            }

            spriteBatch.End();

            mGameInstance.GDM().GraphicsDevice.SetRenderTarget(null);

            Texture2D thumbnailTexture = renderTarget;

            using (FileStream stream = new FileStream(pMapFile.Replace(".json", "_thumbnail.png"), FileMode.Create))
            {
                thumbnailTexture.SaveAsPng(stream, thumbnailWidth, thumbnailHeight);
            }
        }

        void DrawOutline(Rectangle rect, string textureName)
        {
            int offset = 2; // Outline thickness
            Texture2D texture = mGameInstance.CM().Load<Texture2D>(textureName);
            Color outlineColor = Color.Black;

            // Draw the outline by offsetting the position
            spriteBatch.Draw(texture, new Rectangle(rect.X - offset, rect.Y, rect.Width, rect.Height), outlineColor);
            spriteBatch.Draw(texture, new Rectangle(rect.X + offset, rect.Y, rect.Width, rect.Height), outlineColor);
            spriteBatch.Draw(texture, new Rectangle(rect.X, rect.Y - offset, rect.Width, rect.Height), outlineColor);
            spriteBatch.Draw(texture, new Rectangle(rect.X, rect.Y + offset, rect.Width, rect.Height), outlineColor);
        }

        public override void Escape()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                mGameInstance.SM().Transition(mStartScene, true);
            }
        }
    }
}
