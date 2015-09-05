using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Inlumino_SHARED
{
    class SaveMenu : IState
    {
        protected UIButton savebtn, backbtn;
        protected UITextField nametext;
        protected UIMenu menu;

        public SaveMenu()
        {
            menu = new UIMenu();

            nametext = new UITextField(16, Color.White, Color.Blue, "Enter Name Here");
            savebtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.SaveButton]);
            backbtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.BackButton]);

            savebtn.Pressed += savepressed;
            backbtn.Pressed += backpressed;

            menu.Add(savebtn);
            menu.Add(backbtn);
            menu.Add(nametext);

            VirtualKeyboard.KeyPressed += keypressed;
        }

        private void backpressed(UIButton sender)
        {
            Manager.StateManager.SwitchTo(GameState.EditMode);
        }

        private async void savepressed(UIButton sender)
        {
            if (nametext.Text.Length == 0)
            {
                await MessageBox.Show("Warning", "Please enter at least 1 character in the name field.", new string[] { "OK" });
                return;
            }
            if (DataHandler.LevelExists(nametext.Text))
            {
                int? r = await MessageBox.Show("Warning", "Level \"" + nametext.Text + "\" already exists.", new string[] { "Cancel", "Overwrite", });
                if (r == 0 || r == null)
                    return;
            }
            DataHandler.SaveStage((Manager.StateManager.GetGameState(GameState.EditMode) as StageContainer).getCurrentStage(), nametext.Text, icon);
            Manager.StateManager.SwitchTo(GameState.MainMenu);
        }

        private void SetupMenu()
        {
            nametext.setSizeRelative(0.2f, Screen.Mode);
            savebtn.setSizeRelative(0.15f * (Screen.Mode == Orientation.Portrait ? 2 : 1), Screen.Mode);
            backbtn.setSizeRelative(0.15f * (Screen.Mode == Orientation.Portrait ? 2 : 1), Screen.Mode);
            VirtualKeyboard.Show(Orientation.Landscape, 0.1f * Screen.BigDim, Screen.Width, Screen.Height, k => ((int)k >= 65 && (int)k <= 90) || k == Keys.Back || k == Keys.LeftShift);
            nametext.Position = new Vector2((Screen.Width - nametext.Size.X) / 2, VirtualKeyboard.BoundingBox.Bottom);
            savebtn.Position = new Vector2((Screen.Width - savebtn.Size.X) / 2, nametext.BoundingBox.Bottom);
            backbtn.Position = new Vector2((Screen.Width - backbtn.Size.X) / 2, savebtn.BoundingBox.Bottom);
        }

        private void keypressed(Keys key)
        {
            if (key == Keys.Back)
                nametext.Text = nametext.Text.Substring(0, MathHelper.Max(0, nametext.Text.Length - 1));
            else if (key != Keys.LeftShift)
                nametext.Text += (VirtualKeyboard.Low ? key.ToString().ToLower() : key.ToString());
        }

        public void Update(GameTime time)
        {
            VirtualKeyboard.Update(time);
            menu.Update(time);
        }

        public void Draw(SpriteBatch batch)
        {
            VirtualKeyboard.Draw(batch);
            menu.Draw(batch);
        }

        public void HandleEvent(WorldEvent e, bool forcehandle = false)
        {
            if (e is DisplaySizeChangedEvent || e is OrientationChangedEvent)
                SetupMenu();
            VirtualKeyboard.HandleEvent(e);
            menu.HandleEvent(e);
        }
        Texture2D icon = null;
        public void OnActivated(params object[] args)
        {
            if (args.Length > 0) icon = args[0] as Texture2D;
            SetupMenu();
        }
    }
}

