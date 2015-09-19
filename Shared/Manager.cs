using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;
using Parse;
using System.Threading.Tasks;

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
        static PackageSelector packselector;
        static Game parentGame;
        static Settings settings;
        static UserData userdata;
        const int timeout = 5000;
        //static EncryptionProvider crypto = new EncryptionProvider();
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
        static bool suppressconnectionwarning = false;
        static bool syncingdata = false, connected = false;
        internal static bool IsIdle { get { return !syncingdata; } }
        internal static async Task<Exception> SyncData()
        {            
            while (!IsIdle) { }
            syncingdata = true;
            if (ParseUser.CurrentUser != null)
            {
                try
                {
                    ParseObject data = await FetchUserData();
                    UserData online = new UserData();
                    online._timestamp = DateTime.Now;
                    if (data.ContainsKey("PData"))
                        online.PData = (string)data["PData"];
                    if (data.ContainsKey("SData"))
                        online.SData = (string)data["SData"];
                    online.LoadRawData();
                    userdata.UpdateFrom(online);
                    userdata.UpdateRawData();
                    userdata.EncryptStrings();
                    data["PData"] = userdata.PData;
                    data["SData"] = userdata.SData;
                    SaveUserDataLocal();
                    await data.SaveAsync();                    
                    connected = true;
                }
                catch (Exception e)
                {
                    connected = false;
                    return e;
                }
            }
            SaveUserDataLocal();
            syncingdata = false;
            return null;
        }

        internal static void SaveUserDataLocal()
        {
            userdata.UpdateRawData();
            userdata.EncryptStrings();
            DataHandler.SaveData<UserData>(userdata, datafile);
        }

        internal static Task<Exception> LoadUserDataLocal()
        {
            while (!IsIdle) { }
            UserData temp = DataHandler.LoadData<UserData>(datafile);
            if (temp != null) userdata = temp;
            else userdata = new UserData();
            userdata.LoadRawData();            
            return null;
        }

        private static async Task<ParseObject> FetchUserData()
        {
            await ParseUser.CurrentUser.FetchAsync();
            object nullableobj = null;
            if (ParseUser.CurrentUser.ContainsKey("data"))
                nullableobj = ParseUser.CurrentUser["data"];
            string id = nullableobj == null ? "" : nullableobj.ToString();
            ParseQuery<ParseObject> query = ParseObject.GetQuery("UserData");
            if ((from obj in query where obj.ObjectId == id select obj).CountAsync().Result == 0)
            {
                // first time
                ParseObject data = new ParseObject("UserData");
                data.ACL = new ParseACL(ParseUser.CurrentUser);
                await data.SaveAsync();
                ParseUser.CurrentUser["data"] = data.ObjectId;
                await ParseUser.CurrentUser.SaveAsync();
                await ParseUser.CurrentUser.FetchAsync();
                if (ParseUser.CurrentUser["data"].ToString() == data.ObjectId)
                    return data;
                else
                {
                    // something went wrong, retry
                    await data.DeleteAsync();
                    return await FetchUserData();
                }
            }
            return await query.GetAsync(id);
        }

        internal static void HandleShareReq(string v)
        {
            MessageBox.Show("Req", "Request to share " + v, new string[] { "OK" });
        }

        internal static StateManager StateManager { get { return stateManager; } }
        internal static ContentManager ContentManager { get { return contentManager; } }
        internal static Settings GameSettings { get { return settings; } set { settings = value; } }
        //internal static EncryptionProvider Cipher { get { return crypto; } set { crypto = value; } }
        internal static Game Parent { get { return parentGame; } }
        internal static UserData UserData { get { return userdata; } set { userdata = value; } }
        internal static bool Connected { get { return connected; } }
        internal static void init(Game parent)
        {
            LoadUserDataLocal();
            SyncData();
            parentGame = parent;
            contentManager = parent.Content;
            LoadSettings();
            DataHandler.LoadCurrentTheme();
            stateManager = new StateManager();
            menu = new MainMenu();
            optionsmenu = new OptionsMenu();
            savemenu = new SaveMenu();
            stagecont = new StageContainer(false);
            selector = new LevelSelector();            
            packselector = new PackageSelector();

            stateManager.AddGameState(GameState.MainMenu, menu);
            stateManager.AddGameState(GameState.SelectLevel, selector);
            stateManager.AddGameState(GameState.SaveLevel, savemenu);
            stateManager.AddGameState(GameState.OnStage, stagecont);
            StateManager.AddGameState(GameState.EditMode, stagecont);            
            stateManager.AddGameState(GameState.Options, optionsmenu);
            stateManager.AddGameState(GameState.PackageSelector, packselector);

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

        internal static void Play(string levelname, PackageType package = PackageType.User)
        {
            if (!initd) return;
            stateManager.SwitchTo(GameState.OnStage, null, levelname, package);
        }

        internal static void HandleEvent(WorldEvent e)
        {
            if (!initd) return;
            VirtualKeyboard.HandleEvent(e);
            stateManager.CurrentGameState.HandleEvent(e);
        }
        internal static void Draw(SpriteBatch spriteBatch)
        {
            if (!initd) return;
            stateManager.Draw(spriteBatch);
            VirtualKeyboard.Draw(spriteBatch);
        }
        internal static void Update(GameTime time)
        {
            if (!initd) return;
            stateManager.Update(time);
            SoundManager.Update(time);
            VirtualKeyboard.Update(time);
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
            HandleEvent(new TouchAllFingersOffEvent());
        }

        private static void drcomplete(Vector2 position)
        {
            HandleEvent(new TouchDragCompleteEvent(position));
        }

        private static void keyup(Keys k)
        {
            HandleEvent(new KeyUpEvent(k));
        }

        private static void mdown(InputManager.MouseKey k, Point position)
        {
            HandleEvent(new MouseDownEvent(k, position));
        }

        private static void pinched(float delta)
        {
            HandleEvent(new TouchPinchEvent(delta));
        }

        private static void tapped(Vector2 position)
        {
            HandleEvent(new TouchTapEvent(position));
        }

        private static void dragged(Vector2 delta, Vector2 pos)
        {
            HandleEvent(new TouchFreeDragEvent(delta, pos));
        }

        private static void mmoved(Point position, Point offset)
        {
            HandleEvent(new MouseMovedEvent(position, offset));
        }

        private static void mscrolled(int value)
        {
            HandleEvent(new MouseScrollEvent(value));
        }

        private static void mup(InputManager.MouseKey k, Point pos)
        {
            HandleEvent(new MouseUpEvent(k, pos));
        }

        private static void keydown(Keys k)
        {
            HandleEvent(new KeyDownEvent(k));
        }
    }
}
