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
        Camera cam;

        public float TotalWidth { get { return mode == Orientation.Landscape ? unitwidth * cellsperrowcol : unitwidth * rowcolcount; } }
        public float TotalHeight { get { return mode == Orientation.Portrait ? unitheight * cellsperrowcol : unitheight * rowcolcount; } }

        public override RectangleF BoundingBox
        {
            get
            {
                return new RectangleF(GlobalPosition, new Vector2(TotalWidth, TotalHeight));
            }
        }
        public UIHud(IEnumerable<UIButton> content, Orientation mode, float minuwidth, float minuheight, float maxwidth, float maxheight)
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
            float cw, ch;
            cam = new Camera(0, 0, TotalWidth, TotalHeight, TotalWidth, TotalHeight);
            if (mode == Orientation.Landscape)
            {
                do
                {
                    unitwidth = maxwidth / c;
                    c--;
                }
                while (unitwidth < minuwidth);
                unitheight = unitwidth / aspect;
                cw = TotalWidth; ch = TotalHeight;
                ch = TotalHeight * cw / TotalWidth;
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
                cw = TotalWidth; ch = TotalHeight;
                cw = TotalWidth * ch / TotalHeight;
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
            cam = new Camera(0, 0, cw, ch, TotalWidth, TotalHeight);
            cam.FitToScreen = false;
        }

        public override void Draw(SpriteBatch batch, Camera cam = null)
        {
            if (visible)
                if (buttons.Count > 0)
                    foreach (UIButton b in buttons) b.Draw(batch,cam); //TODO: Implement camera here to shift position
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
            {
                foreach (UIButton b in buttons) b.HandleEvent(e);
                if (e is TouchFreeDragEvent)
                {
                    Vector2 delta = (e as TouchFreeDragEvent).Delta;
                    cam.StepHorizontal(-delta.X);
                    cam.StepVertical(-delta.Y);
                }

                if (e is MouseMovedEvent)
                {                    
                    if (InputManager.isMouseDown(InputManager.MouseKey.LeftKey))
                    {
                        Point offset = (e as MouseMovedEvent).Offset;
                        cam.StepHorizontal(offset.X);
                        cam.StepVertical(offset.Y);
                    }
                }
            }
        }
    }
}
public enum Orientation { Landscape, Portrait }
