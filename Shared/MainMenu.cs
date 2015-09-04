using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Inlumino_SHARED
{
    class MainMenu : IState
    {
        protected UIButton playButton, editorButton, OptionsButton;
        protected UIMenu mainmenu;

        public MainMenu()
        {
            mainmenu = new UIMenu();

            playButton = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.PlayBtn], 1);
            editorButton = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.EditModeBtn], 1);

            playButton.Pressed += playpressed;
            editorButton.Pressed += editpressed;

            mainmenu.Add(playButton);
            mainmenu.Add(editorButton);
        }

        private void SetupMenu()
        {
            playButton.setSizeRelative(0.3f * (Screen.Mode == Orientation.Portrait ? 2 : 1), Screen.Mode);
            editorButton.setSizeRelative(0.3f * (Screen.Mode == Orientation.Portrait ? 2 : 1), Screen.Mode);

            playButton.Position = new Vector2((Screen.Width - playButton.Size.X) / 2, Screen.Height * 0.2f);
            editorButton.Position = new Vector2((Screen.Width - editorButton.Size.X) / 2, playButton.BoundingBox.Bottom + Screen.Height * 0.05f);
        }

        private void editpressed(UIButton sender)
        {
            Manager.StartEditor();
        }

        private void playpressed(UIButton sender)
        {
            Manager.StateManager.SwitchTo(GameState.SelectLevel);
        }

        public void Update(GameTime time)
        {
            mainmenu.Update(time);
        }

        public void Draw(SpriteBatch batch)
        {
            mainmenu.Draw(batch);
        }

        public void Clear()
        {
            mainmenu.Clear();
        }

        public void HandleEvent(WorldEvent e, bool forcehandle = false)
        {
            if (e is DisplaySizeChangedEvent || e is OrientationChangedEvent)
                SetupMenu();
            mainmenu.HandleEvent(e);
        }

        public void OnActivated(params object[] args)
        {
            SetupMenu();
        }
    }
}
