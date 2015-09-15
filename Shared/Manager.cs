using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;

namespace Inlumino_SHARED
{
    internal static class Manager
    {
        static StateManager stateManager;
        static MainMenu menu;
        static SaveMenu savemenu;
        static LevelSelector selector;
        static StageContainer stagecont;
        static ContentManager contentManager;
        static OptionsMenu optionsmenu;
        static DeleteMenu delmenu;
        static Game parentGame;
        static Settings settings;
        static UserData userdata;
        static EncryptionProvider crypto = new EncryptionProvider();
        static bool initd = false;
        const string settingsfile = "GameSettings.xml";
        const string datafile = "UserData.xml";
        internal static void SaveSettings()
        {
            DataHandler.SaveData<Settings>(settings, settingsfile);
        }
        internal static void LoadSettings()
        {
            Settings temp = DataHandler.LoadData<Settings>(settingsfile);
            if (temp != null) GameSettings = temp;
            else settings = new Settings();
        }
        internal static void SaveUserData()
        {
            DataHandler.SaveData<UserData>(userdata, datafile);
        }
        internal static void LoadUserData()
        {
            UserData temp = DataHandler.LoadData<UserData>(datafile);
            if (temp != null) userdata = temp;
            else userdata = new UserData();
        }
        internal static StateManager StateManager { get { return stateManager; } }
        internal static ContentManager ContentManager { get { return contentManager; } }
        internal static Settings GameSettings { get { return settings; } set { settings = value; } }
        internal static EncryptionProvider Cipher { get { return crypto; } set { crypto = value; } }
        internal static Game Parent { get { return parentGame; } }
        public static UserData UserData { get { return userdata; } set { userdata = value; } }

        internal static void init(Game parent)
        {
            parentGame = parent;
            contentManager = parent.Content;
            LoadSettings();
            LoadUserData();
            DataHandler.LoadCurrentTheme();
            stateManager = new StateManager();
            menu = new MainMenu();
            optionsmenu = new OptionsMenu();
            savemenu = new SaveMenu();
            stagecont = new StageContainer(false);
            selector = new LevelSelector();
            delmenu = new DeleteMenu();

            stateManager.AddGameState(GameState.MainMenu, menu);
            stateManager.AddGameState(GameState.SelectLevel, selector);
            stateManager.AddGameState(GameState.SaveLevel, savemenu);
            stateManager.AddGameState(GameState.OnStage, stagecont);
            StateManager.AddGameState(GameState.EditMode, stagecont);
            stateManager.AddGameState(GameState.DeleteLevel, delmenu);
            stateManager.AddGameState(GameState.Options, optionsmenu);

            initInput();

            stateManager.SwitchTo(GameState.MainMenu);

            initd = true;
        }

        internal static void StartEditor()
        {
            if (!initd) return;
            stagecont.InitEditing();
            stateManager.SwitchTo(GameState.EditMode);
        }

        internal static void Play(string levelname, bool ismainlevel)
        {
            if (!initd) return;
            stateManager.SwitchTo(GameState.OnStage, null, levelname, ismainlevel);
        }

        internal static void HandleEvent(WorldEvent e)
        {
            if (!initd) return;
            stateManager.CurrentGameState.HandleEvent(e);
        }
        internal static void Draw(SpriteBatch spriteBatch)
        {
            if (!initd) return;
            stateManager.Draw(spriteBatch);
        }
        internal static void Update(GameTime time)
        {
            if (!initd) return;
            stateManager.Update(time);
            SoundManager.Update(time);
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
            InputManager.DragComplete += drcomplete;
            InputManager.AllFingersOff += afo;
        }

        private static void afo()
        {
            stateManager.CurrentGameState.HandleEvent(new TouchAllFingersOffEvent());
        }

        private static void drcomplete(Vector2 position)
        {
            stateManager.CurrentGameState.HandleEvent(new TouchDragCompleteEvent(position));
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

        private static void dragged(Vector2 delta, Vector2 pos)
        {
            StateManager.CurrentGameState.HandleEvent(new TouchFreeDragEvent(delta, pos));
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
