using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace Inlumino_SHARED
{
    class UITextField : UIButton
    {
        static List<UITextField> active = new List<UITextField>();
        private string text = "";
        private string deftext;
        private int maxl;
        private SpriteFont font;
        private Color color;
        private Color background;
        private bool selected = false;
        internal string Padding = "";
        internal char HashChar = '#';
        internal bool IsPassword = false;
        internal CharType AllowedCharTypes = CharType.Lower | CharType.Upper | CharType.Num | CharType.Symb;
        bool vk = false;
        internal override Vector2 Position
        {
            get
            {
                return base.Position;
            }

            set
            {
                base.Position = value;
                if (vk) VirtualKeyboard.NotifyPosChanged();
            }
        }
        internal Color ForegroundColor { get { return color; } set { color = value; } }
        internal Color BackgroundColor { get { return background; } set { background = value; } }
        internal bool Selected
        {
            get { return selected; }
            set
            {
                if (value == selected) return;
                selected = value; OnSelectedChanged();
            }
        }

        private void OnSelectedChanged()
        {
            if (SelectedChanged != null) SelectedChanged(this, Selected);
            if (selected)
            {
                vk = true; VirtualKeyboard.Show(this);
            }
            else vk = false;
        }

        internal delegate void SelectedChangedEventHandler(UITextField sender, bool state);
        internal event SelectedChangedEventHandler SelectedChanged;
        internal string Text
        {
            get { return text; }
            set { if (value != null) text = value.Substring(0, MathHelper.Min(value.Length, maxl)); }
        }
        internal UITextField(int maxl, Color col, Color background, string defaulttext = "", int layer = 0, string id = "", ButtonPressedEventHandler pressed = null) : base(null, pressed, layer, id)
        {
            this.maxl = maxl;
            this.font = DataHandler.Fonts[0];
            this.color = col;
            this.deftext = defaulttext;
            this.background = background;
            ScaleToDefault();
            active.Add(this);
        }
        protected override void OnPressed()
        {
            UnselectAll(); Selected = true;
            base.OnPressed();
        }

        internal static void UnselectAll()
        {
            foreach (UITextField f in active) f.Selected = false;
        }
        internal static UITextField GetSelected()
        {
            return active.FirstOrDefault(t => t.Selected);
        }
        internal override void Draw(SpriteBatch batch, Camera cam = null)
        {
            if (!visible) return;
            // background
            UpdateBackground();
            batch.Draw(rect, BoundingBox.ToRectangle(), Color.White);
            //
            string t = Padding + GetTextToDraw();

            Vector2 tsize = font.MeasureString(t);
            if (!Selected)
                while (tsize.X > Size.X && t != "")
                {
                    t = t.Substring(0, t.Length - 1);
                    tsize = font.MeasureString(t);
                }
            else
                while (tsize.X > size.X && t.Length > 1)
                {
                    t = t.Substring(1, t.Length - 1);
                    tsize = font.MeasureString(t);
                }
            batch.DrawString(font, t, cam == null ? this.GlobalCenter - tsize / 2 : cam.Transform(this.GlobalCenter - tsize / 2), text == "" ? Color.Gray : color);
            base.Draw(batch, cam);
        }

        private string GetTextToDraw()
        {
            if (text == "") return deftext;
            else if (IsPassword)
                return string.Join("", Enumerable.Repeat(HashChar, text.Length - (Selected ? 1 : 0))) + (Selected ? text[text.Length - 1].ToString() : "");
            else return text;
        }
        Texture2D rect = null;
        private void UpdateBackground()
        {
            int w = (int)Width, h = (int)Height;
            if (w < 1 || h == 1) return;
            if (rect != null && rect.Width == w && rect.Height == h) return;
            rect = new Texture2D(Manager.Parent.GraphicsDevice, w, h);
            Color[] data = new Color[w * h];
            for (int i = 0; i < data.Length; ++i) data[i] = background;
            rect.SetData(data);
        }

        internal void ScaleToDefault(bool trimtoscreen = true)
        {
            if (deftext == "") size = new Vector2(1, 1);
            Vector2 tsize = font.MeasureString(deftext);
            if (trimtoscreen)
            {
                tsize.X = Math.Min(tsize.X, Screen.Width - GlobalPosition.X);
                tsize.Y = Math.Min(tsize.Y, Screen.Height - GlobalPosition.Y);
            }
            Size = tsize;
        }
        internal void ScaleToText(bool trimtoscreen = true)
        {
            string t = Padding + (text == "" ? deftext : IsPassword ? string.Join("", Enumerable.Repeat(HashChar, text.Length)) : text);
            Vector2 tsize = font.MeasureString(t);
            if (trimtoscreen)
            {
                tsize.X = Math.Min(tsize.X, Screen.Width - GlobalPosition.X);
                tsize.Y = Math.Min(tsize.Y, Screen.Height - GlobalPosition.Y);
            }
            Size = new Vector2(Math.Max(tsize.X, size.X), Math.Max(tsize.Y, size.Y));
        }
        internal override void Update(GameTime time)
        {
            base.Update(time);
        }
        internal override void HandleEvent(WorldEvent e)
        {
            if (!visible) return;
            base.HandleEvent(e);
            if (e.Handled) return;
            if (!selected) return;
            if (e is KeyDownEvent)
            {
                Keys k = (e as KeyDownEvent).Key;
                if (k == Keys.Back || k == Keys.Delete)
                    Input((char)8);
                else if (CommonData.KeyCharMap.Keys.Contains(k))
                    Input(CommonData.KeyCharMap[k][InputManager.isKeyDown(Keys.LeftShift) || InputManager.isKeyDown(Keys.RightShift) ? 1 : 0]);
            }
        }
        internal void NotifyVKExit()
        {
            vk = false;
            Selected = false;
        }
        internal void Input(char c)
        {
            if (c == 8 || c == 127)
                text = text.Substring(0, MathHelper.Max(0, text.Length - 1));
            else if (
               (c >= 65 && c <= 90 && (AllowedCharTypes & CharType.Upper) > 0)
            || (c >= 97 && c <= 122 && (AllowedCharTypes & CharType.Lower) > 0)
            || (c >= 48 && c <= 57 && (AllowedCharTypes & CharType.Num) > 0)
            || (c == 32 && (AllowedCharTypes & CharType.Space) > 0)
            || (((c >= 33 && c <= 47) || (c >= 58 && c <= 64) || (c >= 91 && c <= 96) || (c >= 123 && c <= 126)) && (AllowedCharTypes & CharType.Symb) > 0)
            )
            {
                if (text.Length < maxl)
                    text += c;
            }
        }

        internal override void setSizeRelative(float perc, Orientation mode)
        {
            base.setSizeRelative(perc, mode);
        }
    }
    [Flags]
    internal enum CharType { Upper = 1, Lower = 2, Num = 4, Symb = 8, Space = 16 }
}