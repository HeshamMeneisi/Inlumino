using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlumino_SHARED
{
    class Block : StaticObject, IObstructingObject
    {

        internal Block(TextureID[] tid, Tile parent) : base(tid, parent) { }
        internal override ObjectType getType()
        {
            return ObjectType.Block;
        }

        public void HandlePulse(bool charge, Direction dir, ILightSource source)
        {
            //
        }

        public void Reset()
        {            
        }
    }
}
