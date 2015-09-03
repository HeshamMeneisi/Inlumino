using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Inlumino_SHARED
{
    class Prism : StaticObject, IObstructingObject, ILightSource
    {
        ILightSource currentsource = null;
        Dictionary<ILightSource, Direction> allsources = new Dictionary<ILightSource, Direction>();
        Direction input;
        Direction output;

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
            return ObjectType.Prism;
        }

        public void HandleOn(ILightSource source, Direction dir)
        {
            if (!allsources.ContainsKey(source))
                allsources.Add(source, dir);
            Tile target = default(Tile);
            if (dir == Common.NextDirCW(rotation))
            {
                currentsource = source;
                state = 1;
                input = Common.NextDirCW(rotation);
                output = Common.NextDirCW(rotation, 2);
                target = parenttile.getAdjacentTile(output);
                Common.PowerUpTile(target, Common.ReverseDir(output), this);
            }
            else if (dir == Common.NextDirCW(rotation, 2))
            {
                currentsource = source;
                state = 1;
                input = Common.NextDirCW(rotation, 2);
                output = Common.NextDirCW(rotation);
                target = parenttile.getAdjacentTile(output);
                Common.PowerUpTile(target, Common.ReverseDir(output), this);
            }
        }

        public void HandleOff(ILightSource source, Direction dir)
        {
            if (allsources.ContainsKey(source))
                allsources.Remove(source);
            if (source == currentsource)
            {
                currentsource = null;
                state = 0;
                Tile target = parenttile.getAdjacentTile(output);
                Common.PowerOffTile(target, output, this);
            }
        }

        public bool IsFeedingDirection(Direction dir)
        {
            return IsOn && output == dir;
        }

        public Prism(TextureID[] tid, Tile parent) : base(tid, parent)
        {

        }
    }
}