using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Parse;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Inlumino_SHARED
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public Game()
        {
            ParseClient.Initialize("XxT2BsMH9JlhdvG8tITFXCVrq5Qur8piPOJKQodU", "b4tHTzoZPlnY174EankGG20zRM5RVNesjaFBrFaz");
            ParseFacebookUtils.Initialize("906591706062304");

            ParseAnalytics.TrackAppOpenedAsync();

            //ParseInstallation.CurrentInstallation.AddUniqueToList("channels", "main");
            //ParseInstallation.CurrentInstallation.SaveAsync();          

            graphics = new GraphicsDeviceManager(this);
            Content = new SmartContentManager(Content.ServiceProvider);
            Content.RootDirectory = "Content";
            Screen.SetUp(Window, graphics);
            Screen.SetFullScreen(true);
#if WINDOWS_UAP
            IsMouseVisible = true;
            graphics.SupportedOrientations = DisplayOrientation.Portrait | DisplayOrientation.PortraitDown;
#endif
#if !WP81
            graphics.SupportedOrientations = DisplayOrientation.Portrait | DisplayOrientation.PortraitDown | DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
#endif
#if WP81
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
#endif

            Window.ClientSizeChanged += sizechanged;
            Window.OrientationChanged += orientationchanged;
        }
        bool Loaded = false;
        private void orientationchanged(object sender, EventArgs e)
        {
            if (!Loaded) return;
            OrientationChangedEvent ev = new OrientationChangedEvent(Window.CurrentOrientation);
            Manager.HandleEvent(ev);
        }

        private void sizechanged(object sender, EventArgs e)
        {
            if (!Loaded) return;
            Manager.HandleEvent(new DisplaySizeChangedEvent(Window.ClientBounds.Size.ToVector2()));
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
            Deactivated += exiting;
        }

        private void exiting(object sender, EventArgs e)
        {
            if (!Loaded) return;
            Manager.SaveSettings();
            Manager.SyncData();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            DisplayOrientation t = Window.CurrentOrientation;
            //Screen.MakeVirtual(new Vector2(800, 480));
            Manager.init(this);
            Loaded = true;
            /*
            foreach (PackageType p in Manager.UserData.PackageAvailability.Keys)
            {
                DataHandler.GetPackageThumb(p);
                if (Manager.UserData.PackageAvailability[p])
                    foreach (string l in Common.Packages[p])
                        DataHandler.GetLevelThumb(l, p);
            }*/
            graphics.ApplyChanges();
            // This is to override the bug in monogame
#if WP81
            graphics.SupportedOrientations = DisplayOrientation.Portrait | DisplayOrientation.PortraitDown | DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
#endif
#if DEBUG
            //UnlockAll();
            //CheckSolved();
            //ScreenShotAll();
#endif
        }
#if DEBUG
        /// <summary>
        /// Debugging function
        /// </summary>
        /// <returns></returns>
        private async Task CheckSolved()
        {
            foreach (string name in DataHandler.getSavedLevelNames())
            {
                Stage temp = Common.CreateLevel(name, PackageType.User);
                temp.SetSourceStatus(true);
                if (!temp.CheckWin())
                    if (await AlertHandler.ShowMessage("WARNING", "Level not solved: " + name, new string[] { "Ok", "Stop" }) == 1) return;

            }
        }

        /// <summary>
        /// Debugging function
        /// </summary>
        private void UnlockAll()
        {
            foreach (PackageType pack in Common.Packages.Keys)
            {
                Manager.UserData.MakeAvailable(pack);
                foreach (string s in Common.Packages[pack])
                    Common.SetScore(pack, s, 3);
            }
        }
        /// <summary>
        /// Debugging function
        /// </summary>
        private void ScreenShotAll()
        {
            foreach (string name in DataHandler.getSavedLevelNames())
            {
                Stage temp = Common.CreateLevel(name, PackageType.User);
                DataHandler.SaveStage(temp, name);
            }
        }
#endif
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed && !Manager.StateManager.SwitchBack())
                Exit();

            // TODO: Add your update logic here
            InputManager.Update(gameTime);
            Manager.Update(gameTime);
            base.Update(gameTime);
        }
        GameTime _lastdrawtime = new GameTime();
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            _lastdrawtime = gameTime;


            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            spriteBatch.Begin();
            Manager.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
        internal Texture2D TakeScreenshot()
        {
            RenderTarget2D screenshot;
            screenshot = new RenderTarget2D(GraphicsDevice, (int)Screen.Width, (int)Screen.Height);
            GraphicsDevice.SetRenderTarget(screenshot);
            Draw(_lastdrawtime);
            GraphicsDevice.Present();
            GraphicsDevice.SetRenderTarget(null);
            return screenshot;
        }
        internal Texture2D TakeScreenshot(Stage currentLevel)
        {
            RenderTarget2D screenshot;
            screenshot = new RenderTarget2D(GraphicsDevice, (int)Screen.Width, (int)Screen.Height);
            GraphicsDevice.SetRenderTarget(screenshot);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            currentLevel.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(_lastdrawtime);
            GraphicsDevice.Present();
            GraphicsDevice.SetRenderTarget(null);
            return screenshot;
        }
        internal Texture2D Concat(params TextureID[] args)
        {
            int w = 0, h = 0;
            foreach (TextureID tid in args)
            {
                w += (int)tid.TotalWidth; h = Math.Max(h, (int)tid.TotalHeight);
            }
            RenderTarget2D target = new RenderTarget2D(GraphicsDevice, w, h);
            GraphicsDevice.SetRenderTarget(target);
            GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin();
            int x = 0;
            foreach (TextureID tid in args)
            {
                spriteBatch.Draw(DataHandler.getTexture(tid.RefKey), new Rectangle(x, 0, (int)tid.TotalWidth, (int)tid.TotalHeight), DataHandler.getTextureSource(tid), Color.White);
                x += (int)tid.TotalWidth;
            }
            spriteBatch.End();
            GraphicsDevice.Present();
            GraphicsDevice.SetRenderTarget(null);
            return target;
        }
    }
}
