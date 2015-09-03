using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlumino_SHARED
{
    class LightSourceObject : StaticObject, ILightSource, IObstructingObject
    {
        bool on = false;
        public LightSourceObject(TextureID[] tid, Tile parent) : base(tid, parent) { }

        public bool IsOn
        {
            get
            {
                return on;
            }
        }

        public override ObjectType getType()
        {
            return ObjectType.LightSource;
        }

        public void HandleOff(ILightSource source, Direction dir) {/*don't care*/}

        public void HandleOn(ILightSource source, Direction dir) { /*already obstructed*/}

        public bool IsFeedingDirection(Direction dir)
        {
            return dir == rotation;
        }

        public void TurnOn()
        {
            on = true;
            state = 1;
            Common.PowerUpTile(parenttile.getAdjacentTile(rotation), Common.ReverseDir(rotation), this);
        }
        public void TurnOff()
        {
            on = false;
            state = 0;
            Common.PowerOffTile(parenttile.getAdjacentTile(rotation), Common.ReverseDir(rotation), this);
        }
        public override void RotateCW(bool instant, int clicks = 1)
        {
            if (instant) base.RotateCW(instant, clicks);
        }
        public override void RotateCCW(bool instant, int clicks = 1)
        {
            if (instant) base.RotateCCW(instant, clicks);
        }
    }
}
