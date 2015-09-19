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
        internal object Tag { get { return tag; } }

        internal UICell(TextureID[] tid, object tag, string text = "", Color textcol = default(Color), TextureID overlay = null, float border = 0, ButtonPressedEventHandler pressed = null, int layer = 0) : base(tid, pressed, layer, tag.ToString())
        {
            this.overlay = overlay;
            this.tag = tag;
            font = DataHandler.Fonts[0];
            color = textcol;
            this.border = border;
            this.text = text;
        }

        internal UICell(TextureID[] tid, object tag, TextureID overlay, float border = 0, string text = "", Color textcol = default(Color), ButtonPressedEventHandler pressed = null, int layer = 0) : base(tid, pressed, layer, tag.ToString())
        {
            this.overlay = overlay;
            this.tag = tag;
            font = DataHandler.Fonts[0];
            color = textcol;
            this.border = border;
            this.text = text;
        }
        internal override void Draw(SpriteBatch batch, Camera cam = null)
        {
            base.Draw(batch, cam);
            if (!visible) return;
            // Draw overlay
            if (DataHandler.isValid(overlay))
            {
                RectangleF rect = LocalBoundingBox.Inflate(-Width * border, -Height * border);
                if (cam == null)
                    batch.Draw(DataHandler.getTexture(overlay.RefKey)/*Texture2D from file*/, cam == null ? rect.ToRectangle() : cam.Transform(rect).ToRectangle()/*on-screen box*/, DataHandler.getTextureSource(overlay)/*Rectange on the sheet*/, Color.White/*white=no tint*/);
                else if (cam.isInsideView(rect))
                {
                    RectangleF nocrop;
                    RectangleF cropped = cam.TransformWithCropping(rect, out nocrop);
                    RectangleF source = DataHandler.getTextureSource(overlay);
                    source = source.Mask(nocrop, cropped);
                    batch.Draw(DataHandler.getTexture(overlay.RefKey)/*Texture2D from file*/, cropped.Offset(parent.GlobalPosition).ToRectangle()/*on-screen box*/, source.ToRectangle()/*Rectange on the sheet*/, Color.White/*white=no tint*/);
                }
            }
            // Siblings
            foreach (UIVisibleObject obj in siblings.Where(t => t is UIVisibleObject)) obj.Draw(batch, cam);
            // Children
            foreach (UIVisibleObject obj in children.Where(t => t is UIVisibleObject)) obj.Draw(batch, cam);
            // Draw text                        
            Vector2 tsize = font.MeasureString(text);
            if (cam == null)
                batch.DrawString(font, text, Center - tsize / 2 + parent.GlobalPosition, color);
            else
            {
                Vector2 pos = LocalBoundingBox.Center - tsize / 2;
                if (cam.isInsideView(pos))
                    batch.DrawString(font, text, cam.Transform(pos) + parent.GlobalPosition, color);
            }
        }
    }
}
