using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Inlumino_SHARED
{
    class UITextField : UIVisibleObject
    {
        private string text = "";
        private string deftext;
        private int maxl;
        private SpriteFont font;
        private Color color;
        private Color background;

        public string Text
        {
            get { return text; }
            set { text = value.Substring(0, MathHelper.Min(value.Length, maxl)); }
        }
        public UITextField(int maxl, Color col, Color background, string defaulttext = "", int layer = 0, string id = "") : base(DataHandler.ObjectTextureMap[ObjectType.Invisible], layer, id)
        {
            this.maxl = maxl;
            this.font = DataHandler.Fonts[0];
            this.color = col;
            this.deftext = defaulttext;
            this.background = background;
        }
        public override void Draw(SpriteBatch batch, Camera cam = null)
        {
            Vector2 tsize = font.MeasureString(text == "" ? deftext : text);
            batch.DrawString(font, text == "" ? deftext : text, cam == null ? this.Center - tsize / 2 : cam.Transform(this.Center - tsize / 2), text == "" ? Color.Gray : color);
            base.Draw(batch, cam);
        }
        public override void HandleEvent(WorldEvent e)
        {
            if (e is KeyDownEvent)
            {
                Keys k = (e as KeyDownEvent).Key;
                if (k == Keys.Back || k == Keys.Delete)
                    text = text.Substring(0, MathHelper.Max(0, text.Length - 1));
                else if ((int)k >= 65 && (int)k <= 90)
                {
                    if (text.Length < maxl)
                        text += InputManager.isKeyDown(Keys.LeftShift) || InputManager.isKeyDown(Keys.RightShift) ? k.ToString() : k.ToString().ToLower();
                }
            }
        }
    }
}