using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlumino_SHARED
{
    interface ILightSource
    {
        Tile ParentTile { get; }
        bool IsOn { get; }
    }
}
