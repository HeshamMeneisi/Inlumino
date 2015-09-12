using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inlumino_SHARED
{
    class Camera
    {
        private RectangleF currentView;
        private RectangleF targetView;
        // Moving allowance around the stage (For background visibility)
        private Padding stagepadding;

        public Padding StagePadding { get { return stagepadding; } set { stagepadding = value; } }

        private float stgxlim;
        private float stgylim;

        public RectangleF ActualView { get { return currentView; } }
        public RectangleF TargetView
        { get { return targetView; } }
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
            currentView = new RectangleF(x, y, vieww, viewh);
            targetView = currentView.Clone();
            stgxlim = totalw;
            stgylim = totalh;
            stagepadding = stpad != null ? stpad : new Padding(0, 0, 0, 0);
            maxzoom = maxz;
            smoothness = smoothfactor;
        }
        public bool FitToScreen = true;
        private float xScale { get { return FitToScreen ? Screen.Width / currentView.Width : 1; } }
        private float yScale { get { return FitToScreen ? Screen.Height / currentView.Height : 1; } }
        public bool isInsideView(RectangleF r)
        {
            return currentView.Intersects(r);
        }
        public bool isInsideView(Vector2 v)
        {
            return currentView.ContainsPoint(v);
        }
        public Vector2 Transform(Vector2 v)
        {
            return new Vector2(xScale * (v.X - currentView.X), yScale * (v.Y - currentView.Y));
        }
        public RectangleF Transform(RectangleF r)
        {
            if (r == null) return null;
            return new RectangleF(xScale * (r.X - currentView.X), yScale * (r.Y - currentView.Y), xScale * r.Width, yScale * r.Height);
        }
        public RectangleF TransformWithCropping(RectangleF r, out RectangleF rectnocropping)
        {
            rectnocropping = Transform(r);
            return Transform(currentView.Intersection(r));
        }
        public Rectangle Transform(Rectangle r)
        {
            return new RectangleF(xScale * (r.X - currentView.X), yScale * (r.Y - currentView.Y), xScale * r.Width, yScale * r.Height).ToRectangle();
        }
        internal Vector2 DeTransform(Vector2 v)
        {
            return new Vector2(v.X / xScale + currentView.X, v.Y / yScale + currentView.Y);
        }
        public void StepHorizontal(float stepsize)
        {
            float scaledStep = stepsize / xScale;
            if (targetView.X + targetView.Width + scaledStep > MaxX)
                targetView.X = MaxX - targetView.Width;
            else if (targetView.X + scaledStep < MinX)
                targetView.X = MinX;
            else
                targetView.X += scaledStep;
        }

        public void StepVertical(float stepsize)
        {
            float scaledStep = stepsize / yScale;
            if (targetView.Y + targetView.Height + scaledStep > MaxY)
                targetView.Y = MaxY - targetView.Height;
            else if (targetView.Y + scaledStep < MinY)
                targetView.Y = MinY;
            else
                targetView.Y += scaledStep;
        }

        public void Zoom(float p)
        {
            if (p != 0) // Save time
            {
                float nx, ny, nw, nh;
                nw = targetView.Width * (1 + p);
                nh = targetView.Height * (1 + p);
                // Enforcing max zoom settings
                if (nh / MaxH < maxzoom)
                {
                    nh = MaxH * maxzoom;
                    nw = targetView.Width * (nh / targetView.Height);
                }
                else if (nw / MaxW < maxzoom)
                {
                    nw = MaxW * maxzoom;
                    nh = targetView.Height * (nw / targetView.Width);
                }
                // Making the zoom effect appear central
                nx = targetView.X - (nw - targetView.Width) / 2;
                ny = targetView.Y - (nh - targetView.Height) / 2;
                if (nx < MinX) nx = MinX;
                if (ny < MinY) ny = MinY;
                recheck:
                if (nx + nw > MaxX)
                {
                    if (nw > MaxW)
                    {
                        nw = MaxW; nx = MinX;
                        nh = targetView.Height * (nw / targetView.Width);
                        ny = targetView.Y - (nh - targetView.Height) / 2;
                        if (ny < MinY) ny = MinY;
                    }
                    else nx = MaxX - nw;
                }
                if (ny + nh > MaxY)
                {
                    if (nh > MaxH)
                    {
                        nh = MaxH; ny = MinY;
                        nw = targetView.Width * (nh / targetView.Height);
                        nx = targetView.X - (nw - targetView.Width) / 2;
                        if (nx < MinX) nx = MinX;
                    }
                    else ny = MaxY - nh;
                    goto recheck;
                }
                targetView = new RectangleF(nx, ny, nw, nh);
            }

        }

        internal void CenterStage(bool animated)
        {
            float nx = (MaxW - currentView.Width) / 2 + MinX;
            float ny = (MaxH - currentView.Height) / 2 + MinY;
            if (animated) targetView = new RectangleF(nx, ny, targetView.Width, targetView.Height);
            else
            {
                currentView = new RectangleF(nx, ny, currentView.Width, currentView.Height);
                targetView = currentView.Clone();
            }
        }

        internal void EnsureVisible(RectangleF r,bool resize=false)
        {
            if (resize) Zoom(r.Area / targetView.Area);
            targetView = new RectangleF(r.X, r.Y, targetView.Width, targetView.Height);            
        }

        public float getStageScale()
        {
            return MaxY / currentView.Height;
        }
        public void Update(GameTime time)
        {
            Vector2 lv = targetView.Location - currentView.Location;
            Vector2 sv = targetView.Size - currentView.Size;
            float ls;
            if (lv.Length() > 0)
            {
                ls = lv.Length() * smoothness;
                lv.Normalize();
                currentView.X = MathHelper.Clamp(currentView.X + ls * lv.X, lv.X > 0 ? MinX : targetView.X, lv.X > 0 ? targetView.X : MaxX);
                currentView.Y = MathHelper.Clamp(currentView.Y + ls * lv.Y, lv.Y > 0 ? MinY : targetView.Y, lv.Y > 0 ? targetView.Y : MaxY);
            }
            if (sv.Length() > 0)
            {
                ls = sv.Length() * smoothness;
                sv.Normalize();
                currentView.Width = MathHelper.Clamp(currentView.Width + ls * sv.X, sv.X > 0 ? 0 : targetView.Width, sv.X > 0 ? targetView.Width : MaxW);
                currentView.Height = MathHelper.Clamp(currentView.Height + ls * sv.Y, sv.Y > 0 ? 0 : targetView.Height, sv.Y > 0 ? targetView.Height : MaxH);
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
