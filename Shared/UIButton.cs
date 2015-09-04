using System;
using Microsoft.Xna.Framework;

namespace Inlumino_SHARED
{
    class UIButton : UIVisibleObject
    {
        public TextureID[] Texture {
            get { return sprite; }
            set { sprite = value; } }

        public delegate void ButtonPressedEventHandler(UIButton sender);
        public event ButtonPressedEventHandler Pressed;

        public UIButton(TextureID[] tid, int layer = 0, string id = "")
            : base(tid, layer, id)
        {
        }

        public override void Clear()
        {
            base.Clear();
        }

        public override void HandleEvent(WorldEvent e)
        {
            if (e.Handled) return;
            if (visible)
            {
                Vector2 pos;
                if (e is MouseDownEvent)
                    pos = (e as MouseDownEvent).Position.ToVector2();
                else if (e is TouchTapEvent)
                    pos = (e as TouchTapEvent).Position;
                else return;
                if (pos.X > GlobalPosition.X && pos.Y > GlobalPosition.Y && pos.X < BoundingBox.Right && pos.Y < BoundingBox.Bottom)
                    if (Pressed != null) { Pressed(this); e.Handled = true; }
            }
            base.HandleEvent(e);
        }
    }
}
