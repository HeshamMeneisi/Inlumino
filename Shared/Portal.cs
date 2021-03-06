﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Inlumino_SHARED
{
    class Portal : StaticObject, IObstructingObject, ILightSource
    {
        List<ILightSource> currentnearbysources = new List<ILightSource>();
        List<ILightSource> currentdistantsources = new List<ILightSource>();
        Dictionary<ILightSource, Direction> allsources = new Dictionary<ILightSource, Direction>();

        internal override void Update(GameTime time)
        {
            if (rotation != targetrotation)
            {
                if (IsOn)
                {
                    Tile target = parenttile.getAdjacentTile(Common.NextDirCW(rotation));
                    if (target != default(Tile)) Common.PulseTile(target, false, Common.ReverseDir(Common.NextDirCW(rotation)), this);
                }
                // Simulate current source off
                while (currentnearbysources.Count > 0)
                {
                    try
                    {
                        ILightSource temp = currentnearbysources[0];
                        HandlePulse(false, Common.NextDirCW(rotation), currentnearbysources[0]);
                        allsources.Add(temp, Common.NextDirCW(rotation));
                    }
                    catch { }
                }
                Rotation = targetrotation;
                // Rehandle everything currently on
                try
                {
                    foreach (KeyValuePair<ILightSource, Direction> pair in allsources)
                        HandlePulse(true, pair.Value, pair.Key);
                }
                catch { /* Already found our working source.*/}
                if (IsOn)
                {
                    Tile target = parenttile.getAdjacentTile(Common.NextDirCW(rotation));
                    if (target != default(Tile)) Common.PulseTile(target, true, Common.ReverseDir(Common.NextDirCW(rotation)), this);
                }
            }
            base.Update(time);
        }
        public bool IsOn
        {
            get
            {
                return currentdistantsources.Count > 0;
            }
        }

        internal override ObjectType getType()
        {
            return ObjectType.Portal;
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
        private void PowerUp(Portal source)
        {
            if (source == this) return;
            state = 1;
            bool flag = IsOn;
            if (!currentdistantsources.Contains(source))
                currentdistantsources.Add(source);
            if (flag) return; // Avoid the nasty stack overflow that could happen here
            Tile target = parenttile.getAdjacentTile(Common.NextDirCW(rotation));
            if (target != default(Tile)) Common.PulseTile(target, true, Common.ReverseDir(Common.NextDirCW(rotation)), this);            
        }
        private void PowerOff(Portal source)
        {
            if (source == this) return;
            if (currentdistantsources.Contains(source))
                currentdistantsources.Remove(source);
            if (currentdistantsources.Count == 0)
            {
                Tile target = parenttile.getAdjacentTile(Common.NextDirCW(rotation));
                if (target != default(Tile)) Common.PulseTile(target, false, Common.ReverseDir(Common.NextDirCW(rotation)), this);
                if (currentnearbysources.Count == 0)
                    state = 0;
            }
        }

        internal bool IsFeedingDirection(Direction dir)
        {
            return IsOn && dir == Common.NextDirCW(rotation);
        }

        public void HandlePulse(bool charge, Direction dir, ILightSource source)
        {
            if (charge)
            {
                if (!allsources.ContainsKey(source))
                    allsources.Add(source, dir);
                if (dir == Common.NextDirCW(rotation))
                {
                    if (!currentnearbysources.Contains(source)) currentnearbysources.Add(source);
                    state = 1;
                    foreach (Tile t in getCrossTiles())
                        if (t.hasObject<Portal>())
                            (t.getObject() as Portal).PowerUp(this);
                }
            }
            else
            {
                if (allsources.ContainsKey(source))
                    allsources.Remove(source);
                if (currentnearbysources.Contains(source))
                {
                    currentnearbysources.Remove(source);
                    if (currentnearbysources.Count == 0)
                    {
                        foreach (Tile t in getCrossTiles())
                            if (t.hasObject<Portal>())
                                (t.getObject() as Portal).PowerOff(this);
                        if (currentdistantsources.Count == 0)
                            state = 0;
                    }
                }
            }
        }

        public void Reset()
        {
            state = 0;
            currentdistantsources.Clear();
            currentnearbysources.Clear();
            allsources.Clear();
        }

        internal Portal(TextureID[] tid, Tile parent) : base(tid, parent)
        {
            IsInteractable = true;
        }
    }
}