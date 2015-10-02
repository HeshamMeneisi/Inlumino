using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inlumino_SHARED
{
    class FBButton:UIButton
    {
        public FBButton() : base(DataHandler.UIObjectsTextureMap[UIObjectType.FBBtn]) {
            Manager.StateManager.StateChanged += statechanged;
            SetPos();
        }

        private void SetPos()
        {
            Position = new Vector2(Screen.Width - Width, Screen.Height - Height);
        }

        private void statechanged(GameState obj)
        {
            Visible = !(obj == GameState.EditMode || obj == GameState.OnStage);
        }

        protected override void OnPressed()
        {
            base.OnPressed();
            Common.HandleFacebookPressed();
        }

        internal override void HandleEvent(WorldEvent e)
        {
            base.HandleEvent(e);
            if (e is DisplaySizeChangedEvent || e is OrientationChangedEvent)
                SetPos();
        }
    }
}
