using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inlumino_SHARED
{
    class RectangleF
    {
        float _x;
        float _y;
        float width;
        float height;

        public RectangleF(float x, float y, float width, float height)
        {
            this._x = x;
            this._y = y;
            this.width = width;
            this.height = height;
        }

        public RectangleF(Vector2 location, Vector2 size)
        {
            Location = location;
            Size = size;
        }

        public float X { get { return _x; } set { _x = value; } }
        public float Y { get { return _y; } set { _y = value; } }
        public float Width { get { return width; } set { width = value; } }
        public float Height { get { return height; } set { height = value; } }

        public Vector2 Location { get { return new Vector2(_x, _y); } set { _x = value.X; _y = value.Y; } }
        public Vector2 Size { get { return new Vector2(width, height); } set { width = value.X; height = value.Y; } }

        public float Right { get { return _x + width; } }

        internal RectangleF Offset(Vector2 offset)
        {
            return new RectangleF(_x + offset.X, _y + offset.Y, width, height);
        }

        public float Left { get { return _x; } }
        public float Bottom { get { return _y + height; } }
        public float Top { get { return _y; } }
        public Vector2 Center { get { return new Vector2(_x + width / 2, _y + height / 2); } }

        public float Area { get { return Width * Height; } }

        internal Rectangle ToRectangle()
        {
            return new Rectangle((int)_x, (int)_y, (int)width, (int)height);
        }

        internal RectangleF Clone()
        {
            return new RectangleF(_x, _y, width, height);
        }

        internal Rectangle getSmoothRectangle(float fuzz)
        {
            return new Rectangle((int)Math.Floor(_x - fuzz), (int)Math.Floor(_y - fuzz), (int)Math.Ceiling(width + fuzz), (int)Math.Ceiling(height + fuzz));
        }

        internal bool ContainsPoint(Vector2 postion)
        {
            return postion.X >= Left && postion.Y >= Top && postion.X <= Right && postion.Y <= Bottom;
        }

        internal RectangleF Inflate(float x, float y)
        {
            return new RectangleF(X - x, Y - y, Width + x * 2, Height + y * 2);
        }

        internal RectangleF Mask(RectangleF whole, RectangleF part)
        {
            float xs = (part.X - whole.X) / whole.Width,
                        ys = (part.Y - whole.Y) / whole.Height,
                        ws = part.Width / whole.Width,
                        hs = part.Height / whole.Height;

            return new RectangleF(X + Width * xs, Y + Height * ys, Width * ws, Height * hs);
        }

        public bool Intersects(RectangleF r)
        {
            return Left <= r.Right && Right >= r.Left && Top <= r.Bottom && Bottom >= r.Top;
        }

        public RectangleF Intersection(RectangleF r)
        {
            if (!Intersects(r)) return default(RectangleF);
            float x, y;
            return new RectangleF(
                x = Math.Max(Left, r.Left),
                y = Math.Max(Top, r.Top),
                Math.Min(Right, r.Right) - x,
                Math.Min(Bottom, r.Bottom) - y);
        }
        public static implicit operator RectangleF(Rectangle r)
        {
            return new RectangleF(r.X, r.Y, r.Width, r.Height);
        }

        public override string ToString()
        {
            return "Pos:" + Location.ToString() + "Size:" + Size.ToString();
        }
    }
}
