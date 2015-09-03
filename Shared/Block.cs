using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlumino_SHARED
{
    class Block : StaticObject, IObstructingObject
    {

        public Block(TextureID[] tid, Tile parent) : base(tid, parent) { }
        public override ObjectType getType()
        {
            return ObjectType.Block;
        }

        // Maybe have the texture affected with light??
        public void HandleOff(ILightSource source, Direction dir)
        {
            // Just a block here nothing to do
        }

        public void HandleOn(ILightSource source, Direction dir)
        {
            // Just a block here nothing to do
        }
    }
}
