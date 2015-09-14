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
        List<UICell> cells = new List<UICell>();
        Orientation mode;
        Camera cam;
        UICell snaptarget = null;
        public float ActualWidth { get { return Math.Min(TotalWidth, maxwidth); } }
        public float ActualHeight { get { return Math.Min(TotalHeight, maxheight); } }

        public float TotalWidth { get { return mode == Orientation.Landscape ? unitwidth * cellsperrowcol : unitwidth * rowcolcount; } }
        public float TotalHeight { get { return mode == Orientation.Portrait ? unitheight * cellsperrowcol : unitheight * rowcolcount; } }

        public bool SnapCameraToCells = true;

        public UICell SnapTarget { get { return snaptarget; } set { snaptarget = value; cam.EnsureVisible(snaptarget.LocalBoundingBox); } }

        public override RectangleF BoundingBox
        {
            get
            {
                return new RectangleF(GlobalPosition, new Vector2(ActualWidth, ActualHeight));
            }
        }

        public Camera Camera { get { return cam; } }

        public UIHud(IEnumerable<UICell> content, Orientation mode, float minuwidth, float minuheight, float maxwidth, float maxheight)
        {
            cells.AddRange(content);
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
            int c = cells.Count;
            float cw = maxwidth, ch = maxheight;
            if (mode == Orientation.Landscape)
            {
                do
                {
                    unitwidth = maxwidth / c;
                    c--;
                }
                while (unitwidth < minuwidth);
                unitheight = unitwidth / aspect;
                ch = ActualHeight * cw / ActualWidth;
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
                cw = ActualWidth * ch / ActualHeight;
            }
            cellsperrowcol = c + 1;
            rowcolcount = (int)Math.Ceiling((float)cells.Count / cellsperrowcol);
            int i = 0;
            float x = 0, y = 0;
            foreach (UICell cell in cells)
            {
                cell.Size = new Vector2(unitwidth, unitheight);
                cell.Position = new Vector2(x, y);
                cell.CentralizeSiblings();
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

        public void Draw(SpriteBatch batch)
        {
            if (visible)
                if (cells.Count > 0)
                    foreach (UIButton b in cells) b.Draw(batch, cam);
                else
                {
                    string s = "Nothing to show.";
                    Vector2 size = DataHandler.Fonts[0].MeasureString(s);
                    batch.DrawString(DataHandler.Fonts[0], s, new Vector2(maxwidth / 2, maxheight / 2) - size / 2 + GlobalPosition, Color.White);
                }
        }

        public override void Update(GameTime time)
        {
            if (visible)
            {
                cam.Update(time);
                foreach (UIButton b in cells) b.Update(time);
            }
        }
        bool dragging = true;
        public override void HandleEvent(WorldEvent e)
        {
            if (visible)
            {
                if (e is MouseUpEvent && dragging) { dragging = false; if(SnapCameraToCells)SnapCam(); return; }
                foreach (UICell b in cells) b.HandleEvent(e);
                if (e is TouchFreeDragEvent)
                {
                    TouchFreeDragEvent ev = (e as TouchFreeDragEvent);
                    if (!BoundingBox.ContainsPoint(ev.Postion)) goto skip;
                    Vector2 delta = ev.Delta;
                    cam.StepHorizontal(-delta.X);
                    cam.StepVertical(-delta.Y);
                skip:;
                }
                if (e is MouseMovedEvent)
                {
                    if (InputManager.isMouseDown(InputManager.MouseKey.LeftKey))
                    {
                        if (!dragging && !BoundingBox.ContainsPoint((e as MouseMovedEvent).Position.ToVector2())) return;
                        dragging = true;
                        Point offset = (e as MouseMovedEvent).Offset;
                        cam.StepHorizontal(offset.X);
                        cam.StepVertical(offset.Y);
                    }
                }
                if (e is TouchAllFingersOffEvent && SnapCameraToCells)
                    SnapCam();
            }
        }

        private void SnapCam()
        {
            snaptarget = null;
            float intersize = 0;
            foreach (UICell cell in cells)
            {
                if (cell.LocalBoundingBox.Intersects(cam.TargetView))
                {
                    float cis = cell.LocalBoundingBox.Intersection(cam.TargetView).Area;
                    if (cis > intersize)
                    {
                        intersize = cis;
                        snaptarget = cell;
                    }
                }
            }
            if (snaptarget != null)
                cam.EnsureVisible(snaptarget.LocalBoundingBox);
        }

        internal void FitCellSiblings()
        {
            foreach (UICell cell in cells)
            { cell.FitSiblings(); cell.CentralizeSiblings(); }
        }
    }
}
public enum Orientation { Landscape, Portrait }
