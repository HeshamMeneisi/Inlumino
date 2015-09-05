using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlumino_SHARED
{
    class Crystal : StaticObject, IObstructingObject
    {
        public Crystal(TextureID[] tid, Tile parent) : base(tid, parent) { }
        public override ObjectType getType()
        {
            return ObjectType.Crystal;
        }

        public void HandleOff(ILightSource source, Direction dir)
        {
            if (dir == Common.ReverseDir(rotation))
            {
                state = 0;
                parenttile.Parent.CheckWin();
            }
        }

        public void HandleOn(ILightSource source, Direction dir)
        {
            if (dir == Common.ReverseDir(rotation))
            {
                state = 1;
                SoundManager.PlaySound(DataHandler.Sounds[SoundType.CrystalLit], SoundCategory.SFX);
                parenttile.Parent.CheckWin();
            }
        }

        public override void RotateCW(bool instant, int clicks = 1)
        {
            if (instant) base.RotateCW(instant, clicks);
        }
        public override void RotateCCW(bool instant, int clicks = 1)
        {
            if (instant) base.RotateCCW(instant, clicks);
        }

        internal bool IsLit()
        {
            return state == 1;
        }
    }
}
