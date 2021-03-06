﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Inlumino_SHARED
{
    class Stage
    {
        Texture2D background = null;
        RectangleF bgbounds;
        Camera maincam;

        TileMap map = null;

        internal bool LoadedUp { get { return map != null; } }
        internal int Width { get { return map.Width; } }
        internal int Height { get { return map.Height; } }

        private bool paused = false;

        private bool editmode = false;
        private Padding minpadding;

        internal Camera Camera { get { return maincam; } set { maincam = value; } }
        internal Vector2 Origin { get { return new Vector2(0, 0); } }

        internal bool IsPaused { get { return paused; } }

        internal bool EditMode { get { return editmode; } }

        internal bool CheckWin()
        {
            Crystal[] crystals = map.AllTiles.Select(t => t.getObject() as Crystal).ToArray();
            if (crystals.Length == 0) return false;
            foreach (Crystal c in crystals)
                if (c != default(Crystal))
                    if (!c.IsLit()) return false;
            // Level Won                        
            if (LevelWon != null) LevelWon();
            return true;
        }
        internal delegate void LevelWonEventHandler();
        internal event LevelWonEventHandler LevelWon;
        internal Stage(Padding minpad = null, int[,] map = null)
        {
            this.map = new TileMap(map, this);
            if (minpad == null) minpadding = new Padding(0, 0, 0, 0);
            else minpadding = minpad;
            SetupCamera();
        }


        internal Stage(LevelData temp, Padding pad = null)
        {
            minpadding = new Padding(0, 0, 0, 0);
            if (temp == null) Debug.WriteLine("Attempted to load file from null data.");
            else
            {
                map = new TileMap(temp.getTileMap(), this);
                int[,] objmap = temp.getObjMap();
                int[,] rotmap = temp.getRotationMap();
                SetupCamera();
                for (int row = 0; row < objmap.GetLength(0); row++)
                    for (int col = 0; col < objmap.GetLength(1); col++)
                    {
                        Tile target = map.getTileAt(row, col);
                        ObjectType type = (ObjectType)objmap[row, col];
                        StaticObject obj = StaticObjectCreator.CreateObject(type, target);
                        target.SetObject(obj);
                        if (obj != default(StaticObject)) obj.RotateCW(true, rotmap[row, col]);
                    }
            }
        }

        internal void SetMinScreenPadding(Padding padding)
        {
            minpadding = padding;
            SetupCamera();
        }

        private void SetupCamera()
        {
            if (!LoadedUp) return;
            // Display
            int stgH = GetTotalStageHeight();
            int stgW = GetTotalStageWidth();
            float cw, ch;
            // Make sure the entire stage can be visible at max zoom
            /*
            Algorithm 1
            // 1 set camera to stage size
            cw = stgW; ch = stgH;
            // 2 normalize aspect ratio through expansion only
            Padding pad = new Padding();
            Padding min = minpadding.Clone();
            if (Screen.Width > Screen.Height)
            {
                cw = ch * Screen.Width / Screen.Height;
                float dif = Math.Max(cw - stgW, ch - stgH);
                min.Scale(cw / Screen.Width, ch / Screen.Height);
                pad = new Padding(MathHelper.Max(dif / 2, min.Left), MathHelper.Max(dif / 2, min.Right), min.Top, min.Bottom);
            }
            else if (Screen.Width < Screen.Height)
            {
                ch = cw * Screen.Height / Screen.Width;
                float dif = Math.Max(cw - stgW, ch - stgH);
                min.Scale(cw / Screen.Width, ch / Screen.Height);
                pad = new Padding(min.Left, min.Right, MathHelper.Max(dif / 2, min.Top), MathHelper.Max(dif / 2, min.Bottom));
            }
            */
            // Algorithm 2 (better)
            Padding pad = new Padding();
            Padding min = minpadding.Clone();            
            float ar = Screen.Width / Screen.Height;
            // Set camera width to stage width and update height
            cw = stgW;
            ch = cw / ar;
            // Check current case (one of the three is undesirable)
            if(ch < stgH) // reverse
            {
                ch = stgH;
                cw = ch * ar;
            }
            float xdif = (cw - stgW) / 2, ydif = (ch - stgH) / 2;
            min.Scale(cw / Screen.Width, ch / Screen.Height);
            pad = new Padding(Math.Max(min.Left, xdif), Math.Max(min.Right, xdif), Math.Max(min.Top, ydif), Math.Max(min.Bottom, ydif));

            maincam = new Camera(-pad.Left, -pad.Top, cw, ch, stgW, stgH, pad);
            updateBackgroundBounds();
        }
        public void Draw(SpriteBatch batch)
        {
            // First, draw the background
            if (background != null)
                batch.Draw(background, maincam.Transform(bgbounds).getSmoothRectangle(maincam.GetRecommendedDrawingFuzz()), Color.White);
            // Then tiles
            foreach (Tile t in map.AllTiles)
                if (maincam.isInsideView(t.Bounds2D))
                    t.Draw(batch, maincam, Origin);
        }
        internal virtual void Update(GameTime time)
        {
            maincam.Update(time);
            if (paused) return;
            foreach (Tile t in map.AllTiles) t.Update(time);
        }
        internal int GetTotalStageWidth()
        {
            return (int)(Width * TextureID.UnitSizeX2D);
        }

        internal int GetTotalStageHeight()
        {
            return (int)(Height * TextureID.UnitSizeY2D);
        }

        internal int[,] getObjectRotationMap()
        {
            return map.getObjectRotationMap();
        }

        internal bool hasMap()
        {
            return map != null;
        }

        internal Tile getTileAt(Vector2 p)
        {
            return map.AllTiles.Where(t => t.isPointOnSurface(p)).LastOrDefault();
        }

        internal Tile getTileAt(int row, int col)
        {
            if (!LoadedUp) return default(Tile);
            return map.getTileAt(row, col);
        }
        internal Tile[] getAdjacentTiles(Tile t1, bool includeDiagonals)
        {
            return map.getAdjacentsOf(t1, includeDiagonals);
        }

        internal int[,] getObjectMap()
        {
            return map.getObjectMap();
        }

        internal TileMap getTileMap()
        {
            return map;
        }

        internal void HighlightTileAt(Vector2 vector2)
        {
            foreach (Tile t in map.AllTiles)
            { t.RemoveEffect(OverlayEffect.Highlighted); }
            Tile temp = getTileAt(vector2);
            if (temp != null) temp.SetEffect(OverlayEffect.Highlighted);
        }
        bool gridon = false;
        internal void EnableGrid()
        {
            gridon = true;
            foreach (Tile t in map.AllTiles)
            { t.SetEffect(OverlayEffect.Grid); }
        }
        internal void DisableGrid()
        {
            gridon = false;
            foreach (Tile t in map.AllTiles)
            { t.RemoveEffect(OverlayEffect.Grid); }
        }
        internal void Pause()
        {
            paused = true;
        }
        internal void Resume()
        {
            paused = false;
        }

        internal void setBackground(Texture2D tex)
        {
            background = tex;
            updateBackgroundBounds();
        }

        private void updateBackgroundBounds()
        {
            if (Camera != null)
                bgbounds = new RectangleF(Camera.MinX, Camera.MinY, Camera.MaxW, Camera.MaxH);
        }

        /// 
        /// Editing Features Enabled
        /// getTileAt(point).Set/RemoveObject() can be used by the editing hud
        /// 
        internal void ToggleEditMode()
        {
            editmode = !editmode;
            if (editmode)
            {
                Pause();
                SetSourceStatus(false);
                ClearAllObjects(typeof(LightBeam));
                ResetAll();
                EnableGrid();
            }
            else
            {
                Resume();
                SetSourceStatus(true);
                DisableGrid();
            }
        }

        private void ResetAll()
        {
            foreach (Tile t in map.AllTiles)
            {
                IObstructingObject obj = t.getObject() as IObstructingObject;
                if (obj != default(IObstructingObject)) obj.Reset();
            }
        }

        internal void SetSourceStatus(bool s)
        {
            foreach (Tile t in map.AllTiles)
            {
                LightSourceObject obj = (t.getObject() as LightSourceObject);
                if (obj != default(StaticObject)) if (s) obj.TurnOn(); else obj.TurnOff();
            }
        }

        public void ResetCamera()
        {
            SetupCamera();
        }

        /// <summary>
        /// For edit mode only.
        /// </summary>
        /// <param name="w">Width in tiles</param>
        /// <param name="h">Height in tiles</param>
        internal void SetSize(int w, int h)
        {
            if (w <= 0 || h <= 0) return;
            if (map == null)
                map = new TileMap(h, w, this);
            else if (editmode)
            {
                while (h > map.Height)
                    map.AddDefaultRow(this, map.Width);
                while (w > map.Width)
                    map.AddDefaultCol(this, map.Height);
                while (h < map.Height)
                    map.RemoveLastRow();
                while (w < map.Width)
                    map.RemoveLastCol();
            }
            SetupCamera();
            if (gridon) EnableGrid();
        }
        internal void Clear()
        {
            if (editmode)
            {
                ClearAllObjects();
            }
        }

        private void ClearAllObjects(Type type = null)
        {
            foreach (Tile t in map.AllTiles)
                if (type == null || t.hasObject(type)) t.RemoveObject();
        }

        internal int ShuffleLevel()
        {
            Random ran = new Random();
            int moves = 0;
            foreach (Tile t in map.AllTiles)
                if (t.hasObject() && !(t.getObject() is LightBeam) && !(t.getObject() is LightSourceObject) && !(t.getObject() is Crystal) && !(t.getObject() is Block))
                    for (int i = ran.Next(1, 4); i > 0; i--)
                    {
                        moves++;
                        t.getObject().RotateCCW(true);
                    }
            return moves;
        }
    }
}