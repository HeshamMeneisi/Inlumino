using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inlumino_SHARED
{
    class DeleteMenu : LevelSelector
    {
        internal DeleteMenu() : base()
        {
            switchbtn.Visible = false;
            menubtn.Texture = DataHandler.UIObjectsTextureMap[UIObjectType.BackButton];
        }

        protected override void SetupHud()
        {
            base.SetupHud();
            mainlevels.Visible = false;
            userlevels.Visible = true;            
        }
        protected override async void ulcellpressed(UIButton sender)
        {
            string name = (string)(sender as UICell).Tag;
            int? r = await MessageBox.Show("Warning", "Are you sure you want to permanently delete " + name + " ?", new string[] { "Cancel", "Delete" });
            if (r == 1)
            {
                DataHandler.DeleteStage(name);
                SetupHud();
            }
        }
        protected override void menupressed(UIButton sender)
        {
            Manager.StateManager.SwitchTo(GameState.EditMode);
        }
    }
}
