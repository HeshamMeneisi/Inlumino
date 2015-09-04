using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inlumino_SHARED
{
    class DeleteMenu : LevelSelector
    {
        public DeleteMenu() : base()
        {
            switchbtn.Visible = false;
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
    }
}
