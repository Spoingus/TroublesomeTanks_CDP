using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Tankontroller.Controller;
using Tankontroller.GUI;
using Tankontroller.World;
using Tankontroller.World.Particles;

namespace Tankontroller.Scenes
{
    //-------------------------------------------------------------------------------------------------
    // GameScene
    //
    // This class is used to display the game scene. The game scene displays the tanks, bullets, tracks,
    // and walls of the game. The class contains a list of controllers, a world, a sprite batch, and a
    // list of tank positions and rotations. The class provides methods to draw the game scene, update
    // the game scene, and transition to the game over scene.
    //-------------------------------------------------------------------------------------------------
    public class GameScene : IScene
    {
        IGame gameInstance = Tankontroller.Instance();
        Tankontroller tankControllerInstance = (Tankontroller)Tankontroller.Instance();
        private TheWorld m_World;
        private Texture2D m_BulletTexture;
        private Texture2D m_ErrorBGTexture;
        private SoundEffectInstance introMusicInstance = null;
        private SoundEffectInstance tankMoveSound = null;
        private SpriteFont m_SpriteFont;

        private List<Vector2> m_TankPositions = new List<Vector2>();
        private List<float> m_TankRotations = new List<float>();

        private const float SECONDS_BETWEEN_TRACKS_ADDED = 0.2f;
        private float m_SecondsTillTracksAdded = SECONDS_BETWEEN_TRACKS_ADDED;

        private List<Player> m_Teams;

        Texture2D mBackgroundTexture = null;
        Texture2D mPixelTexture = null;
        Rectangle mBackgroundRectangle;
        Rectangle mPlayAreaRectangle;
        Rectangle mPlayAreaOutlineRectangle;
        public Rectangle PlayArea { get { return mPlayAreaRectangle; } }

        private bool mControllersConnected = true;

        

        public GameScene(List<Player> pPlayers)
        {
            Tank.SetupStaticTextures(
                tankControllerInstance.CM().Load<Texture2D>("Tank-B-05"),
                tankControllerInstance.CM().Load<Texture2D>("BrokenTank"),
                tankControllerInstance.CM().Load<Texture2D>("Tank track B-R"),
                tankControllerInstance.CM().Load<Texture2D>("Tank track B-L"),
                tankControllerInstance.CM().Load<Texture2D>("cannon"),
                tankControllerInstance.CM().Load<Texture2D>("cannonFire"));
            m_BulletTexture = tankControllerInstance.CM().Load<Texture2D>("circle");
            mPixelTexture = tankControllerInstance.CM().Load<Texture2D>("block");
            m_ErrorBGTexture = tankControllerInstance.CM().Load<Texture2D>("background_err");
            TrackSystem.SetupStaticMembers(tankControllerInstance.CM().Load<Texture2D>("track"));
            TeamGUI.SetupStaticTextures(
                tankControllerInstance.CM().Load<Texture2D>("port1"),
                tankControllerInstance.CM().Load<Texture2D>("port2"),
                tankControllerInstance.CM().Load<Texture2D>("port3"),
                tankControllerInstance.CM().Load<Texture2D>("port4"),
                tankControllerInstance.CM().Load<Texture2D>("port5"),
                tankControllerInstance.CM().Load<Texture2D>("port6"),
                tankControllerInstance.CM().Load<Texture2D>("port7"),
                tankControllerInstance.CM().Load<Texture2D>("port8"));

            JackIcon.SetupStaticTextures(
                tankControllerInstance.CM().Load<Texture2D>("leftTrackForward"),
                tankControllerInstance.CM().Load<Texture2D>("leftTrackBackwards"),
                tankControllerInstance.CM().Load<Texture2D>("rightTrackForward"),
                tankControllerInstance.CM().Load<Texture2D>("rightTrackBackwards"),
                tankControllerInstance.CM().Load<Texture2D>("fire"),
                tankControllerInstance.CM().Load<Texture2D>("charge"),
                tankControllerInstance.CM().Load<Texture2D>("none"),
                tankControllerInstance.CM().Load<Texture2D>("turretLeft"),
                tankControllerInstance.CM().Load<Texture2D>("turretRight"));
            PowerBar.SetupStaticTextures(tankControllerInstance.CM().Load<Texture2D>("powerBar_border"),
                tankControllerInstance.CM().Load<Texture2D>("powerBar_power"));

            m_SpriteFont = tankControllerInstance.CM().Load<SpriteFont>("handwritingfont");

            spriteBatch = new SpriteBatch(tankControllerInstance.GDM().GraphicsDevice);

            mBackgroundTexture = tankControllerInstance.CM().Load<Texture2D>("background_01");
            mBackgroundRectangle = new Rectangle(0, 0, tankControllerInstance.GDM().GraphicsDevice.Viewport.Width, tankControllerInstance.GDM().GraphicsDevice.Viewport.Height);
            mPlayAreaRectangle = new Rectangle(tankControllerInstance.GDM().GraphicsDevice.Viewport.Width * 2 / 100, tankControllerInstance.GDM().GraphicsDevice.Viewport.Height * 25 / 100, tankControllerInstance.GDM().GraphicsDevice.Viewport.Width * 96 / 100, tankControllerInstance.GDM().GraphicsDevice.Viewport.Height * 73 / 100);
            mPlayAreaOutlineRectangle = new Rectangle(mPlayAreaRectangle.X - 5, mPlayAreaRectangle.Y - 5, mPlayAreaRectangle.Width + 10, mPlayAreaRectangle.Height + 10);
            introMusicInstance = tankControllerInstance.ReplaceCurrentMusicInstance("Music/Music_intro", false);


            m_Teams = pPlayers;

            int numberOfPlayers = m_Teams.Count;

            for (int i = 0; i < numberOfPlayers; i++)
            {
                IController controller = Tankontroller.Instance().GetController(i);
                controller.ResetJacks();
            }
            tankMoveSound = tankControllerInstance.GetSoundManager().GetLoopableSoundEffectInstance("Sounds/Tank_Tracks"); 

            if (numberOfPlayers < 4)
            {
                setupNot4Player(mPlayAreaRectangle, numberOfPlayers);
            }
            else
            {
                setup4Player(mPlayAreaRectangle);
            }
            foreach (Player p in m_Teams)
            {
                m_World.AddTank(p.Tank);
            }
        }

