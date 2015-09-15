using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlumino_SHARED
{
    class Padding
    {
        internal float Top { get; set; }
        internal float Bottom { get; set; }
        internal float Left { get; set; }
        internal float Right { get; set; }

        internal Padding(float left = 0, float right = 0, float top = 0, float bottom = 0)
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
