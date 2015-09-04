using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;

namespace Inlumino_SHARED
{
    public static class Manager
    {
        static StateManager stateManager;
        static MainMenu menu;
        static SaveMenu savemenu;
        static LevelSelector selector;
        static StageContainer stagecont;
        static StageContainer editablecont;
        static ContentManager contentManager;
        static DeleteMenu delmenu;
        static Game parentGame;
        static bool initd = false;

        public static StateManager StateManager { get { return stateManager; } }
        public static ContentManager ContentManager { get { return contentManager; } }
        public static Game Parent { get { return parentGame; } }
        public static void init(Game parent)
        {
            parentGame = parent;
            contentManager = parent.Content;
            stateManager = new StateManager();
            menu = new MainMenu();
            savemenu = new SaveMenu();
            stagecont = new StageContainer(false);
            editablecont = new StageContainer(true);
            selector = new LevelSelector();
            delmenu = new DeleteMenu();

            stateManager.AddGameState(GameState.MainMenu, menu);
            stateManager.AddGameState(GameState.SelectLevel, selector);
            stateManager.AddGameState(GameState.SaveLevel, savemenu);
            stateManager.AddGameState(GameState.OnStage, stagecont);
            StateManager.AddGameState(GameState.EditMode, editablecont);
            stateManager.AddGameState(GameState.DeleteLevel, delmenu);

            stateManager.SwitchTo(GameState.MainMenu);

            initInput();

            initd = true;
        }

        public static void StartEditor()
        {
            if (!initd) return;
            editablecont.InitEditing();
            stateManager.SwitchTo(GameState.EditMode);
        }

        public static void Play(string levelname, bool ismainlevel)
        {
            if (!initd) return;
            stateManager.SwitchTo(GameState.OnStage, null, levelname, ismainlevel);
        }

        public static void HandleEvent(WorldEvent e)
        {
            if (!initd) return;
            stateManager.CurrentGameState.HandleEvent(e);
        }
        public static void Draw(SpriteBatch spriteBatch)
        {
            if (!initd) return;
            stateManager.Draw(spriteBatch);
        }
        public static void Update(GameTime time)
        {
            if (!initd) return;
            stateManager.Update(time);
        }

        private static void initInput()
        {
            InputManager.init();
            InputManager.KeyDown += keydown;
            InputManager.KeyUp += keyup;
            InputManager.MouseDown += mdown;
            InputManager.MouseUp += mup;
            InputManager.Scrolled += mscrolled;
            InputManager.MouseMoved += mmoved;
            InputManager.Dragged += dragged;
            InputManager.Tapped += tapped;
            InputManager.Pinched += pinched;
        }

        private static void keyup(Keys k)
        {
            stateManager.CurrentGameState.HandleEvent(new KeyUpEvent(k));
        }

        private static void mdown(InputManager.MouseKey k, Point position)
        {
            stateManager.CurrentGameState.HandleEvent(new MouseDownEvent(k, position));
        }

        private static void pinched(float delta)
        {
            StateManager.CurrentGameState.HandleEvent(new TouchPinchEvent(delta));
        }

        private static void tapped(Vector2 position)
        {
            StateManager.CurrentGameState.HandleEvent(new TouchTapEvent(position));
        }

        private static void dragged(Vector2 delta)
        {
            StateManager.CurrentGameState.HandleEvent(new TouchFreeDragEvent(delta));
        }

        private static void mmoved(Point position, Point offset)
        {
            StateManager.CurrentGameState.HandleEvent(new MouseMovedEvent(position, offset));
        }

        private static void mscrolled(int value)
        {
            StateManager.CurrentGameState.HandleEvent(new MouseScrollEvent(value));
        }

        private static void mup(InputManager.MouseKey k, Point pos)
        {
            StateManager.CurrentGameState.HandleEvent(new MouseUpEvent(k, pos));
        }

        private static void keydown(Keys k)
        {
            StateManager.CurrentGameState.HandleEvent(new KeyDownEvent(k));
        }
    }
}
