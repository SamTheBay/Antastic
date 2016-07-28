using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Microsoft.Advertising.Mobile.Xna;
using Microsoft.Phone.Shell;

namespace Antastic
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Antastic : Microsoft.Xna.Framework.Game
    {
        public static GraphicsDeviceManager graphics;
        public static ScreenManager screenManager;
        public static AdControlManager adControlManager;
        public static Game sigletonGame;
        public static int masterController = 0;

        public static Rectangle ScreenSize = new Rectangle(0, 0, 480, 800);
        private static Vector2 gameTimeDrawLoc = new Vector2(100, 100);
        private static Vector2 maxGameTimeDrawLoc = new Vector2(100, 130);
        private static Vector2 memoryDrawLoc = new Vector2(100, 160);
        public static GameplayScreen gameplayScreen;


        public Antastic()
        {
            sigletonGame = this;
            graphics = new GraphicsDeviceManager(this);

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Pre-auto scale settings.
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;
            graphics.IsFullScreen = true;
            graphics.SupportedOrientations = DisplayOrientation.Portrait;

            Content.RootDirectory = "Content";

            PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;

            // add the screen manager
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            GameSprite.game = this;

            adControlManager = new AdControlManager(Components, false);
            adControlManager.ShowAds = true;

            Activated += new EventHandler<EventArgs>(AntasticOnActivated);
            Deactivated += new EventHandler<EventArgs>(AntasticDeactivated);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //InputManager.Initialize();

            base.Initialize();

            AntSprite.InitializePaths();
            AppleSprite.InitializeStartPositions();
            Fonts.LoadContent(Content);
            Accelerometer.Initialize();
            InternalContentManager.Load();
            AudioManager.Initialize(Antastic.sigletonGame);
            AudioManager.audioManager.LoadSFX(0, 2);
            new MusicManager();
            //MusicManager.SingletonMusicManager.LoadTune("intro", TunnelDecent.sigletonGame.Content);

            gameplayScreen = new GameplayScreen();
            screenManager.AddScreen(new MenuBackgroundScreen());
            screenManager.AddScreen(new TitleScreen());
        }


        void AntasticOnActivated(object sender, EventArgs args)
        {
            if (adControlManager != null)
            {
                adControlManager.Load();
            }

            // check if we have a game currently running
            if (GameplayScreen.singleton != null && GameplayScreen.singleton.IsPlaying && !(screenManager.GetScreens()[screenManager.GetScreens().Length - 1] is PauseScreen))
            {
                screenManager.AddScreen(new PauseScreen());
            }
        }

        void AntasticDeactivated(object sender, EventArgs e)
        {
            if (adControlManager != null)
            {
                adControlManager.UnLoad();
            }
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            adControlManager.Load();
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Fonts.UnloadContent();

            base.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            InputManager.Update();

            base.Update(gameTime);

            adControlManager.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }
    }
}
