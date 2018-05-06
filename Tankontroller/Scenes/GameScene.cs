using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tankontroller.World;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework.Input;
using Tankontroller.World.Particles;

namespace Tankontroller.Scenes
{
    public class GameScene : IScene
    {
        private IController mController0;
        private IController mController1;
        private IController mController2;
        private IController mController3;
        private TheWorld m_World;
        private Texture2D m_TankBaseTexture;
        private Texture2D m_TankBrokenTexture;
        private Texture2D m_TankRightTrackTexture;
        private Texture2D m_TankLeftTrackTexture;
        private Texture2D mPlayAreaTexture;
        private Texture2D m_CircleTexture;
        private Texture2D m_CannonTexture;
        private Texture2D m_CannonFireTexture;
        private Texture2D m_BulletTexture;
        private SpriteBatch m_SpriteBatch;
        private SoundEffectInstance introMusicInstance = null;
        private SoundEffectInstance loopMusicInstance = null;
        private SoundEffectInstance tankMoveSound = null;

        private const float SECONDS_BETWEEN_TRACKS_ADDED = 0.2f;
        private float m_SecondsTillTracksAdded = SECONDS_BETWEEN_TRACKS_ADDED;

        private List<Player> m_Teams = new List<Player>();

        Texture2D mBackgroundTexture = null;
        Texture2D mPixelTexture = null;
        Rectangle mBackgroundRectangle;
        Rectangle mPlayAreaRectangle;
        Rectangle mPlayAreaOutlineRectangle;
        public Rectangle PlayArea { get { return mPlayAreaRectangle; } }

        // private Effect m_Shader;
        // private RenderTarget2D m_ShaderRenderTarget; // might not need this
        // private Texture2D m_ShaderTexture; // might not need this

