using System.Collections.Generic;

namespace Inlumino_SHARED
{
    internal class FourWay : PowerableSource
    {
        public FourWay(TextureID[] tid, Tile tile) : base(tid, tile)
        {
            map[Direction.East] = new List<Direction>() { Direction.North, Direction.South, Direction.West };
            map[Direction.North] = new List<Direction>() { Direction.South, Direction.East, Direction.West };
            map[Direction.West] = new List<Direction>() { Direction.North, Direction.South, Direction.East };
            map[Direction.South] = new List<Direction>() { Direction.North, Direction.East, Direction.West };
        }

        internal override ObjectType getType()
        {
            return ObjectType.FourWay;
        }
    }
}