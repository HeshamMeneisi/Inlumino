﻿using System;
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

        internal SaveMenu()
        {
            menu = new UIMenu();
            nametext = new UITextField(10, Color.White, Color.Black, "Enter Name Here");
            nametext.AllowedCharTypes = CharType.Lower | CharType.Upper;
            savebtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.SaveButton], savepressed);
            backbtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.BackButton], backpressed);

            //menu.Add(nametext);
            menu.Add(savebtn);
            menu.Add(backbtn);
        }

        private void backpressed(UIButton sender)
        {
            Manager.StateManager.SwitchBack();
        }

        private async void savepressed(UIButton sender)
        {
            if (nametext.Text.Length == 0)
            {
                await AlertHandler.ShowMessage("Warning", "Please enter at least 1 character in the name field.", new string[] { "OK" });
                return;
            }
            if (DataHandler.LevelExists(nametext.Text))
            {
                int? r = await AlertHandler.ShowMessage("Warning", "Level \"" + nametext.Text + "\" already exists.", new string[] { "Cancel", "Overwrite", });
                if (r == 0 || r == null)
                    return;
            }
            DataHandler.SaveStage(stage, nametext.Text);
            Manager.StateManager.SwitchTo(GameState.MainMenu);
        }

        private void SetupMenu()
        {
            nametext.Size = new Vector2(Screen.Width, Screen.Height * 0.2f);
            menu.setAllSizeRelative(0.2f, Orientation.Landscape);
            menu.ArrangeInForm(Orientation.Landscape);
            nametext.Position = new Vector2((Screen.Width - nametext.Width) / 2, 0);
            menu.Position = new Vector2((Screen.Width - menu.Width) / 2, nametext.BoundingBox.Bottom);
        }

        public void Update(GameTime time)
        {
            nametext.Update(time);
            menu.Update(time);
        }

        public void Draw(SpriteBatch batch)
        {
            menu.Draw(batch);
            nametext.Draw(batch);
        }

        public void HandleEvent(WorldEvent e, bool forcehandle = false)
        {
            if (e is DisplaySizeChangedEvent || e is OrientationChangedEvent)
                SetupMenu();
            menu.HandleEvent(e);
            nametext.HandleEvent(e);
        }
        Stage stage = null;
        public void OnActivated(params object[] args)
        {
            if (args.Length == 0) Manager.StateManager.SwitchTo(GameState.MainMenu);
            stage = args[0] as Stage;
            if (args.Length > 1) nametext.Text = args[1].ToString();
            SetupMenu();
        }
    }
}

