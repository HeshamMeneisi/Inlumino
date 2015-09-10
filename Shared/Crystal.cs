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
        public List<ILightSource> allsources = new List<ILightSource>();
        public override ObjectType getType()
        {
            return ObjectType.Crystal;
        }

        public void HandlePulse(bool charge, Direction dir, ILightSource source)
        {
            if (dir != Common.ReverseDir(rotation)) return;

            if (charge)
            { if (!allsources.Contains(source)) allsources.Add(source); }
            else if (allsources.Contains(source)) allsources.Remove(source);
            if(allsources.Count>0)
                if(state == 0)
                {
                    SoundManager.PlaySound(DataHandler.Sounds[SoundType.CrystalLit], SoundCategory.SFX);
                    parenttile.Parent.CheckWin();
                }
            state = Math.Min(1, allsources.Count);
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

        public void Reset()
        {
            state = 0;
            allsources.Clear();
        }
    }
}
