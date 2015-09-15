using System;
using Microsoft.Xna.Framework;

namespace Inlumino_SHARED
{
    class UIButton : UIVisibleObject
    {
        internal TextureID[] Texture {
            get { return sprite; }
            set { sprite = value; } }

        internal delegate void ButtonPressedEventHandler(UIButton sender);
        internal event ButtonPressedEventHandler Pressed;

        internal UIButton(TextureID[] tid,ButtonPressedEventHandler pressed=null, int layer = 0, string id = "")
            : base(tid, layer, id)
        {
            Pressed += pressed;
        }

        internal override void Clear()
        {
            base.Clear();
        }

        internal override void HandleEvent(WorldEvent e)
        {
            if (e.Handled) return;
            if (visible)
            {
                Vector2 pos;
                if (e is MouseUpEvent)
                    pos = (e as MouseUpEvent).Position.ToVector2();
                else if (e is TouchTapEvent)
                    pos = (e as TouchTapEvent).Position;
                else return;
                UIHud p = parent as UIHud;
                if (p != null) pos = p.Camera.DeTransform(pos);
                if (BoundingBox.ContainsPoint(pos))
                {
                    if (Pressed != null) { Pressed(this); e.Handled = true; }
                    SoundManager.PlaySound(DataHandler.Sounds[SoundType.TapSound], SoundCategory.SFX); ;
                }
            }
            base.HandleEvent(e);
        }
    }
}
