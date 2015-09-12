using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Inlumino_SHARED
{
    class UICell : UIButton
    {
        object tag;
        SpriteFont font;
        Color color;
        TextureID overlay;
        float border;
        string text = "";
        public object Tag { get { return tag; } }

        public UICell(TextureID[] tid, object tag, string text = "", Color textcol = default(Color), TextureID overlay = null, float border = 0, ButtonPressedEventHandler pressed = null, int layer = 0) : base(tid, pressed, layer, tag.ToString())
        {
            this.overlay = overlay;
            this.tag = tag;
            font = DataHandler.Fonts[0];
            color = textcol;
            this.border = border;
            this.text = text;
        }

        public UICell(TextureID[] tid, object tag, TextureID overlay = null, float border = 0, string text = "", Color textcol = default(Color), ButtonPressedEventHandler pressed = null, int layer = 0) : base(tid, pressed, layer, tag.ToString())
        {
            this.overlay = overlay;
            this.tag = tag;
            font = DataHandler.Fonts[0];
            color = textcol;
            this.border = border;
            this.text = text;
        }
        public override void Draw(SpriteBatch batch, Camera cam = null)
        {
            base.Draw(batch, cam);
            if (!visible) return;
            // Draw overlay
            if (DataHandler.isValid(overlay))
            {
                RectangleF rect = LocalBoundingBox.Inflate(-Width * border, -Height * border);
                if (cam == null)
                    batch.Draw(DataHandler.getTexture(overlay.GroupIndex)/*Texture2D from file*/, cam == null ? rect.ToRectangle() : cam.Transform(rect).ToRectangle()/*on-screen box*/, DataHandler.getTextureSource(overlay)/*Rectange on the sheet*/, Color.White/*white=no tint*/);
                else if (cam.isInsideView(rect))
                {
                    RectangleF nocrop;
                    RectangleF cropped = cam.TransformWithCropping(rect, out nocrop);
                    RectangleF source = DataHandler.getTextureSource(overlay);
                    source = source.Mask(nocrop, cropped);
                    batch.Draw(DataHandler.getTexture(overlay.GroupIndex)/*Texture2D from file*/, cropped.Offset(parent.GlobalPosition).ToRectangle()/*on-screen box*/, source.ToRectangle()/*Rectange on the sheet*/, Color.White/*white=no tint*/);
                }
            }
            // Children
            foreach (UIVisibleObject obj in siblings) obj.Draw(batch, cam);
            // Draw text
            Vector2 tsize = font.MeasureString(text);
            batch.DrawString(font, text, this.Center - tsize / 2, color);
        }

        internal void CentralizeSiblings()
        {
            foreach (UIVisibleObject obj in siblings)
                obj.Position = new Vector2(position.X + (Width - obj.Width) / 2, position.Y + (Height - obj.Height) / 2);
        }

        public void AttachSibling(UIVisibleObject child)
        {
            child.Parent = Parent;
            siblings.Add(child);
        }

        internal void FitSiblings()
        {
            foreach (UIVisibleObject obj in siblings)
                if (obj.Width > obj.Height)
                {
                    float h = Width * obj.Height / obj.Width,
                          w = Width;
                    obj.Size = new Vector2(w, h);
                }
                else
                {
                    float w = Height * obj.Width / obj.Height,
                          h = Height;
                    obj.Size = new Vector2(w, h);
                }

        }
    }
}
