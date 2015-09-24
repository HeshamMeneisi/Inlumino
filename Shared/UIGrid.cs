using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlumino_SHARED
{
    class UIGrid : UIVisibleObject
    {
        float maxwidth, maxheight, unitwidth, unitheight;
        bool dragging = false;
        int cellsperrowcol, rowcolcount;
        List<UIVisibleObject> cells = new List<UIVisibleObject>();
        Orientation mode;
        Camera cam;
        UIVisibleObject snaptarget = null;

        internal List<UIVisibleObject> Cells { get { return cells; } }
        internal override float Width { get { return Math.Min(TotalWidth, maxwidth); } }
        internal override float Height { get { return Math.Min(TotalHeight, maxheight); } }

        internal float TotalWidth { get { return mode == Orientation.Landscape ? unitwidth * cellsperrowcol : unitwidth * rowcolcount; } }
        internal float TotalHeight { get { return mode == Orientation.Portrait ? unitheight * cellsperrowcol : unitheight * rowcolcount; } }

        internal bool SnapCameraToCells = false;

        internal UIVisibleObject SnapTarget { get { return snaptarget; } set { snaptarget = value; if (snaptarget != null) cam.EnsureVisible(snaptarget.LocalBoundingBox); } }

        internal Camera Camera { get { return cam; } }

        internal override Vector2 Size { get { return new Vector2(Width, Height); } }

        internal bool LockCamera = false;

        internal UIGrid(IEnumerable<UIVisibleObject> content, Orientation drawingmode, float maxwidth, float maxheight, int cellsperrowcol, bool setup = false, float unitwidth = 256, float unitheight = 256) : base(null)
        {
            cells.AddRange(content);
            foreach (UIVisibleObject b in content) b.Parent = this;
            this.mode = drawingmode;
            this.maxheight = maxheight;
            this.maxwidth = maxwidth;
            this.unitwidth = unitwidth;
            this.unitheight = unitheight;
            this.cellsperrowcol = cellsperrowcol;
            rowcolcount = (int)Math.Ceiling((float)cells.Count / cellsperrowcol);
            if (setup) Setup();
        }

        internal void Setup()
        {
            int i = 0;
            float x = 0, y = 0;
            foreach (UIVisibleObject cell in cells)
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
            cam = new Camera(0, 0, maxwidth, maxheight, TotalWidth, TotalHeight);
            cam.FitToScreen = false;
        }
        internal void ShowEntireRowCol()
        {
            float temp;
            if (mode == Orientation.Landscape)
            {
                temp = unitwidth; unitwidth = maxwidth / cellsperrowcol;
                unitheight = unitheight * unitwidth / temp;
            }
            else
            {
                temp = unitheight; unitheight = maxheight / cellsperrowcol;
                unitwidth = unitwidth * unitheight / temp;
            }
            Setup();
        }
        internal override void Draw(SpriteBatch batch, Camera cam = null)
        {
            if (!visible) return;

            if (cells.Count > 0)
                foreach (UIVisibleObject b in cells) b.Draw(batch, this.cam);   //TODO: Implement camera inside camera scenario (Camera.Parent)
            else
            {
                string s = "Nothing to show.";
                Vector2 size = DataHandler.Fonts[0].MeasureString(s);
                batch.DrawString(DataHandler.Fonts[0], s, new Vector2(maxwidth / 2, maxheight / 2) - size / 2 + GlobalPosition, Color.White);
            }
        }
        internal void ScaleAllRelative(float p, Orientation mode)
        {
            foreach (UIVisibleObject obj in children) obj.setSizeRelative(p, mode);
        }
        internal override void Update(GameTime time)
        {
            if (!visible) return;

            cam.Update(time);
            foreach (UIVisibleObject b in cells) b.Update(time);

        }

        internal override void HandleEvent(WorldEvent e)
        {
            if (!visible || e.Handled) return;

            if (e is MouseUpEvent && dragging)
            { e.Handled = true; dragging = false; if (SnapCameraToCells) SnapCam(); return; }
            foreach (UIVisibleObject b in cells) b.HandleEvent(e);
            if (e is TouchFreeDragEvent)
            {
                e.Handled = true;
                if (LockCamera) return;
                TouchFreeDragEvent ev = (e as TouchFreeDragEvent);
                if (!BoundingBox.ContainsPoint(ev.Postion)) goto skip;
                Vector2 delta = ev.Delta;
                cam.StepHorizontal(-delta.X);
                cam.StepVertical(-delta.Y);
                return;
            skip: e.Handled = false; return;
            }
            if (e is MouseMovedEvent)
            {
                if (InputManager.isMouseDown(InputManager.MouseKey.LeftKey))
                {
                    e.Handled = true;
                    if (LockCamera) return;
                    if (!dragging && !BoundingBox.ContainsPoint((e as MouseMovedEvent).Position.ToVector2())) goto skip;
                    dragging = true;
                    Point offset = (e as MouseMovedEvent).Offset;
                    cam.StepHorizontal(offset.X);
                    cam.StepVertical(offset.Y);
                    return;
                skip: e.Handled = false; return;
                }
            }
            if (e is TouchAllFingersOffEvent && SnapCameraToCells)
                SnapCam();
        }

        internal void SlideLeft()
        {
            cam.StepHorizontal(-unitwidth); SnapCam();
        }

        internal void SlideRight()
        {
            cam.StepHorizontal(unitwidth); SnapCam();
        }

        internal void TrimGridToVisible()
        {
            maxwidth = Math.Min(Screen.Width, Width); maxheight = Math.Min(Screen.Height, Height);            
            cam = new Camera(0, 0, maxwidth, maxheight, TotalWidth, TotalHeight);
            cam.FitToScreen = false;
        }
        internal void TrimAllToGrid()
        {
            float w = Width, h = Height;
            foreach (UIVisibleObject obj in cells)
                obj.Size = new Vector2(Math.Min(w, obj.Width), Math.Min(h, obj.Height));
        }
        private void SnapCam()
        {
            snaptarget = null;
            float intersize = 0;
            foreach (UIVisibleObject cell in cells)
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
            foreach (UIVisibleObject cell in cells)
            { cell.FitSiblings(); cell.CentralizeSiblings(); }
        }
    }
}
internal enum Orientation { Landscape, Portrait }
