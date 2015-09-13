using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Inlumino_SHARED
{
    class MainMenu : IState
    {
        protected UIButton playButton, editorButton, optionsButton;
        protected UIMenu mainmenu;
        Texture2D background;
        public MainMenu()
        {
            background = DataHandler.getTexture(3);
            mainmenu = new UIMenu();

            playButton = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.PlayBtn]);
            editorButton = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.EditModeBtn]);
            optionsButton = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.OptionsBtn]);

            playButton.Pressed += playpressed;
            editorButton.Pressed += editpressed;
            optionsButton.Pressed += optionspressed;

            mainmenu.Add(playButton);
            mainmenu.Add(editorButton);
            //mainmenu.Add(optionsButton);

            SoundManager.PlaySound(DataHandler.Sounds[SoundType.Background], SoundCategory.Music, true);
        }

        private void SetupMenu()
        {
            float logo = Screen.Height * 0.25f;
            mainmenu.setAllSizeRelative(0.25f, Orientation.Landscape);
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
            Manager.StateManager.SwitchTo(GameState.SelectLevel);
        }

        public void Update(GameTime time)
        {
            mainmenu.Update(time);
        }

        public void Draw(SpriteBatch batch)
        {
            float w = Screen.Height * background.Width / background.Height;
            batch.Draw(background, new Rectangle((int)(Screen.Width - w) / 2, 0, (int)(w), (int)Screen.Height), Color.White);
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
