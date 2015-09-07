using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Inlumino_SHARED
{
    class Portal : StaticObject, IObstructingObject, ILightSource
    {
        ILightSource currentnearbysource = null;
        List<ILightSource> currentdistantsources = new List<ILightSource>();
        Dictionary<ILightSource, Direction> allsources = new Dictionary<ILightSource, Direction>();

        public override void Update(GameTime time)
        {
            if (rotation != targetrotation)
            {
                if (ison)
                {
                    Tile target = parenttile.getAdjacentTile(Common.NextDirCW(rotation));
                    if (target != default(Tile)) Common.PowerOffTile(target, Common.ReverseDir(Common.NextDirCW(rotation)), this);
                }
                // Simulate current source off
                if (currentnearbysource != null)
                {
                    ILightSource temp = currentnearbysource;
                    HandleOff(currentnearbysource, Common.NextDirCW(rotation));
                    allsources.Add(temp, Common.NextDirCW(rotation));
                }
                Rotation = targetrotation;
                // Rehandle everything currently on
                try
                {
                    foreach (KeyValuePair<ILightSource, Direction> pair in allsources)
                        HandleOn(pair.Key, pair.Value);
                }
                catch { /* Already found our working source.*/}
                if (ison)
                {
                    Tile target = parenttile.getAdjacentTile(Common.NextDirCW(rotation));
                    if (target != default(Tile)) Common.PowerUpTile(target, Common.ReverseDir(Common.NextDirCW(rotation)), this);
                }
            }
            base.Update(time);
        }
        bool ison = false;
        public bool IsOn
        {
            get
            {
                return ison;
            }
        }

        public override ObjectType getType()
        {
            return ObjectType.Portal;
        }

        public void HandleOn(ILightSource source, Direction dir)
        {
            if (!allsources.ContainsKey(source))
                allsources.Add(source, dir);
            if (dir == Common.NextDirCW(rotation))
            {
                currentnearbysource = source;
                state = 1;
                foreach (Tile t in getCrossTiles())
                    if (t.hasObject(typeof(Portal)))
                        (t.getObject() as Portal).PowerUp(this);
            }
        }

        private IEnumerable<Tile> getCrossTiles()
        {
            IEnumerable<Tile> rowtiles = parenttile.Parent.getTileMap().getRow(parenttile.MapPos.X);
            IEnumerable<Tile> coltiles = parenttile.Parent.getTileMap().getColumn(parenttile.MapPos.Y);
            foreach (Tile t in rowtiles)
                yield return t;
            foreach (Tile t in coltiles)
                yield return t;
        }
        public void HandleOff(ILightSource source, Direction dir)
        {
            if (allsources.ContainsKey(source))
                allsources.Remove(source);
            if (source == currentnearbysource)
            {
                currentnearbysource = null;
                state = 0;
                foreach (Tile t in getCrossTiles())
                    if (t.hasObject(typeof(Portal)))
                        (t.getObject() as Portal).PowerOff(this);
            }
        }
        private void PowerUp(Portal source)
        {
            if (source == this) return;
            ison = true;
            state = 1;
            if (!currentdistantsources.Contains(source))
                currentdistantsources.Add(source);
            Tile target = parenttile.getAdjacentTile(Common.NextDirCW(rotation));
            if (target != default(Tile)) Common.PowerUpTile(target, Common.ReverseDir(Common.NextDirCW(rotation)), this);
        }
        private void PowerOff(Portal source)
        {
            if (source == this) return;
            if (currentdistantsources.Contains(source))
                currentdistantsources.Remove(source);
            if (currentnearbysource == null && currentdistantsources.Count == 0)
            {
                ison = false; state = 0;
                Tile target = parenttile.getAdjacentTile(Common.NextDirCW(rotation));
                if (target != default(Tile)) Common.PowerOffTile(target, Common.ReverseDir(Common.NextDirCW(rotation)), this);
            }
        }

        public bool IsFeedingDirection(Direction dir)
        {
            return IsOn && dir == Common.NextDirCW(rotation);
        }

        public Portal(TextureID[] tid, Tile parent) : base(tid, parent)
        {

        }
    }
}