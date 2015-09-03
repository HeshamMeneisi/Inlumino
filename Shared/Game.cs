using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

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
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Screen.SetUp(Window, graphics);
            Screen.SetFullScreen(true);
#if ANDROID
            graphics.SupportedOrientations = DisplayOrientation.Portrait | DisplayOrientation.PortraitDown | DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;            
#else
            graphics.SupportedOrientations = DisplayOrientation.Portrait | DisplayOrientation.PortraitDown;
#endif
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
            DataHandler.LoadTextures(Content);
            DataHandler.LoadFonts(Content);
            Manager.init(this);
            graphics.ApplyChanges();
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

        public Texture2D TakeScreenshot()
        {
            RenderTarget2D screenshot;
            screenshot = new RenderTarget2D(GraphicsDevice, (int)Screen.Width, (int)Screen.Height);
            GraphicsDevice.SetRenderTarget(screenshot);
            Draw(_lastdrawtime);
            GraphicsDevice.Present();
            GraphicsDevice.SetRenderTarget(null);
            return screenshot;
        }
    }
}