        public GameScene()
        {
            Tankontroller game = (Tankontroller)Tankontroller.Instance();
            m_TankBaseTexture = game.CM().Load<Texture2D>("Tank-B-05");
            m_TankBrokenTexture = game.CM().Load<Texture2D>("BrokenTank");
            m_TankRightTrackTexture = game.CM().Load<Texture2D>("Tank track B-R");
            m_TankLeftTrackTexture = game.CM().Load<Texture2D>("Tank track B-L");
            m_CannonTexture = game.CM().Load<Texture2D>("cannon");
            m_CannonFireTexture = game.CM().Load<Texture2D>("cannonFire");
            m_BulletTexture = game.CM().Load<Texture2D>("circle");
            mPlayAreaTexture = game.CM().Load<Texture2D>("playArea");
            mPixelTexture = game.CM().Load<Texture2D>("block");
            TrackSystem.SetupStaticMembers(game.CM().Load<Texture2D>("track"));
            TeamGUI.SetupStaticTextures(
                game.CM().Load<Texture2D>("port1"),
                game.CM().Load<Texture2D>("port2"),
                game.CM().Load<Texture2D>("port3"),
                game.CM().Load<Texture2D>("port4"),
                game.CM().Load<Texture2D>("port5"),
                game.CM().Load<Texture2D>("port6"),
                game.CM().Load<Texture2D>("port7"),
                game.CM().Load<Texture2D>("port8"));

            JackIcon.SetupStaticTextures(
                game.CM().Load<Texture2D>("leftTrackForward"),
                game.CM().Load<Texture2D>("leftTrackBackwards"),
                game.CM().Load<Texture2D>("rightTrackForward"),
                game.CM().Load<Texture2D>("rightTrackBackwards"),
                game.CM().Load<Texture2D>("fire"),
                game.CM().Load<Texture2D>("charge"),
                game.CM().Load<Texture2D>("none"),
                game.CM().Load<Texture2D>("turretLeft"),
                game.CM().Load<Texture2D>("turretRight"));
            PowerBar.SetupStaticTextures(game.CM().Load<Texture2D>("powerBar_border"),
                game.CM().Load<Texture2D>("powerBar_power"));

            m_CircleTexture = game.CM().Load<Texture2D>("circle");

            m_SpriteBatch = new SpriteBatch(game.GDM().GraphicsDevice);

            /*m_Shader = game.CM().Load<Effect>("shader");
            m_ShaderRenderTarget = new RenderTarget2D(game.GDM().GraphicsDevice,
                game.GDM().GraphicsDevice.PresentationParameters.BackBufferWidth,
                game.GDM().GraphicsDevice.PresentationParameters.BackBufferHeight);
            m_ShaderTexture = new Texture2D(game.GDM().GraphicsDevice,
                game.GDM().GraphicsDevice.PresentationParameters.BackBufferWidth,
                game.GDM().GraphicsDevice.PresentationParameters.BackBufferHeight, false, m_ShaderRenderTarget.Format);
                */
            mBackgroundTexture = game.CM().Load<Texture2D>("background_01");
            mBackgroundRectangle = new Rectangle(0, 0, game.GDM().GraphicsDevice.Viewport.Width, game.GDM().GraphicsDevice.Viewport.Height);
            mPlayAreaRectangle = new Rectangle(game.GDM().GraphicsDevice.Viewport.Width * 2 / 100, game.GDM().GraphicsDevice.Viewport.Height * 25 / 100, game.GDM().GraphicsDevice.Viewport.Width * 96 / 100, game.GDM().GraphicsDevice.Viewport.Height * 73 / 100);
            mPlayAreaOutlineRectangle = new Rectangle(mPlayAreaRectangle.X - 5, mPlayAreaRectangle.Y - 5, mPlayAreaRectangle.Width + 10, mPlayAreaRectangle.Height + 10);
            introMusicInstance = game.ReplaceCurrentMusicInstance("Music/Music_intro", false);

            mController0 = Tankontroller.Instance().Controller0();
            mController1 = Tankontroller.Instance().Controller1();
            mController2 = Tankontroller.Instance().Controller2();
            mController3 = Tankontroller.Instance().Controller3();
            mController0.ResetJacks();
            mController1.ResetJacks();
            if (DGS.NUM_PLAYERS > 2)
            {
                mController2.ResetJacks();
                if (DGS.NUM_PLAYERS > 3)
                {
                    mController3.ResetJacks();
                }
            }
            
            //loopMusicInstance = game.GetSoundManager().GetLoopableSoundEffectInstance("Music/Music_loopable");  // Put the name of your song here instead of "song_title"
            // game.ReplaceCurrentMusicInstance("Music/Music_loopable", true);
            tankMoveSound = game.GetSoundManager().GetLoopableSoundEffectInstance("Sounds/Tank_Tracks");  // Put the name of your song here instead of "song_title"

            if (DGS.NUM_PLAYERS < 4)
            {
                setupNot4Player(mPlayAreaRectangle);
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

        private void setup4Player(Rectangle pPlayArea)
        {
           
            Tankontroller game = (Tankontroller)Tankontroller.Instance();

            int middleBlockHeight = pPlayArea.Height / 3;
            int outerBlockHeight = pPlayArea.Height / 5;
            int blockThickness = pPlayArea.Width / 50;
            int outerBlockXOffset = pPlayArea.Width / 8;

            List<RectWall> Walls = new List<RectWall>();

     //       Walls.Add(new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
       //         new Rectangle(pPlayArea.X + outerBlockXOffset, pPlayArea.Y + outerBlockHeight, blockThickness, outerBlockHeight)));
        //    Walls.Add(new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
         //       new Rectangle(pPlayArea.X + outerBlockXOffset, pPlayArea.Y + outerBlockHeight, outerBlockHeight, blockThickness)));

      //      Walls.Add(new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
        //        new Rectangle(pPlayArea.X + outerBlockXOffset, pPlayArea.Y + 3 * outerBlockHeight, blockThickness, outerBlockHeight)));
         //   Walls.Add(new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
          //      new Rectangle(pPlayArea.X + outerBlockXOffset, pPlayArea.Y + 4 * outerBlockHeight - blockThickness, outerBlockHeight, blockThickness)));

         //   Walls.Add(new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
          //      new Rectangle(pPlayArea.X + pPlayArea.Width - outerBlockXOffset - blockThickness, pPlayArea.Y + outerBlockHeight, blockThickness, outerBlockHeight)));
           // Walls.Add(new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
            //    new Rectangle(pPlayArea.X + pPlayArea.Width - outerBlockXOffset - outerBlockHeight, pPlayArea.Y + outerBlockHeight, outerBlockHeight, blockThickness)));


      //      Walls.Add(new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
       //         new Rectangle(pPlayArea.X + pPlayArea.Width - outerBlockXOffset - blockThickness, pPlayArea.Y + 3 * outerBlockHeight, blockThickness, outerBlockHeight)));
      //      Walls.Add(new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
      //          new Rectangle(pPlayArea.X + pPlayArea.Width - outerBlockXOffset - outerBlockHeight, pPlayArea.Y + 4 * outerBlockHeight - blockThickness, outerBlockHeight, blockThickness)));


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

            List<Vector2> tankPositions = new List<Vector2>();
            List<float> tankRotations = new List<float>();
            if (DGS.NUM_PLAYERS == 2)
            {
                float xPosition = pPlayArea.X + outerBlockXOffset;
                float yPosition = pPlayArea.Y + pPlayArea.Height / 2;
                Vector2 tankPosition = new Vector2(xPosition, yPosition);
                tankPositions.Add(tankPosition);
                tankRotations.Add(0);

                xPosition = pPlayArea.X + pPlayArea.Width - outerBlockXOffset;
                yPosition = pPlayArea.Y + pPlayArea.Height / 2;
                tankPosition = new Vector2(xPosition, yPosition);
                tankPositions.Add(tankPosition);
                tankRotations.Add((float)Math.PI);
            }
            else if (DGS.NUM_PLAYERS == 3)
            {
                float xPosition = pPlayArea.X + outerBlockXOffset;
                float yPosition = pPlayArea.Y + pPlayArea.Height / 2;
                Vector2 tankPosition = new Vector2(xPosition, yPosition);
                tankPositions.Add(tankPosition);
                tankRotations.Add(0);

                xPosition = pPlayArea.X + pPlayArea.Width - outerBlockXOffset;
                yPosition = pPlayArea.Y + pPlayArea.Height / 2;
                tankPosition = new Vector2(xPosition, yPosition);
                tankPositions.Add(tankPosition);
                tankRotations.Add((float)Math.PI);

                xPosition = pPlayArea.Width / 2 + 35;
                yPosition = pPlayArea.Y + pPlayArea.Height / 2;
                tankPosition = new Vector2(xPosition, yPosition);
                tankPositions.Add(tankPosition);
                tankRotations.Add((float)Math.PI/2);
            }
            else if (DGS.NUM_PLAYERS == 4)
            {
                float xPosition = pPlayArea.X + outerBlockXOffset / 2;
                float yPosition = pPlayArea.Y + outerBlockXOffset / 2;
                Vector2 tankPosition = new Vector2(xPosition, yPosition);
                tankPositions.Add(tankPosition);
                tankRotations.Add((float)(-Math.PI + Math.PI / 4));

                xPosition = pPlayArea.X + pPlayArea.Width - outerBlockXOffset / 2;
                yPosition = pPlayArea.Y + outerBlockXOffset / 2;
                tankPosition = new Vector2(xPosition, yPosition);
                tankPositions.Add(tankPosition);
                tankRotations.Add((float)-Math.PI/4);

                xPosition = pPlayArea.X + outerBlockXOffset / 2;
                yPosition = pPlayArea.Y + pPlayArea.Height - outerBlockXOffset / 2;
                tankPosition = new Vector2(xPosition, yPosition);
                tankPositions.Add(tankPosition);
                tankRotations.Add((float)(Math.PI / 2 + Math.PI/4));

                xPosition = pPlayArea.X + pPlayArea.Width - outerBlockXOffset / 2;
                yPosition = pPlayArea.Y + pPlayArea.Height - outerBlockXOffset / 2;
                tankPosition = new Vector2(xPosition, yPosition);
                tankPositions.Add(tankPosition);
                tankRotations.Add((float)Math.PI / 4);
            }
            else if (DGS.NUM_PLAYERS == 5)
            {
                float xPosition = pPlayArea.X + pPlayArea.Width / 2 + 0 - outerBlockXOffset / 2;
                float yPosition = pPlayArea.Y + pPlayArea.Height / 2 - outerBlockXOffset / 2;
                Vector2 tankPosition = new Vector2(xPosition, yPosition);
                tankPositions.Add(tankPosition);
                tankRotations.Add((float)(-Math.PI + Math.PI / 4));

                xPosition = pPlayArea.X + pPlayArea.Width / 2 + 0 + outerBlockXOffset / 2;
                yPosition = pPlayArea.Y + pPlayArea.Height / 2 - outerBlockXOffset / 2;
                tankPosition = new Vector2(xPosition, yPosition);
                tankPositions.Add(tankPosition);
                tankRotations.Add((float)-Math.PI / 4);

                xPosition = pPlayArea.X + pPlayArea.Width / 2 + 0 - outerBlockXOffset / 2;
                yPosition = pPlayArea.Y + pPlayArea.Height / 2 + outerBlockXOffset / 2;
                tankPosition = new Vector2(xPosition, yPosition);
                tankPositions.Add(tankPosition);
                tankRotations.Add((float)(Math.PI / 2 + Math.PI / 4));

                xPosition = pPlayArea.X + pPlayArea.Width / 2 + 0 + outerBlockXOffset /2;
                yPosition = pPlayArea.Y + pPlayArea.Height / 2 + outerBlockXOffset / 2;
                tankPosition = new Vector2(xPosition, yPosition);
                tankPositions.Add(tankPosition);
                tankRotations.Add((float)Math.PI / 4);
            }

            float tankXPosition = pPlayArea.X + outerBlockXOffset;
            float tankYPosition = pPlayArea.Y + pPlayArea.Height / 2;
            float tankRotation = 0;
            float tankScale = (float)pPlayArea.Width / (50 * 40);
            float textureHeightOverWidth = (float)254 / (float)540;
            int textureWidth = game.GDM().GraphicsDevice.Viewport.Width / 4;
            int textureHeight = (int)(textureWidth * textureHeightOverWidth);

            m_Teams.Add(new Player(
                DGS.COLOUR_TANK1, Tankontroller.Instance().Controller0(),
                tankPositions[0].X, tankPositions[0].Y, tankRotations[0], tankScale,
                game.CM().Load<Texture2D>("healthbar_winterjack_06"),
                game.CM().Load<Texture2D>("healthbar_winterjack_05"),
                game.CM().Load<Texture2D>("healthbar_winterjack_04"),
                game.CM().Load<Texture2D>("healthbar_winterjack_03"),
                game.CM().Load<Texture2D>("healthbar_winterjack_02"),
                game.CM().Load<Texture2D>("healthbar_winterjack_01"),
                new Rectangle(0, 0, textureWidth, textureHeight), true));

            tankXPosition = pPlayArea.X + pPlayArea.Width - outerBlockXOffset;
            tankRotation = (float)Math.PI;

            m_Teams.Add(new Player(
                DGS.COLOUR_TANK2, Tankontroller.Instance().Controller1(),
                tankPositions[1].X, tankPositions[1].Y, tankRotations[1], tankScale,
                game.CM().Load<Texture2D>("healthbar_engineer_06"),
                game.CM().Load<Texture2D>("healthbar_engineer_05"),
                game.CM().Load<Texture2D>("healthbar_engineer_04"),
                game.CM().Load<Texture2D>("healthbar_engineer_03"),
                game.CM().Load<Texture2D>("healthbar_engineer_02"),
                game.CM().Load<Texture2D>("healthbar_engineer_01"),
                new Rectangle(game.GDM().GraphicsDevice.Viewport.Width - textureWidth, 0, textureWidth, textureHeight), false));
            if (DGS.NUM_PLAYERS > 2)
            {
                tankRotation = (float)Math.PI;
                if (DGS.NUM_PLAYERS <= 3)
                {
                    tankXPosition = pPlayArea.Width / 2 + 35;
                    tankRotation = (float)Math.PI / 2;
                }
                
                
                m_Teams.Add(new Player(
                DGS.COLOUR_TANK3, Tankontroller.Instance().Controller2(),
                tankPositions[2].X, tankPositions[2].Y, tankRotations[2], tankScale,
                game.CM().Load<Texture2D>("healthbar_robo_06"),
                game.CM().Load<Texture2D>("healthbar_robo_05"),
                game.CM().Load<Texture2D>("healthbar_robo_04"),
                game.CM().Load<Texture2D>("healthbar_robo_03"),
                game.CM().Load<Texture2D>("healthbar_robo_02"),
                game.CM().Load<Texture2D>("healthbar_robo_01"),
                new Rectangle(0 + textureWidth, 0, textureWidth, textureHeight), true));

                tankXPosition = pPlayArea.X + outerBlockXOffset;
                tankRotation = 0;
                if (DGS.NUM_PLAYERS > 3)
                {
                    tankYPosition = pPlayArea.Y + pPlayArea.Height / 2 + 50;
                    
                    m_Teams.Add(new Player(
        DGS.COLOUR_TANK4, Tankontroller.Instance().Controller3(),
        tankPositions[3].X, tankPositions[3].Y, tankRotations[3], tankScale,
        game.CM().Load<Texture2D>("healthbar_yeti_06"),
        game.CM().Load<Texture2D>("healthbar_yeti_05"),
        game.CM().Load<Texture2D>("healthbar_yeti_04"),
        game.CM().Load<Texture2D>("healthbar_yeti_03"),
        game.CM().Load<Texture2D>("healthbar_yeti_02"),
        game.CM().Load<Texture2D>("healthbar_yeti_01"),
        new Rectangle(game.GDM().GraphicsDevice.Viewport.Width - textureWidth * 2, 0, textureWidth, textureHeight), false));
                }
            }
            

            m_World = new TheWorld(pPlayArea, Walls);
        }

        private void setupNot4Player(Rectangle pPlayArea)
        {

            Tankontroller game = (Tankontroller)Tankontroller.Instance();

            int middleBlockHeight = pPlayArea.Height / 3;
            int outerBlockHeight = pPlayArea.Height / 5;
            int blockThickness = pPlayArea.Width / 50;
            int outerBlockXOffset = pPlayArea.Width / 8;

            List<RectWall> Walls = new List<RectWall>();

            Walls.Add(new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + outerBlockXOffset, pPlayArea.Y + outerBlockHeight, blockThickness, outerBlockHeight)));
            Walls.Add(new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + outerBlockXOffset, pPlayArea.Y + outerBlockHeight, outerBlockHeight, blockThickness)));

