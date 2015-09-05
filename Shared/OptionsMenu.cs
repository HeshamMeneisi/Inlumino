using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Inlumino_SHARED
{
    class OptionsMenu : IState
    {
        UIMenu menu;
        UIButton savebtn;
        public OptionsMenu()
        {
            menu = new UIMenu();
            // Need sliders for music & sfx volume
            savebtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.SaveButton]);
            savebtn.Pressed += savepressed;
            menu.Add(savebtn);
        }

        private void savepressed(UIButton sender)
        {
            //Manager.GameSettings.MusicVolume =
            //Manager.GameSettings.SFXVolume =
            Manager.SaveSettings();
            Manager.StateManager.SwitchTo(GameState.MainMenu);
        }

        private void SetupMenu()
        {
            savebtn.setSizeRelative(0.15f * (Screen.Mode == Orientation.Portrait ? 2 : 1), Screen.Mode);
            savebtn.Position = new Vector2((Screen.Width - savebtn.Size.X) / 2, Screen.Height - savebtn.BoundingBox.Height);
        }
        public void Draw(SpriteBatch batch)
        {
            menu.Draw(batch);
        }

        public void HandleEvent(WorldEvent e, bool forcehandle = false)
        {
            if (e is DisplaySizeChangedEvent || e is OrientationChangedEvent)
                SetupMenu();
            menu.HandleEvent(e);
        }

        public void OnActivated(params object[] args)
        {
            SetupMenu();
        }

        public void Update(GameTime time)
        {
            menu.Update(time);
        }
    }
}