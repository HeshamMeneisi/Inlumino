using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlumino_SHARED
{
    class UIHud : UIObject
    {
        float minuwidth, minuheight, maxwidth, maxheight, unitwidth, unitheight, aspect;
        int cellsperrowcol, rowcolcount;
        List<UIButton> buttons = new List<UIButton>();
        Orientation mode;

        public float TotalWidth { get { return mode == Orientation.Landscape ? unitwidth * cellsperrowcol : unitwidth * rowcolcount; } }
        public float TotalHeight { get { return mode == Orientation.Portrait ? unitheight * cellsperrowcol : unitheight * rowcolcount; } }

        public override RectangleF BoundingBox
        {
            get
            {
                return new RectangleF(GlobalPosition, new Vector2(TotalWidth, TotalHeight));
            }
        }
        public UIHud(UIButton[] content, Orientation mode, float minuwidth, float minuheight, float maxwidth, float maxheight)
        {
            buttons.AddRange(content);
            foreach (UIButton b in content) b.Parent = this;
            this.mode = mode;
            this.minuwidth = minuwidth;
            this.minuheight = minuheight;
            this.maxwidth = maxwidth;
            this.maxheight = maxheight;
            this.aspect = minuwidth / minuheight;
            Setup();
        }

        public void Setup()
        {
            int c = buttons.Count;
            if (mode == Orientation.Landscape)
            {
                do
                {
                    unitwidth = maxwidth / c;
                    c--;
                }
                while (unitwidth < minuwidth);
                unitheight = unitwidth / aspect;
            }
            else
            {
                do
                {
                    unitheight = maxheight / c;
                    c--;
                }
                while (unitheight < minuheight);
                unitwidth = unitheight * aspect;
            }
            cellsperrowcol = c + 1;
            rowcolcount = (int)Math.Ceiling((float)buttons.Count / cellsperrowcol);
            int i = 0;
            float x = 0, y = 0;
            foreach (UIButton b in buttons)
            {
                b.Size = new Vector2(unitwidth, unitheight);
                b.Position = new Vector2(x, y);
                i++;
                if (i == cellsperrowcol)
                {
                    i = 0;
                    x = mode == Orientation.Portrait ? x + unitwidth : 0;
                    y = mode == Orientation.Portrait ? 0 : y + unitheight;
                }
                else
                {
                    x += mode == Orientation.Portrait ? 0 : unitwidth;
                    y += mode == Orientation.Portrait ? unitheight : 0;
                }
            }
        }

        public override void Draw(SpriteBatch batch)
        {
            if (visible)
                if (buttons.Count > 0)
                    foreach (UIButton b in buttons) b.Draw(batch);
                else
                {
                    string s = "Nothing to show.";
                    Vector2 size = DataHandler.Fonts[0].MeasureString(s);
                    batch.DrawString(DataHandler.Fonts[0], s, new Vector2(maxwidth / 2, maxheight / 2) - size/2 + GlobalPosition, Color.White);
                }
        }

        public override void Update(GameTime time)
        {
            if (visible)
                foreach (UIButton b in buttons) b.Update(time);
        }

        public override void HandleEvent(WorldEvent e)
        {
            if (visible)
                foreach (UIButton b in buttons) b.HandleEvent(e);
        }
    }
}
public enum Orientation { Landscape, Portrait }
