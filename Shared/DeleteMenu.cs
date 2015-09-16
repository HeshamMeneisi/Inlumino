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
            backbtn.Visible = false;
            menubtn.Texture = DataHandler.UIObjectsTextureMap[UIObjectType.BackButton];
        }

        protected override void SetupHud()
        {
            base.SetupHud();
            mainlevels.Visible = false;          
        }
        protected override void menupressed(UIButton sender)
        {
            Manager.StateManager.SwitchTo(GameState.EditMode);
        }
    }
}
