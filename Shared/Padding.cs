using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlumino_SHARED
{
    class Padding
    {
        public float Top { get; set; }
        public float Bottom { get; set; }
        public float Left { get; set; }
        public float Right { get; set; }

        public Padding(float left = 0, float right = 0, float top = 0, float bottom = 0)
        {
            Top = top; Bottom = bottom; Left = left; Right = right;
        }

        internal void Scale(float xscale, float yscale)
        {
            Left *= xscale;Right *= xscale;
            Top *= yscale;Bottom *= yscale;
        }

        internal Padding Clone()
        {
            return new Padding(Left, Right, Top, Bottom);
        }
    }
}
