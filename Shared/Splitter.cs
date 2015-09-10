using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlumino_SHARED
{
    class Splitter : PowerableSource
    {
        public Splitter(TextureID[] tid, Tile parent) : base(tid, parent)
        {
            map[Direction.East] = new List<Direction>() { Direction.North, Direction.West };
            map[Direction.North] = new List<Direction>() { Direction.East, Direction.West };
            map[Direction.West] = new List<Direction>() { Direction.North, Direction.East };
        }
        public override ObjectType getType()
        {
            return ObjectType.Splitter;
        }
    }
}
