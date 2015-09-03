using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inlumino_SHARED
{
    class RectangleF
    {
        float x;
        float y;
        float width;
        float height;

        public RectangleF(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public RectangleF(Vector2 location, Vector2 size)
        {
            Location = location;
            Size = size;
        }

        public float X { get { return x; } set { x = value; } }
        public float Y { get { return y; } set { y = value; } }
        public float Width { get { return width; } set { width = value; } }
        public float Height { get { return height; } set { height = value; } }

        public Vector2 Location { get { return new Vector2(x, y); } set { x = value.X; y = value.Y; } }
        public Vector2 Size { get { return new Vector2(width, height); } set { width = value.X; height = value.Y; } }

        public float Right { get { return x + width; } }

        internal RectangleF Offset(Vector2 offset)
        {
            return new RectangleF(x + offset.X, y + offset.Y, width, height);
        }

        public float Left { get { return x; }  }
        public float Bottom { get { return y + height; }  }
        public float Top { get { return y; }  }
        public Vector2 Center { get { return new Vector2(x + width / 2, y + height / 2); } }

        internal Rectangle getRectangle()
        {
            return new Rectangle((int)x, (int)y, (int)width, (int)height);
        }

        internal RectangleF Clone()
        {
            return new RectangleF(x, y, width, height);
        }

        internal Rectangle getSmoothRectangle(float fuzz)
        {
            return new Rectangle((int)Math.Floor(x-fuzz), (int)Math.Floor(y-fuzz), (int)Math.Ceiling(width+fuzz), (int)Math.Ceiling(height+fuzz));
        }
    }
}
