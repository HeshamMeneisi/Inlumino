using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inlumino_SHARED
{
    class Camera
    {
        private RectangleF CurrentView;
        private RectangleF TargetView;
        // Moving allowance around the stage (For background visibility)
        private Padding stagepadding;

        public Padding StagePadding { get { return stagepadding; } set { stagepadding = value; } }

        private float stgxlim;
        private float stgylim;

       public float MaxX
        { get { return stgxlim + stagepadding.Right; } }

        public float MaxY
        { get { return stgylim + stagepadding.Bottom; } }

        public float MinX
        { get { return -stagepadding.Left; } }

        public float MinY
        { get { return -stagepadding.Top; } }

        public float MaxW
        { get { return MaxX - MinX; } }

        public float MaxH
        { get { return MaxY - MinY; } }

        float maxzoom;
        float smoothness;// larger is less smooth, (>=1) means no smoothness
        public Camera(float x, float y, float vieww, float viewh, float totalw, float totalh, Padding stpad = null, float maxz = 0.25f, float smoothfactor = 0.08f)
        {
            CurrentView = new RectangleF(x, y, vieww, viewh);
            TargetView = CurrentView.Clone();
            stgxlim = totalw;
            stgylim = totalh;
            stagepadding = stpad;
            maxzoom = maxz;
            smoothness = smoothfactor;
        }
        private float xScale { get { return Screen.Width / CurrentView.Width; } }
        private float yScale { get { return Screen.Height / CurrentView.Height; } }
        public bool isInsideView(RectangleF r)
        {
            return (r.X < CurrentView.X + CurrentView.Width && r.X + r.Width > CurrentView.X) || (r.Y < CurrentView.Y + CurrentView.Height && r.Y + r.Height > CurrentView.Y);
        }
        public bool isInsideView(Vector2 v)
        {
            return v.X > CurrentView.X && v.Y > CurrentView.Y && v.X < CurrentView.X + CurrentView.Width && v.Y < CurrentView.Y + CurrentView.Height;
        }
        public Vector2 Transform(Vector2 v)
        {
            return new Vector2(xScale * (v.X - CurrentView.X), yScale * (v.Y - CurrentView.Y));
        }
        public RectangleF Transform(RectangleF r)
        {
            return new RectangleF(xScale * (r.X - CurrentView.X), yScale * (r.Y - CurrentView.Y), xScale * r.Width, yScale * r.Height);
        }
        internal Vector2 DeTransform(Vector2 v)
        {
            return new Vector2(v.X / xScale + CurrentView.X, v.Y / yScale + CurrentView.Y);
        }
        public void StepHorizontal(float stepsize)
        {
            float scaledStep = stepsize / xScale;
            if (TargetView.X + TargetView.Width + scaledStep > MaxX)
                TargetView.X = MaxX - TargetView.Width;
            else if (TargetView.X + scaledStep < MinX)
                TargetView.X = MinX;
            else
                TargetView.X += scaledStep;
        }

        public void StepVertical(float stepsize)
        {
            float scaledStep = stepsize / yScale;
            if (TargetView.Y + TargetView.Height + scaledStep > MaxY)
                TargetView.Y = MaxY - TargetView.Height;
            else if (TargetView.Y + scaledStep < MinY)
                TargetView.Y = MinY;
            else
                TargetView.Y += scaledStep;
        }

        public void Zoom(float p)
        {
            if (p != 0) // Save time
            {
                float nx, ny, nw, nh;
                nw = TargetView.Width * (1 + p);
                nh = TargetView.Height * (1 + p);
                // Enforcing max zoom settings
                if (nh / MaxH < maxzoom)
                {
                    nh = MaxH * maxzoom;
                    nw = TargetView.Width * (nh / TargetView.Height);
                }
                else if (nw / MaxW < maxzoom)
                {
                    nw = MaxW * maxzoom;
                    nh = TargetView.Height * (nw / TargetView.Width);
                }
                // Making the zoom effect appear central
                nx = TargetView.X - (nw - TargetView.Width) / 2;
                ny = TargetView.Y - (nh - TargetView.Height) / 2;
                if (nx < MinX) nx = MinX;
                if (ny < MinY) ny = MinY;
                recheck:
                if (nx + nw > MaxX)
                {
                    if (nw > MaxW)
                    {
                        nw = MaxW; nx = MinX;
                        nh = TargetView.Height * (nw / TargetView.Width);
                        ny = TargetView.Y - (nh - TargetView.Height) / 2;
                        if (ny < MinY) ny = MinY;
                    }
                    else nx = MaxX - nw;
                }
                if (ny + nh > MaxY)
                {
                    if (nh > MaxH)
                    {
                        nh = MaxH; ny = MinY;
                        nw = TargetView.Width * (nh / TargetView.Height);
                        nx = TargetView.X - (nw - TargetView.Width) / 2;
                        if (nx < MinX) nx = MinX;
                    }
                    else ny = MaxY - nh;
                    goto recheck;
                }
                TargetView = new RectangleF(nx, ny, nw, nh);
            }

        }

        internal void CenterStage(bool animated)
        {
            float nx = (MaxW - CurrentView.Width) / 2 + MinX;
            float ny = (MaxH - CurrentView.Height) / 2 + MinY;
            if (animated) TargetView = new RectangleF(nx, ny, TargetView.Width, TargetView.Height);
            else
            {
                CurrentView = new RectangleF(nx, ny, CurrentView.Width, CurrentView.Height);
                TargetView = CurrentView.Clone();
            }
        }

        public float getStageScale()
        {
            return MaxY / CurrentView.Height;
        }
        public void Update(GameTime time)
        {
            Vector2 lv = TargetView.Location - CurrentView.Location;
            Vector2 sv = TargetView.Size - CurrentView.Size;
            float ls;
            if (lv.Length() > 0)
            {
                ls = lv.Length() * smoothness;
                lv.Normalize();
                CurrentView.X = MathHelper.Clamp(CurrentView.X + ls * lv.X, lv.X > 0 ? MinX : TargetView.X, lv.X > 0 ? TargetView.X : MaxX);
                CurrentView.Y = MathHelper.Clamp(CurrentView.Y + ls * lv.Y, lv.Y > 0 ? MinY : TargetView.Y, lv.Y > 0 ? TargetView.Y : MaxY);
            }
            if (sv.Length() > 0)
            {
                ls = sv.Length() * smoothness;
                sv.Normalize();
                CurrentView.Width = MathHelper.Clamp(CurrentView.Width + ls * sv.X, sv.X > 0 ? 0 : TargetView.Width, sv.X > 0 ? TargetView.Width : MaxW);
                CurrentView.Height = MathHelper.Clamp(CurrentView.Height + ls * sv.Y, sv.Y > 0 ? 0 : TargetView.Height, sv.Y > 0 ? TargetView.Height : MaxH);
            }
        }

        public float GetRecommendedDrawingFuzz()
        {
            // This has to be based on both the camera to screen ratio and camera to stage ratio
            return ((/*The bigger the screen, the bigger the fuzz needed*/(xScale + yScale) / 2)
                /*The more zoomed the camera is, the more fuzz is needed*/ + getStageScale())
                /*Those effects cancel each other, so we need the average.*/ / 2;
        }
    }
}
