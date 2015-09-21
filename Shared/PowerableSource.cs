using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Inlumino_SHARED
{
    class PowerableSource : StaticObject, ILightSource, IObstructingObject
    {
        internal PowerableSource(TextureID[] tid, Tile parent) : base(tid, parent) { IsInteractable = true; }
        public bool IsOn
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        internal override ObjectType getType()
        {
            throw new NotImplementedException();
        }

        Dictionary<Direction, List<ILightSource>> allsources = new Dictionary<Direction, List<ILightSource>>()
        {
            {Direction.North,new List<ILightSource>() },
            {Direction.East,new List<ILightSource>() },
            {Direction.South,new List<ILightSource>() },
            {Direction.West,new List<ILightSource>() }
        };

        private Dictionary<Direction, bool> inputstate = new Dictionary<Direction, bool>()
        {
            {Direction.North,false },
            {Direction.East,false },
            {Direction.South,false},
            {Direction.West,false }
        };
        private Dictionary<Direction, bool> outputstate = new Dictionary<Direction, bool>()
        {
            {Direction.North,false },
            {Direction.East,false },
            {Direction.South,false},
            {Direction.West,false }
        };
        protected Dictionary<Direction, List<Direction>> map = new Dictionary<Direction, List<Direction>>()
        {
            {Direction.North, new List<Direction>() { } },
            {Direction.East, new List<Direction>() { } },
            {Direction.South, new List<Direction>() { } },
            {Direction.West, new List<Direction>(){ } },
        };

        public void HandlePulse(bool charge, Direction side, ILightSource source)
        {
            if (source == this) return;
            if (charge)
            {
                if (!allsources[side].Contains(source))
                { allsources[side].Add(source); OneSourceUp(side); }
            }
            else
            {
                if (allsources[side].Contains(source))
                    allsources[side].Remove(source); OneSourceDown(side);
            }
        }

        private void OneSourceUp(Direction side)
        {
            if (allsources[side].Count == 1)
            { inputstate[side] = true; updateStatus(); }
        }
        private void OneSourceDown(Direction side)
        {
            if (allsources[side].Count == 0)
            { inputstate[side] = false; /*Inifnite loop turnAllDependantsOff(side);*/ updateStatus(); }
        }
        /*
        private void turnAllDependantsOff(Direction side)
        {
            Direction rs = Common.RelativeDir(side, rotation);
            foreach (Direction dir in Direction.GetValues(typeof(Direction)))
            {
                Direction relativedir = Common.RelativeDir(dir, rotation);
                if (map[relativedir].Contains(rs) && outputstate[dir])
                    Common.PulseTile(parenttile.getAdjacentTile(dir), outputstate[dir] = false, Common.ReverseDir(dir), this);
            }
        }*/

        internal override void Update(GameTime time)
        {
            if (rotation != targetrotation)
            {
                base.Update(time);
                updateStatus();
            }
            else base.Update(time);
        }

        private void updateStatus()
        {
            state = 0;
            foreach (Direction dir in Direction.GetValues(typeof(Direction)))
            {
                // dir is a screen wise direction
                // This is the direction regarding the object itself
                Direction relativedir = Common.RelativeDir(dir, rotation);
                bool on = false;
                foreach (int requirement in map[relativedir]) // Requirements are "or" logic
                    if (inputstate[Common.NextDirCW(rotation, requirement)]) on = true;
                if (on != outputstate[dir])
                    Common.PulseTile(parenttile.getAdjacentTile(dir), outputstate[dir] = on, Common.ReverseDir(dir), this);
                if (on) state = 1;
            }
        }

        public void Reset()
        {
            state = 0;            
            foreach (Direction dir in Direction.GetValues(typeof(Direction)))
            {
                allsources[dir].Clear();
                inputstate[dir] = outputstate[dir] = false;
            }
        }
    }
}
