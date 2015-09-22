using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Parse;
using Microsoft.Xna.Framework.Input;
using System.Threading.Tasks;

namespace Inlumino_SHARED
{
    class MainMenu : IState
    {
        protected UIButton playButton, editorButton, optionsButton;
        protected UIMenu mainmenu;
        internal MainMenu()
        {            
            mainmenu = new UIMenu();

            playButton = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.PlayBtn]);
            editorButton = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.EditModeBtn]);
            optionsButton = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.OptionsBtn]);

            playButton.Pressed += playpressed;
            editorButton.Pressed += editpressed;
            optionsButton.Pressed += optionspressed;

            mainmenu.Add(playButton);
            mainmenu.Add(editorButton);
            mainmenu.Add(optionsButton);            
        }

        private void SetupMenu()
        {
            float logo = Screen.Height * 0.25f;
            mainmenu.setAllSizeRelative(0.25f * 0.75f, Orientation.Landscape);
            mainmenu.Position = new Vector2(0, logo + Screen.Height * 0.1f);
            mainmenu.ArrangeInForm(Orientation.Portrait);
            mainmenu.Position = new Vector2((Screen.Width - mainmenu.Size.X) / 2, mainmenu.GlobalPosition.Y);
        }

        private void editpressed(UIButton sender)
        {
            Manager.StartEditor();
        }

        private void optionspressed(UIButton sender)
        {
            Manager.StateManager.SwitchTo(GameState.Options);
        }

        private void playpressed(UIButton sender)
        {
            Manager.StateManager.SwitchTo(GameState.PackageSelector);
        }

        public void Update(GameTime time)
        {
            mainmenu.Update(time);
        }

        public void Draw(SpriteBatch batch)
        {
            Texture2D background = DataHandler.getTexture(PrimaryTexture._MMBG);
            float w = Screen.Height * background.Width / background.Height;
            batch.Draw(background, new Rectangle((int)(Screen.Width - w) / 2, 0, (int)(w), (int)Screen.Height), Color.White);
            mainmenu.Draw(batch);
        }

        internal void Clear()
        {
            mainmenu.Clear();
        }

        public void HandleEvent(WorldEvent e, bool forcehandle = false)
        {
            if (e is DisplaySizeChangedEvent || e is OrientationChangedEvent)
                SetupMenu();
            mainmenu.HandleEvent(e);
        }
        bool suppressmessage = false, first = true;
        public void OnActivated(params object[] args)
        {
            Manager.LoadUserDataLocal();
            SetupMenu();
#if WP81
            Manager.SyncData();
#endif
            Task t = new Task(() => CheckOnline());
            t.Start();
        }

        private async Task CheckOnline()
        {
            while (!Manager.IsIdle) { }
            if (!suppressmessage && ParseUser.CurrentUser == null)
            {
                int? r = await MessageBox.Show("Hello", "It looks like you are not syncing your data online. Would you like to setup your account?", new string[] { "Take me there", "Remind me later" });
                if (r == 0) Manager.StateManager.SwitchTo(GameState.Options);
                suppressmessage = true;
            }
            else if (first && !Manager.Connected)
            {                
                if (ParseUser.CurrentUser != null)
                    await MessageBox.Show("ERROR", "Failed to sync your data to the cloud.", new string[] { "OK" });
            }
            first = false;
        }
    }
}
