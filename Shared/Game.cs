using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Parse;

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
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Screen.SetUp(Window, graphics);
            Screen.SetFullScreen(true);
#if WINDOWS_UAP
            IsMouseVisible = true;
#endif
            graphics.SupportedOrientations = DisplayOrientation.Portrait | DisplayOrientation.PortraitDown | DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            Window.ClientSizeChanged += sizechanged;
            Window.OrientationChanged += orientationchanged;
        }
        private void orientationchanged(object sender, EventArgs e)
        {
            OrientationChangedEvent ev = new OrientationChangedEvent(Window.CurrentOrientation);
            Manager.HandleEvent(ev);
        }

        private void sizechanged(object sender, EventArgs e)
        {
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
            Manager.SaveSettings();Manager.SaveUserData();
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
            Manager.init(this);
            graphics.ApplyChanges();
            //Temp Code
            //Manager.UserData.PackageAvailability[PackageType.Space] = true;
            //for (int i = 0; i < Common.SpacePackge.Length; i++)
            //Manager.UserData.setStars(PackageType.Space, i, 3);
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
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
