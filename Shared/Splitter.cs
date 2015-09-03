using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlumino_SHARED
{
    class Splitter: StaticObject, IObstructingObject, ILightSource
    {
        ILightSource currentsource = null;
        Dictionary<ILightSource, Direction> allsources = new Dictionary<ILightSource, Direction>();
        Direction input;
        Direction output1;
        Direction output2;

        public override void Update(GameTime time)
        {
            if (rotation != targetrotation)
            {
                // Simulate current source off
                if (IsOn)
                {
                    ILightSource temp = currentsource;
                    HandleOff(currentsource, input);
                    allsources.Add(temp, input);
                }
                Rotation = targetrotation;
                // Rehandle everything currently on
                foreach (KeyValuePair<ILightSource, Direction> pair in allsources)
                    HandleOn(pair.Key, pair.Value);                
            }
            base.Update(time);
        }
        public bool IsOn
        {
            get
            {
                return currentsource != null && currentsource.IsOn;
            }
        }

        public override ObjectType getType()
        {
            return ObjectType.Splitter;
        }

        public void HandleOn(ILightSource source, Direction dir)
        {
            if (!allsources.ContainsKey(source))
                allsources.Add(source, dir);
            Tile target = default(Tile);
            if (dir == rotation)
            {             
                output1 = Common.NextDirCCW(rotation);
                output2 = Common.NextDirCW(rotation);                
            }
            else if(dir == Common.NextDirCW(rotation))
            {                
                output1 = Common.NextDirCCW(rotation);
                output2 = rotation;
            }
            else if (dir == Common.NextDirCCW(rotation))
            {
                output1 = rotation;
                output2 = Common.NextDirCW(rotation);
            }
            else { return; }
            input = dir;
            currentsource = source;
            state = 1;
            target = parenttile.getAdjacentTile(output1);
            Common.PowerUpTile(target, Common.ReverseDir(output1), this);
            target = parenttile.getAdjacentTile(output2);
            Common.PowerUpTile(target, Common.ReverseDir(output2), this);
        }

        public void HandleOff(ILightSource source, Direction dir)
        {
            if (allsources.ContainsKey(source))
                allsources.Remove(source);
            if (source == currentsource)
            {
                currentsource = null;
                state = 0;
                Tile target = parenttile.getAdjacentTile(output1);
                Common.PowerOffTile(target, Common.ReverseDir(output1), this);
                target = parenttile.getAdjacentTile(output2);
                Common.PowerOffTile(target, Common.ReverseDir(output2), this);
            }
        }

        public bool IsFeedingDirection(Direction dir)
        {
            return IsOn && (Common.NextDirCW(rotation) == dir || Common.NextDirCCW(rotation) == dir);
        }

        public Splitter(TextureID[] tid, Tile parent) : base(tid, parent)
        {

        }
    }
}
