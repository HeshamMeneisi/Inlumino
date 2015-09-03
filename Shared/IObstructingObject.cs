using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlumino_SHARED
{
    interface IObstructingObject
    {
        void HandleOn(ILightSource source, Direction dir);
        void HandleOff(ILightSource source, Direction dir);
    }
}
