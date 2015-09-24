using System;
using Microsoft.Xna.Framework;

namespace Inlumino_SHARED
{
    class UIButton : UIVisibleObject
    {
        internal TextureID[] Texture
        {
            get { return sprite; }
            set { sprite = value; }
        }

        internal delegate void ButtonPressedEventHandler(UIButton sender);
        internal event ButtonPressedEventHandler Pressed;

        internal UIButton(TextureID[] tid, ButtonPressedEventHandler pressed = null, int layer = 0, string id = "")
            : base(tid, id, layer)
        {
            Pressed += pressed;
        }

        internal override void Clear()
        {
            base.Clear();
        }

        internal override void HandleEvent(WorldEvent e)
        {
            if (!visible || e.Handled) return;
            foreach (UIObject obj in siblings) obj.HandleEvent(e);
            if (e.Handled) return;
            Vector2 pos;
            if (e is MouseUpEvent)
                pos = (e as MouseUpEvent).Position;
            else if (e is TouchTapEvent)
                pos = (e as TouchTapEvent).Position;
            else return;
            UIGrid p = parent as UIGrid;
            if (p != null) pos = p.Camera.DeTransform(pos);
            if (BoundingBox.ContainsPoint(pos))
            {
                e.Handled = true;
                OnPressed();
                SoundManager.PlaySound(DataHandler.Sounds[SoundType.TapSound], SoundCategory.SFX); ;
            }

            base.HandleEvent(e);
        }

        protected virtual void OnPressed()
        {
            if (Pressed != null) { Pressed(this); }
        }
    }
}
