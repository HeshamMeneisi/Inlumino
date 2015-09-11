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

        public UICell(TextureID[] tid, object tag, string text = "", Color textcol = default(Color), TextureID overlay = null, float border = 0, int layer = 0) : base(tid, layer, tag.ToString())
        {
            this.overlay = overlay;
            this.tag = tag;
            font = DataHandler.Fonts[0];
            color = textcol;
            this.border = border;
            this.text = text;
        }

        public UICell(TextureID[] tid, object tag, TextureID overlay = null, float border = 0, string text = "", Color textcol = default(Color), int layer = 0) : base(tid, layer, tag.ToString())
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
            base.Draw(batch);
            if (!visible) return;
            // Draw overlay
            if (DataHandler.isValid(overlay))
            {
                Rectangle rect = BoundingBox.getRectangle();
                rect.Inflate(-this.Size.X * border, -this.Size.Y * border);
                batch.Draw(DataHandler.getTexture(overlay.GroupIndex)/*Texture2D from file*/, cam == null ? rect : cam.Transform(rect)/*on-screen box*/, DataHandler.getTextureSource(overlay)/*Rectange on the sheet*/, Color.White/*white=no tint*/);
            }
            // Draw text
            Vector2 tsize = font.MeasureString(text);
            batch.DrawString(font, text, this.Center - tsize / 2, color);
        }
    }
}