        //The game set up for 4 players
        private void setup4Player(Rectangle pPlayArea)
        {
            int middleBlockHeight = pPlayArea.Height / 3;
            int outerBlockHeight = pPlayArea.Height / 5;
            int blockThickness = pPlayArea.Width / 50;
            int outerBlockXOffset = pPlayArea.Width / 8;

            //Creates a list of walls for a 4 player map
            List<RectWall> Walls = new List<RectWall>();
            Walls.Add(new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + 3 * outerBlockXOffset - 2 * blockThickness, pPlayArea.Y + middleBlockHeight, blockThickness, middleBlockHeight)));
            Walls.Add(new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + pPlayArea.Width - 3 * outerBlockXOffset + blockThickness, pPlayArea.Y + middleBlockHeight, blockThickness, middleBlockHeight)));

            Walls.Add(new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + pPlayArea.Width - 3 * outerBlockXOffset + 2 * blockThickness, pPlayArea.Y + (pPlayArea.Height - blockThickness) / 2, (pPlayArea.X + pPlayArea.Width) - (pPlayArea.X + pPlayArea.Width - 3 * outerBlockXOffset + 2 * blockThickness), blockThickness)));
            Walls.Add(new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X, pPlayArea.Y + (pPlayArea.Height - blockThickness) / 2, (pPlayArea.X + pPlayArea.Width) - (pPlayArea.X + pPlayArea.Width - 3 * outerBlockXOffset + 2 * blockThickness), blockThickness)));

            Walls.Add(new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + (pPlayArea.Width - blockThickness) / 2, pPlayArea.Y, blockThickness, middleBlockHeight)));
            Walls.Add(new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + (pPlayArea.Width - blockThickness) / 2, pPlayArea.Y + pPlayArea.Height - middleBlockHeight, blockThickness, middleBlockHeight)));

            //Creates a list of tank positions and rotations for each of the 4 players
            List<Vector2> tankPositions = new List<Vector2>();

            float xPosition = pPlayArea.X + outerBlockXOffset / 2;
            float yPosition = pPlayArea.Y + outerBlockXOffset / 2;
            Vector2 tankPosition = new Vector2(xPosition, yPosition);
            m_TankPositions.Add(tankPosition);
            m_TankRotations.Add((float)(-Math.PI + Math.PI / 4));

            xPosition = pPlayArea.X + pPlayArea.Width - outerBlockXOffset / 2;
            yPosition = pPlayArea.Y + outerBlockXOffset / 2;
            tankPosition = new Vector2(xPosition, yPosition);
            m_TankPositions.Add(tankPosition);
            m_TankRotations.Add((float)-Math.PI / 4);

            xPosition = pPlayArea.X + outerBlockXOffset / 2;
            yPosition = pPlayArea.Y + pPlayArea.Height - outerBlockXOffset / 2;
            tankPosition = new Vector2(xPosition, yPosition);
            m_TankPositions.Add(tankPosition);
            m_TankRotations.Add((float)(Math.PI / 2 + Math.PI / 4));

            xPosition = pPlayArea.X + pPlayArea.Width - outerBlockXOffset / 2;
            yPosition = pPlayArea.Y + pPlayArea.Height - outerBlockXOffset / 2;
            tankPosition = new Vector2(xPosition, yPosition);
            m_TankPositions.Add(tankPosition);
            m_TankRotations.Add((float)Math.PI / 4);

            m_World = new TheWorld(pPlayArea, Walls);

            setupPlayers(pPlayArea);
        }

        //The game set up for 1 to 3 players
        private void setupNot4Player(Rectangle pPlayArea, int pNumOfPlayers)
        {
            int middleBlockHeight = pPlayArea.Height / 3;
            int outerBlockHeight = pPlayArea.Height / 5;
            int blockThickness = pPlayArea.Width / 50;
            int outerBlockXOffset = pPlayArea.Width / 8;

            //Creates a list of walls for 1 to 3 player map
            List<RectWall> Walls = new List<RectWall> //Simplified initialisation of the list
            {
                new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + outerBlockXOffset, pPlayArea.Y + outerBlockHeight, blockThickness, outerBlockHeight)),
                new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + outerBlockXOffset, pPlayArea.Y + outerBlockHeight, outerBlockHeight, blockThickness)),
                new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + outerBlockXOffset, pPlayArea.Y + 3 * outerBlockHeight, blockThickness, outerBlockHeight)),
                new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + outerBlockXOffset, pPlayArea.Y + 4 * outerBlockHeight - blockThickness, outerBlockHeight, blockThickness)),
                new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + pPlayArea.Width - outerBlockXOffset - blockThickness, pPlayArea.Y + outerBlockHeight, blockThickness, outerBlockHeight)),
                new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + pPlayArea.Width - outerBlockXOffset - outerBlockHeight, pPlayArea.Y + outerBlockHeight, outerBlockHeight, blockThickness)),
                new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + pPlayArea.Width - outerBlockXOffset - blockThickness, pPlayArea.Y + 3 * outerBlockHeight, blockThickness, outerBlockHeight)),
                new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + pPlayArea.Width - outerBlockXOffset - outerBlockHeight, pPlayArea.Y + 4 * outerBlockHeight - blockThickness, outerBlockHeight, blockThickness)),
                new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + 3 * outerBlockXOffset - 2 * blockThickness, pPlayArea.Y + middleBlockHeight, blockThickness, middleBlockHeight)),
                new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + pPlayArea.Width - 3 * outerBlockXOffset + blockThickness, pPlayArea.Y + middleBlockHeight, blockThickness, middleBlockHeight)),
                new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + pPlayArea.Width - 3 * outerBlockXOffset + 2 * blockThickness, pPlayArea.Y + (pPlayArea.Height - blockThickness) / 2, outerBlockHeight, blockThickness)),
                new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + 3 * outerBlockXOffset - outerBlockHeight - 2 * blockThickness, pPlayArea.Y + (pPlayArea.Height - blockThickness) / 2, outerBlockHeight, blockThickness)),
                new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + (pPlayArea.Width - blockThickness) / 2, pPlayArea.Y, blockThickness, middleBlockHeight)),
                new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + (pPlayArea.Width - blockThickness) / 2, pPlayArea.Y + pPlayArea.Height - middleBlockHeight, blockThickness, middleBlockHeight))
            };

            //Creates a list of tank positions and rotations for each of the players, ranging from 1 to 3 players
            if (pNumOfPlayers >= 1)
            {
                float xPosition = pPlayArea.X + outerBlockXOffset;
                float yPosition = pPlayArea.Y + pPlayArea.Height / 2;
                Vector2 tankPosition = new Vector2(xPosition, yPosition);
                m_TankPositions.Add(tankPosition);
                m_TankRotations.Add(0);
                if (pNumOfPlayers >= 2)
                {
                    xPosition = pPlayArea.X + pPlayArea.Width - outerBlockXOffset;
                    yPosition = pPlayArea.Y + pPlayArea.Height / 2;
                    tankPosition = new Vector2(xPosition, yPosition);
                    m_TankPositions.Add(tankPosition);
                    m_TankRotations.Add((float)Math.PI);
                    if (pNumOfPlayers >= 3)
                    {
                        xPosition = pPlayArea.Width / 2 + 35;
                        yPosition = pPlayArea.Y + pPlayArea.Height / 2;
                        tankPosition = new Vector2(xPosition, yPosition);
                        m_TankPositions.Add(tankPosition);
                        m_TankRotations.Add((float)Math.PI / 2);
                    }
                }
            }

            m_World = new TheWorld(pPlayArea, Walls);

            setupPlayers(pPlayArea);
        }


        private void setupPlayers(Rectangle pPlayArea)
        {
            float tankScale = (float)pPlayArea.Width / (50 * 40);
            int textureWidth = tankControllerInstance.GDM().GraphicsDevice.Viewport.Width / 4;
            int spacePerPlayer = tankControllerInstance.GDM().GraphicsDevice.Viewport.Width / m_Teams.Count;
            int textureHeight = tankControllerInstance.GDM().GraphicsDevice.Viewport.Height * 24 / 100;
            for (int i = 0; i < m_Teams.Count; i++)
            {
                m_Teams[i].GamePreparation(
                m_TankPositions[i].X, m_TankPositions[i].Y, m_TankRotations[i], tankScale,
                tankControllerInstance.CM().Load<Texture2D>("healthbars/heart_bw"),
                tankControllerInstance.CM().Load<Texture2D>("healthbars/heart_colour"),
                new Rectangle((int)(i * spacePerPlayer + (spacePerPlayer - textureWidth) * 0.5f), 0, textureWidth, textureHeight));
            }

        }

        public void Draw(float pSeconds)
        {
            Tankontroller.Instance().GDM().GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            spriteBatch.Draw(mBackgroundTexture, mBackgroundRectangle, Color.White);

            //Draws the GUI for each player
            foreach (Player player in m_Teams)
            {
                if (player.GUI != null)
                {
                    player.GUI.Draw(spriteBatch);
                }
            }

            spriteBatch.Draw(mPixelTexture, mPlayAreaOutlineRectangle, Color.Black);
            spriteBatch.Draw(mPixelTexture, mPlayAreaRectangle, DGS.Instance.GetColour("COLOUR_GROUND"));

            TrackSystem.GetInstance().Draw(spriteBatch);

            //Draw the background of the bullets
            foreach (Player p in m_Teams)
            {
                List<Bullet> bullets = p.Tank.GetBulletList();
                foreach (Bullet b in bullets)
                {
                    b.DrawBackground(spriteBatch, m_BulletTexture);
                }
            }

            World.Particles.ParticleManager.Instance().Draw(spriteBatch);

            //Draw the foreground of the bullets
            foreach (Player p in m_Teams)
            {
                List<Bullet> bullets = p.Tank.GetBulletList();
                foreach (Bullet b in bullets)
                {
                    b.DrawForeground(spriteBatch, m_BulletTexture);
                }
            }

            //Draws the tanks
            float tankScale = (float)mPlayAreaRectangle.Width / (50 * 40);
            for (int i = 0; i < m_Teams.Count(); i++)
            {
                Tank t = m_World.GetTank(i);
                t?.Draw(spriteBatch, tankScale);
            }

            //Draws the walls
            foreach (RectWall w in m_World.Walls)
            {
                w.DrawOutlines(spriteBatch);
            }

            foreach (RectWall w in m_World.Walls)
            {
                w.Draw(spriteBatch);
            }

            if (!mControllersConnected)
            {
                string message = "A controller has been disconnected.\r\nPlease reconnect it to continue.\r\nSearching for controller...";
                Vector2 centre = new Vector2(mPlayAreaRectangle.X + mPlayAreaRectangle.Width / 2, mPlayAreaRectangle.Y + mPlayAreaRectangle.Height / 2);
                Vector2 fontSize = m_SpriteFont.MeasureString(message);
                spriteBatch.Draw(m_ErrorBGTexture, PlayArea, Color.White);
                spriteBatch.DrawString(m_SpriteFont, message, new Vector2(centre.X - (fontSize.X / 2), centre.Y - (fontSize.Y / 2)), Color.Black);
            }

            spriteBatch.End();
        }

        public void Update(float pSeconds)
        {
            Escape();
            if (introMusicInstance.State == SoundState.Stopped)
            {
                tankControllerInstance.ReplaceCurrentMusicInstance("Music/Music_loopable", true);
            }

            if (mControllersConnected) // Game should pause in the event of controller disconnect
            {
                //Updates each controller to check for inputs
                foreach (Player p in m_Teams)
                {
                    p.Controller.UpdateController();
                    // Check if controller is disconnected
                    mControllersConnected = mControllersConnected && p.Controller.IsConnected();
                }

                bool tankMoved = false;
                foreach (Player p in m_Teams)
                {
                    tankMoved = tankMoved | p.DoTankControls(pSeconds);
                }

                //Checks for tank collisons between the play area and the walls
                m_World.Update(pSeconds);
                m_World.CollideAllTheThingsWithPlayArea(mPlayAreaRectangle);

                if (tankMoved)
                {
                    tankMoveSound.Play();
                }
                else
                {
                    tankMoveSound.Pause();
                }

                //If there is only on player remaining, the GameOverScene is transitioned to
                List<int> remainingTeamsList = remainingTeams();
                if (remainingTeamsList.Count <= 1)
                {
                    int winner = -1;
                    if (remainingTeamsList.Count == 1)
                    {
                        winner = remainingTeamsList[0];
                    }
                    Reset();
                    Tankontroller.Instance().SM().Transition(new GameOverScene(mBackgroundTexture, m_Teams, winner));

                }

                //Updates the track particles for each tank
                m_SecondsTillTracksAdded -= pSeconds;
                if (m_SecondsTillTracksAdded <= 0)
                {
                    m_SecondsTillTracksAdded += SECONDS_BETWEEN_TRACKS_ADDED;
                    TrackSystem trackSystem = TrackSystem.GetInstance();
                    foreach (Player p in m_Teams)
                    {
                        trackSystem.AddTrack(p.Tank.GetWorldPosition(), p.Tank.GetRotation(), p.Tank.Colour());
                    }
                }
            }
            else // At least one controller is disconnected
            {
                gameInstance.DetectControllers();

                mControllersConnected = true;
                foreach (Player p in m_Teams)
                {
                    if (!p.Controller.IsConnected())
                    {
                        // Check to see if there is a connected controller not yet associated to a player
                        foreach (IController controller in gameInstance.GetControllers())
                        {
                            if (!m_Teams.Any(player => player.Controller == controller))
                            {
                                if (controller != null) // I don't think this is neccessary but the game crashed when controller was null
                                {
                                    controller.TransferJackCharge(p.Controller);
                                    p.SetController(controller);
                                    break;
                                }
                            }
                        }
                    }
                    mControllersConnected = mControllersConnected && p.Controller.IsConnected();
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

        private void Reset()
        {
            foreach (Player p in m_Teams)
            {
                p.Reset();
            }
        }

        //Checks the health of all players and returns a list of tanks with more that 0 health
        private List<int> remainingTeams()
        {
            List<int> remaining = new List<int>();
            int index = 0;
            foreach (Player player in m_Teams)
            {
                if (player.Tank.Health() > 0)
                {
                    remaining.Add(index);
                }
                index++;
            }
            return remaining;
        }

        public SpriteBatch spriteBatch { get; set; }

    }
}