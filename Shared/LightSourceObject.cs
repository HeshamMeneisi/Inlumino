using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public bool IsFeedingDirection(Direction dir)
        {
            return dir == rotation;
        }

        public void TurnOn()
        {
            on = true;
            state = 1;
            Common.PulseTile(parenttile.getAdjacentTile(rotation), true, Common.ReverseDir(rotation), this);
        }
        public void TurnOff()
        {
            on = false;
            state = 0;
            Common.PulseTile(parenttile.getAdjacentTile(rotation), false, Common.ReverseDir(rotation), this);
        }
        public override void RotateCW(bool instant, int clicks = 1)
        {
            if (instant) base.RotateCW(instant, clicks);
        }
        public override void RotateCCW(bool instant, int clicks = 1)
        {
            if (instant) base.RotateCCW(instant, clicks);
        }

        public void HandlePulse(bool charge, Direction dir, ILightSource source)
        {
        }

        public void Reset()
        {
            
        }
    }
}
