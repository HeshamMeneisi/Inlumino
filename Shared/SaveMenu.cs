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

            nametext.Size = new Vector2(nametext.Width, nametext.Height * 0.5f);
            //menu.Add(nametext);
            menu.Add(savebtn);
            menu.Add(backbtn);            

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
            DataHandler.SaveStage(stage, nametext.Text);
            Manager.StateManager.SwitchTo(GameState.MainMenu);
        }

        private void SetupMenu()
        {
            VirtualKeyboard.Show(Orientation.Landscape, 0.1f * Screen.BigDim, Screen.Width, Screen.Height, k => ((int)k >= 65 && (int)k <= 90) || k == Keys.Back || k == Keys.LeftShift);
            menu.setAllSizeRelative(0.3f * VirtualKeyboard.BoundingBox.Height/Screen.Height, Screen.Mode);
            menu.ArrangeInForm(Screen.Mode);
            nametext.setSizeRelative(0.3f * VirtualKeyboard.BoundingBox.Height / Screen.Height, Orientation.Landscape);
            nametext.Position = new Vector2((Screen.Width - nametext.Width) / 2, VirtualKeyboard.BoundingBox.Bottom);
            menu.Position = new Vector2((Screen.Width - menu.Width) / 2, nametext.BoundingBox.Bottom);
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
            nametext.Update(time);
            menu.Update(time);
        }

        public void Draw(SpriteBatch batch)
        {
            VirtualKeyboard.Draw(batch);
            nametext.Draw(batch);
            menu.Draw(batch);
        }

        public void HandleEvent(WorldEvent e, bool forcehandle = false)
        {
            if (e is DisplaySizeChangedEvent || e is OrientationChangedEvent)
                SetupMenu();
            VirtualKeyboard.HandleEvent(e);
            menu.HandleEvent(e);
            nametext.HandleEvent(e);
        }
        Stage stage = null;
        public void OnActivated(params object[] args)
        {
            if (args.Length == 0) Manager.StateManager.SwitchTo(GameState.MainMenu);
            stage = args[0] as Stage;
            SetupMenu();
        }
    }
}

