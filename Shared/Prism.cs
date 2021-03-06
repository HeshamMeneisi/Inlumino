﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Inlumino_SHARED
{
    class Prism : PowerableSource
    {
        internal Prism(TextureID[] tid, Tile parent) : base(tid, parent)
        {
            map[Direction.East] = new List<Direction>() { Direction.South };
            map[Direction.South] = new List<Direction>() { Direction.East };
        }
        internal override ObjectType getType()
        {
            return ObjectType.Prism;
        }
    }
}