            Walls.Add(new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + outerBlockXOffset, pPlayArea.Y + 3 * outerBlockHeight, blockThickness, outerBlockHeight)));
            Walls.Add(new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + outerBlockXOffset, pPlayArea.Y + 4 * outerBlockHeight - blockThickness, outerBlockHeight, blockThickness)));

            Walls.Add(new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + pPlayArea.Width - outerBlockXOffset - blockThickness, pPlayArea.Y + outerBlockHeight, blockThickness, outerBlockHeight)));
            Walls.Add(new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + pPlayArea.Width - outerBlockXOffset - outerBlockHeight, pPlayArea.Y + outerBlockHeight, outerBlockHeight, blockThickness)));


            Walls.Add(new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + pPlayArea.Width - outerBlockXOffset - blockThickness, pPlayArea.Y + 3 * outerBlockHeight, blockThickness, outerBlockHeight)));
            Walls.Add(new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + pPlayArea.Width - outerBlockXOffset - outerBlockHeight, pPlayArea.Y + 4 * outerBlockHeight - blockThickness, outerBlockHeight, blockThickness)));


            Walls.Add(new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + 3 * outerBlockXOffset - 2 * blockThickness, pPlayArea.Y + middleBlockHeight, blockThickness, middleBlockHeight)));
            Walls.Add(new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + pPlayArea.Width - 3 * outerBlockXOffset + blockThickness, pPlayArea.Y + middleBlockHeight, blockThickness, middleBlockHeight)));

            Walls.Add(new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + pPlayArea.Width - 3 * outerBlockXOffset + 2 * blockThickness, pPlayArea.Y + (pPlayArea.Height - blockThickness) / 2, outerBlockHeight, blockThickness)));
            Walls.Add(new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + 3 * outerBlockXOffset - outerBlockHeight - 2 * blockThickness, pPlayArea.Y + (pPlayArea.Height - blockThickness) / 2, outerBlockHeight, blockThickness)));

            Walls.Add(new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + (pPlayArea.Width - blockThickness) / 2, pPlayArea.Y, blockThickness, middleBlockHeight)));
            Walls.Add(new RectWall(Tankontroller.Instance().CM().Load<Texture2D>("block"),
                new Rectangle(pPlayArea.X + (pPlayArea.Width - blockThickness) / 2, pPlayArea.Y + pPlayArea.Height - middleBlockHeight, blockThickness, middleBlockHeight)));

            List<Vector2> tankPositions = new List<Vector2>();
            List<float> tankRotations = new List<float>();
            if (DGS.NUM_PLAYERS == 2)
            {
                float xPosition = pPlayArea.X + outerBlockXOffset;
                float yPosition = pPlayArea.Y + pPlayArea.Height / 2;
                Vector2 tankPosition = new Vector2(xPosition, yPosition);
                tankPositions.Add(tankPosition);
                tankRotations.Add(0);

                xPosition = pPlayArea.X + pPlayArea.Width - outerBlockXOffset;
                yPosition = pPlayArea.Y + pPlayArea.Height / 2;
                tankPosition = new Vector2(xPosition, yPosition);
                tankPositions.Add(tankPosition);
                tankRotations.Add((float)Math.PI);
            }
            else if (DGS.NUM_PLAYERS == 3)
            {
                float xPosition = pPlayArea.X + outerBlockXOffset;
                float yPosition = pPlayArea.Y + pPlayArea.Height / 2;
                Vector2 tankPosition = new Vector2(xPosition, yPosition);
                tankPositions.Add(tankPosition);
                tankRotations.Add(0);

                xPosition = pPlayArea.X + pPlayArea.Width - outerBlockXOffset;
                yPosition = pPlayArea.Y + pPlayArea.Height / 2;
                tankPosition = new Vector2(xPosition, yPosition);
                tankPositions.Add(tankPosition);
                tankRotations.Add((float)Math.PI);

                xPosition = pPlayArea.Width / 2 + 35;
                yPosition = pPlayArea.Y + pPlayArea.Height / 2;
                tankPosition = new Vector2(xPosition, yPosition);
                tankPositions.Add(tankPosition);
                tankRotations.Add((float)Math.PI / 2);
            }
            else if (DGS.NUM_PLAYERS == 4)
            {
                float xPosition = pPlayArea.X + outerBlockXOffset / 2;
                float yPosition = pPlayArea.Y + outerBlockXOffset / 2;
                Vector2 tankPosition = new Vector2(xPosition, yPosition);
                tankPositions.Add(tankPosition);
                tankRotations.Add((float)(-Math.PI + Math.PI / 4));

                xPosition = pPlayArea.X + pPlayArea.Width - outerBlockXOffset / 2;
                yPosition = pPlayArea.Y + outerBlockXOffset / 2;
                tankPosition = new Vector2(xPosition, yPosition);
                tankPositions.Add(tankPosition);
                tankRotations.Add((float)-Math.PI / 4);

                xPosition = pPlayArea.X + outerBlockXOffset / 2;
                yPosition = pPlayArea.Y + pPlayArea.Height - outerBlockXOffset / 2;
                tankPosition = new Vector2(xPosition, yPosition);
                tankPositions.Add(tankPosition);
                tankRotations.Add((float)(Math.PI / 2 + Math.PI / 4));

                xPosition = pPlayArea.X + pPlayArea.Width - outerBlockXOffset / 2;
                yPosition = pPlayArea.Y + pPlayArea.Height - outerBlockXOffset / 2;
                tankPosition = new Vector2(xPosition, yPosition);
                tankPositions.Add(tankPosition);
                tankRotations.Add((float)Math.PI / 4);
            }
            else if (DGS.NUM_PLAYERS == 5)
            {
                float xPosition = pPlayArea.X + pPlayArea.Width / 2 + 0 - outerBlockXOffset / 2;
                float yPosition = pPlayArea.Y + pPlayArea.Height / 2 - outerBlockXOffset / 2;
                Vector2 tankPosition = new Vector2(xPosition, yPosition);
                tankPositions.Add(tankPosition);
                tankRotations.Add((float)(-Math.PI + Math.PI / 4));

                xPosition = pPlayArea.X + pPlayArea.Width / 2 + 0 + outerBlockXOffset / 2;
                yPosition = pPlayArea.Y + pPlayArea.Height / 2 - outerBlockXOffset / 2;
                tankPosition = new Vector2(xPosition, yPosition);
                tankPositions.Add(tankPosition);
                tankRotations.Add((float)-Math.PI / 4);

                xPosition = pPlayArea.X + pPlayArea.Width / 2 + 0 - outerBlockXOffset / 2;
                yPosition = pPlayArea.Y + pPlayArea.Height / 2 + outerBlockXOffset / 2;
                tankPosition = new Vector2(xPosition, yPosition);
                tankPositions.Add(tankPosition);
                tankRotations.Add((float)(Math.PI / 2 + Math.PI / 4));

                xPosition = pPlayArea.X + pPlayArea.Width / 2 + 0 + outerBlockXOffset / 2;
                yPosition = pPlayArea.Y + pPlayArea.Height / 2 + outerBlockXOffset / 2;
                tankPosition = new Vector2(xPosition, yPosition);
                tankPositions.Add(tankPosition);
                tankRotations.Add((float)Math.PI / 4);
            }

            float tankXPosition = pPlayArea.X + outerBlockXOffset;
            float tankYPosition = pPlayArea.Y + pPlayArea.Height / 2;
            float tankRotation = 0;
            float tankScale = (float)pPlayArea.Width / (50 * 40);
            float textureHeightOverWidth = (float)254 / (float)540;
            int textureWidth = game.GDM().GraphicsDevice.Viewport.Width / 4;
            int textureHeight = (int)(textureWidth * textureHeightOverWidth);

            m_Teams.Add(new Player(
                DGS.COLOUR_TANK1, Tankontroller.Instance().Controller0(),
                tankPositions[0].X, tankPositions[0].Y, tankRotations[0], tankScale,
                game.CM().Load<Texture2D>("healthbar_winterjack_06"),
                game.CM().Load<Texture2D>("healthbar_winterjack_05"),
                game.CM().Load<Texture2D>("healthbar_winterjack_04"),
                game.CM().Load<Texture2D>("healthbar_winterjack_03"),
                game.CM().Load<Texture2D>("healthbar_winterjack_02"),
                game.CM().Load<Texture2D>("healthbar_winterjack_01"),
                new Rectangle(0, 0, textureWidth, textureHeight), true));

            tankXPosition = pPlayArea.X + pPlayArea.Width - outerBlockXOffset;
            tankRotation = (float)Math.PI;

            m_Teams.Add(new Player(
                DGS.COLOUR_TANK2, Tankontroller.Instance().Controller1(),
                tankPositions[1].X, tankPositions[1].Y, tankRotations[1], tankScale,
                game.CM().Load<Texture2D>("healthbar_engineer_06"),
                game.CM().Load<Texture2D>("healthbar_engineer_05"),
                game.CM().Load<Texture2D>("healthbar_engineer_04"),
                game.CM().Load<Texture2D>("healthbar_engineer_03"),
                game.CM().Load<Texture2D>("healthbar_engineer_02"),
                game.CM().Load<Texture2D>("healthbar_engineer_01"),
                new Rectangle(game.GDM().GraphicsDevice.Viewport.Width - textureWidth, 0, textureWidth, textureHeight), false));
            if (DGS.NUM_PLAYERS > 2)
            {
                tankRotation = (float)Math.PI;
                if (DGS.NUM_PLAYERS <= 3)
                {
                    tankXPosition = pPlayArea.Width / 2 + 35;
                    tankRotation = (float)Math.PI / 2;
                }


                m_Teams.Add(new Player(
                DGS.COLOUR_TANK3, Tankontroller.Instance().Controller2(),
                tankPositions[2].X, tankPositions[2].Y, tankRotations[2], tankScale,
                game.CM().Load<Texture2D>("healthbar_robo_06"),
                game.CM().Load<Texture2D>("healthbar_robo_05"),
                game.CM().Load<Texture2D>("healthbar_robo_04"),
                game.CM().Load<Texture2D>("healthbar_robo_03"),
                game.CM().Load<Texture2D>("healthbar_robo_02"),
                game.CM().Load<Texture2D>("healthbar_robo_01"),
                new Rectangle(0 + textureWidth, 0, textureWidth, textureHeight), true));

                tankXPosition = pPlayArea.X + outerBlockXOffset;
                tankRotation = 0;
                if (DGS.NUM_PLAYERS > 3)
                {
                    tankYPosition = pPlayArea.Y + pPlayArea.Height / 2 + 50;

                    m_Teams.Add(new Player(
        DGS.COLOUR_TANK4, Tankontroller.Instance().Controller3(),
        tankPositions[3].X, tankPositions[3].Y, tankRotations[3], tankScale,
        game.CM().Load<Texture2D>("healthbar_yeti_06"),
        game.CM().Load<Texture2D>("healthbar_yeti_05"),
        game.CM().Load<Texture2D>("healthbar_yeti_04"),
        game.CM().Load<Texture2D>("healthbar_yeti_03"),
        game.CM().Load<Texture2D>("healthbar_yeti_02"),
        game.CM().Load<Texture2D>("healthbar_yeti_01"),
        new Rectangle(game.GDM().GraphicsDevice.Viewport.Width - textureWidth * 2, 0, textureWidth, textureHeight), false));
                }
            }


            m_World = new TheWorld(pPlayArea, Walls);
        }

        private void IntroFinished()
        {
            if (introMusicInstance.State == SoundState.Stopped)
            {
                Tankontroller game = (Tankontroller)Tankontroller.Instance();
                game.ReplaceCurrentMusicInstance("Music/Music_loopable", true);
            }
        }


        public void Draw(float pSeconds)
        {
            Tankontroller.Instance().GDM().GraphicsDevice.Clear(Color.CornflowerBlue);

            m_SpriteBatch.Begin();

            m_SpriteBatch.Draw(mBackgroundTexture, mBackgroundRectangle, Color.White);

            foreach (Player player in m_Teams)
            {
                if (player.GUI != null)
                {
                    player.GUI.Draw(m_SpriteBatch);
                }
            }

            m_SpriteBatch.Draw(mPixelTexture, mPlayAreaOutlineRectangle, Color.Black);
            m_SpriteBatch.Draw(mPixelTexture, mPlayAreaRectangle, DGS.COLOUR_GROUND);

            TrackSystem.GetInstance().Draw(m_SpriteBatch);

            // bullet background
            int bulletRadius = 10;
            int radius = bulletRadius + 2 * DGS.PARTICLE_EDGE_THICKNESS;
            Rectangle bulletRect = new Rectangle(0, 0, radius, radius);
            foreach (Player p in m_Teams)
            {
                List<Bullet> bullets = p.Tank.GetBulletList();
                foreach (Bullet b in bullets)
                {
                    bulletRect.X = (int)b.Position.X - radius / 2;
                    bulletRect.Y = (int)b.Position.Y - radius / 2;
                    m_SpriteBatch.Draw(m_BulletTexture, bulletRect, Color.Black);
                }
            }

            World.Particles.ParticleManager.Instance().Draw(m_SpriteBatch);

            // bullet colour
            bulletRect.Width = bulletRadius;
            bulletRect.Height = bulletRadius;
            foreach (Player p in m_Teams)
            {
                List<Bullet> bullets = p.Tank.GetBulletList();
                foreach (Bullet b in bullets)
                {
                    bulletRect.X = (int)b.Position.X - bulletRadius / 2;
                    bulletRect.Y = (int)b.Position.Y - bulletRadius / 2;
                    m_SpriteBatch.Draw(m_BulletTexture, bulletRect, b.Colour);
                }
            }

            Rectangle trackRect = new Rectangle(0, 0, m_TankLeftTrackTexture.Width, m_TankLeftTrackTexture.Height / 15);

            float tankScale = (float)mPlayAreaRectangle.Width / (50 * 40);

            for (int i = 0; true; i++)
            {
                Tank t = m_World.GetTank(i);
                if (t == null)
                {
                    break;
                }
                if (t.Health() > 0)
                {
                    trackRect.Y = t.LeftTrackFrame() * m_TankLeftTrackTexture.Height / 15;
                    m_SpriteBatch.Draw(m_TankLeftTrackTexture, t.GetWorldPosition(), trackRect, t.Colour(), t.GetRotation(), new Vector2(m_TankBaseTexture.Width / 2, m_TankBaseTexture.Height / 2), tankScale, SpriteEffects.None, 0.0f);
                    trackRect.Y = t.RightTrackFrame() * m_TankLeftTrackTexture.Height / 15;
                    m_SpriteBatch.Draw(m_TankRightTrackTexture, t.GetWorldPosition(), trackRect, t.Colour(), t.GetRotation(), new Vector2(m_TankBaseTexture.Width / 2, m_TankBaseTexture.Height / 2), tankScale, SpriteEffects.None, 0.0f);
                    m_SpriteBatch.Draw(m_TankBaseTexture, t.GetWorldPosition(), null, t.Colour(), t.GetRotation(), new Vector2(m_TankBaseTexture.Width / 2, m_TankBaseTexture.Height / 2), tankScale, SpriteEffects.None, 0.0f);
                    if (t.Fired() == 0)
                    {
                        m_SpriteBatch.Draw(m_CannonTexture, t.GetCannonWorldPosition(), null, t.Colour(), t.GetCannonWorldRotation(), new Vector2(m_CannonTexture.Width / 2, m_CannonTexture.Height / 2), tankScale, SpriteEffects.None, 0.0f);
                    }
                    else
                    {
                        m_SpriteBatch.Draw(m_CannonFireTexture, t.GetCannonWorldPosition(), null, t.Colour(), t.GetCannonWorldRotation(), new Vector2(m_CannonTexture.Width / 2, m_CannonTexture.Height / 2), tankScale, SpriteEffects.None, 0.0f);
                    }
                }
                else
                {
                    m_SpriteBatch.Draw(m_TankBrokenTexture, t.GetWorldPosition(), null, t.Colour(), t.GetRotation(), new Vector2(m_TankBrokenTexture.Width / 2, m_TankBrokenTexture.Height / 2), tankScale, SpriteEffects.None, 0.0f);
                }
            }

     /*       Vector2 bulletOffset = new Vector2(m_BulletTexture.Width / 2, m_BulletTexture.Height / 2);
            foreach (Player p in m_Teams)
            {
                List<Bullet> bullets = p.Tank.GetBulletList();
                foreach (Bullet b in bullets)
                {
                    m_SpriteBatch.Draw(m_BulletTexture, b.Position, null, p.Colour, 0, bulletOffset, 1f, SpriteEffects.None, 0.0f);
                }
            }
            */
            foreach (RectWall w in m_World.Walls)
            {
                w.DrawOutlines(m_SpriteBatch);
            }

            foreach (RectWall w in m_World.Walls)
            {
                w.Draw(m_SpriteBatch);
            }

            m_SpriteBatch.End();
        }

        public void Escape()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Tankontroller.Instance().SM().Transition(null);
            }
        }

        public void Update(float pSeconds)
        {
            Escape();

            IntroFinished();
            mController0.UpdateController();
            mController1.UpdateController();
            bool tankMoved = false;

            foreach (Player p in m_Teams)
            {
                tankMoved = tankMoved | p.DoTankControls(pSeconds);
            }

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

            m_SecondsTillTracksAdded -= pSeconds;
            if(m_SecondsTillTracksAdded <= 0)
            {
                m_SecondsTillTracksAdded += SECONDS_BETWEEN_TRACKS_ADDED;
                TrackSystem trackSystem = TrackSystem.GetInstance();
                foreach (Player p in m_Teams)
                {
                    trackSystem.AddTrack(p.Tank.GetWorldPosition(), p.Tank.GetRotation(), p.Tank.Colour());
                }
            }
        }

        private void Reset()
        {
            foreach (Player p in m_Teams)
            {
                p.Reset();
            }
        }

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
    }
}

