using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlumino_SHARED
{
    interface IObstructingObject
    {
        void HandlePulse(bool charge, Direction side, ILightSource source);
        void Reset();
    }
}
