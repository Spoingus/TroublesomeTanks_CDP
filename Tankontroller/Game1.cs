using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using Tankontroller.Controller;
using Tankontroller.Scenes;

namespace Tankontroller
{
    /// <summary>
    /// This interface specifies things that we want to get global access to (probs everything)
    /// </summary>
    public interface IGame
    {
        SoundManager GetSoundManager();
        SceneManager SM();
        ContentManager CM();
        GraphicsDeviceManager GDM();

        IController GetController(int pIndex);
        int GetControllerCount();
        List<IController> GetControllers();
        Task DetectControllers();
        void Exit();
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Tankontroller : Game, IGame
    {
        private SoundManager mSoundManager;
        private GraphicsDeviceManager mGraphics;
        private SpriteBatch mBatch;
        private SceneManager mSceneManager;
        private Dictionary<string, IController> mControllers;
        private static IGame mGameInterface = null;
        private SoundEffectInstance mCurrentMusic;
        private string mCurrentMusicName;

        public static IGame Instance()
        {
            if(mGameInterface == null)
            {
                mGameInterface = new Tankontroller();
            }
            return mGameInterface;
        }
        public async Task DetectControllers()
        {
            string[] portNames = SerialPort.GetPortNames();
            foreach (string portName in portNames)
            {
                if (!mControllers.ContainsKey(portName))
                {
                    SerialPort port = new SerialPort(portName, 19200);//, Parity.None, 8, StopBits.One);

                    port.Open();

                    port.DtrEnable = true;

                    port.ReadTimeout = 10;
                    port.WriteTimeout = 10;

                    port.DiscardInBuffer();
                    //port.Write(new byte[] { (byte)'I' }, 0, 1);
                    await port.BaseStream.WriteAsync(new byte[] { (byte)'I' }, 0, 1);

                    System.Threading.Thread.Sleep(10); 

                    if (port.BytesToRead > 0)
                    {
                        // The comms starts with 0xff 0xff
                        try
                        {
                            string response = port.ReadLine();
                            if (response == "Tankontroller")
                            {
                                Hacktroller hacktroller = new Hacktroller(port);
                                IController controller = new ModularController(hacktroller);
                                mControllers.Add(portName, controller);
                            }
                            else
                            {
                                port.Close();
                            }
                        }
                        catch(Exception e)
                        {
                            //possible timeout - ignore
                        }
                    }
                    else { port.Close(); }
                }
            }

            List<string> toRemove = new List<string>();
            foreach (KeyValuePair<string, IController> controller in mControllers)
            {
                if (!controller.Value.lights_on())
                {
                    toRemove.Add(controller.Key);
                }
            }
            foreach (string key in toRemove)
            {
                mControllers.Remove(key);
            }
        }

        public void TurnOffControllers()
        {
            foreach (KeyValuePair<string, IController> controller in mControllers)
            {
                controller.Value.TurnOffLights();
            }
        }

        public IController GetController(int pIndex) {  return mControllers.Values.ElementAt(pIndex); }
        public int GetControllerCount() { return mControllers.Count; }
        public List<IController> GetControllers() { return mControllers.Values.ToList(); }
       
        public SoundManager GetSoundManager() { return mSoundManager; }
        public SceneManager SM() { return mSceneManager; }
        public ContentManager CM() { return Content; }
        public GraphicsDeviceManager GDM() { return mGraphics; }
        public Tankontroller()
        {
            mGraphics               = new GraphicsDeviceManager(this);
            Content.RootDirectory   = "Content";
            mSceneManager           = new SceneManager();
            mControllers = new Dictionary<string, IController>();
            
                
            if (DGS.Instance.GetBool("ADD_KEYBOARD_CONTROLLER_1"))
            {
                    
                Dictionary<Keys, Control> Player1KeyMap = new Dictionary<Keys, Control>
                {
                    { Keys.Q, Control.LEFT_TRACK_FORWARDS },
                    { Keys.A, Control.LEFT_TRACK_BACKWARDS },
                    { Keys.W, Control.RIGHT_TRACK_FORWARDS },
                    { Keys.S, Control.RIGHT_TRACK_BACKWARDS },
                    { Keys.Z, Control.TURRET_LEFT },
                    { Keys.X, Control.TURRET_RIGHT },
                    { Keys.Space, Control.FIRE },
                    { Keys.C, Control.RECHARGE }
                };

                Dictionary<Keys, int> Player1PortMap = new Dictionary<Keys, int>
                {
                    { Keys.D1, 3 },
                    { Keys.D2, 2 },
                    { Keys.D3, 6 },
                    { Keys.D4, 7 },
                    { Keys.D5, 1 },
                    { Keys.D6, 0 },
                    { Keys.D7, 5 },
                    { Keys.D8, 4 }
                };

                IController controller = new KeyboardController(Player1KeyMap, Player1PortMap);
                mControllers.Add("Keyboard1", controller);
            }

            if(DGS.Instance.GetBool("ADD_KEYBOARD_CONTROLLER_2"))
            {
                Dictionary<Keys, Control> Player2KeyMap = new Dictionary<Keys, Control>
                {
                    { Keys.Insert, Control.LEFT_TRACK_FORWARDS },
                    { Keys.Delete, Control.LEFT_TRACK_BACKWARDS },
                    { Keys.Home, Control.RIGHT_TRACK_FORWARDS },
                    { Keys.End, Control.RIGHT_TRACK_BACKWARDS },
                    { Keys.PageUp, Control.TURRET_LEFT },
                    { Keys.PageDown, Control.TURRET_RIGHT },
                    { Keys.Enter, Control.FIRE },
                    { Keys.P, Control.RECHARGE }
                };

                Dictionary<Keys, int> Player2PortMap = new Dictionary<Keys, int>
                {
                    { Keys.F1, 3 },
                    { Keys.F2, 2 },
                    { Keys.F3, 6 },
                    { Keys.F4, 7 },
                    { Keys.F5, 1 },
                    { Keys.F6, 0 },
                    { Keys.F7, 5 },
                    { Keys.F8, 4 }
                };

                IController controller = new KeyboardController(Player2KeyMap, Player2PortMap);
                mControllers.Add("Keyboard2", controller);
            }

            mSoundManager = new SoundManager();

            mGraphics.PreferredBackBufferHeight = DGS.Instance.GetInt("SCREENHEIGHT");
            mGraphics.PreferredBackBufferWidth = DGS.Instance.GetInt("SCREENWIDTH");
            mGraphics.IsFullScreen = DGS.Instance.GetBool("IS_FULL_SCREEN");
            this.Window.Title = "TroubleSome Tanks - CDP Edition!";
        }

        

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            mBatch = new SpriteBatch(GraphicsDevice);
            mSoundManager.Add("Music/Music_start");
            mSoundManager.Add("Music/Music_intro");
            mSoundManager.Add("Music/Music_loopable");
            mSoundManager.Add("Sounds/Tank_Gun");
            mSoundManager.Add("Sounds/Tank_Tracks");
            mSoundManager.Add("Sounds/Tank_Clang1");
            mSoundManager.Add("Sounds/Tank_Clang2");
            mSoundManager.Add("Sounds/Tank_Clang3");

            mSceneManager.Push(new FlashScreenScene());
           // mSceneManager.Push(new GameScene());

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float seconds = 0.001f * gameTime.ElapsedGameTime.Milliseconds;
            mSceneManager.Update(seconds);
            base.Update(gameTime);
        }
        public SoundEffectInstance ReplaceCurrentMusicInstance(string pName, bool pLoopable)
        {
            if (mCurrentMusicName != pName)
            {
                if (mCurrentMusic != null)
                {
                    mCurrentMusic.Stop();
                }
                mCurrentMusicName = pName;
                SoundEffectInstance replacement = mSoundManager.GetSoundEffectInstance(pName);
                replacement.IsLooped = pLoopable;
                mCurrentMusic = replacement;
                mCurrentMusic.Play();
            }
            mCurrentMusic.IsLooped = pLoopable;

            return mCurrentMusic;
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            float seconds = 0.001f * gameTime.ElapsedGameTime.Milliseconds;
            mSceneManager.Draw(seconds);
            base.Draw(gameTime);
        }
    }
}
