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

            playButton = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.PlayBtn],1);
            editorButton = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.EditModeBtn], 1);
            mainmenu.Add(playButton);
            mainmenu.Add(editorButton);

            SetupMenu();            
        }

        private void SetupMenu()
        {            
            playButton.Position = new Vector2((Screen.Width - playButton.Size.X) / 2, Screen.Height * 0.2f);            
            playButton.Pressed += playpressed;


            editorButton.Position = new Vector2((Screen.Width - editorButton.Size.X) / 2, playButton.BoundingBox.Bottom + Screen.Height * 0.05f);         
            editorButton.Pressed += editpressed;
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

        public void HandleEvent(WorldEvent e, bool forcehandle=false)
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
