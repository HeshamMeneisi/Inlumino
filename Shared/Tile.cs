using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inlumino_SHARED
{
    class Tile
    {
        private Point mappos;
        internal RectangleF Bounds2D;
        internal TileType Type;
        internal TileState CurrentState = TileState.Default;
        internal OverlayEffect ActiveEffect;
        private Stage parentstage;

        RectangleF AuxRect;
        TextureID Auxiliary = null;

        bool adjdi = false;
        Tile[] adj = null;
        internal Tile[] getAdjacentTiles(bool includediagonals)
        {
            // DP caching because the function is used alot per frame.            
            return adj != null && (adjdi == includediagonals) ? adj : adj = parentstage.getAdjacentTiles(this, adjdi = includediagonals);
        }

        private StaticObject obj = null;
        internal Color HighlightColor = Color.Cyan;

        int columncount = 0;

        internal TextureID[] TextureID;

        internal Point MapPos { get { return mappos; } }

        internal int ColumnCount { get { return columncount; } }

        internal Tile RightAdj { get { return getAdjacentTiles(adjdi).FirstOrDefault(t => t.Bounds2D.X > Bounds2D.X); } }

        internal Tile getAdjacentTile(Direction direction)
        {
            switch (direction)
            {
                case Direction.North: return TopAdj;
                case Direction.East: return RightAdj;
                case Direction.South: return BottomAdj;
                case Direction.West: return LeftAdj;
                default: return default(Tile);
            }
        }

        internal Tile LeftAdj { get { return getAdjacentTiles(adjdi).FirstOrDefault(t => t.Bounds2D.X < Bounds2D.X); } }
        internal Tile TopAdj { get { return getAdjacentTiles(adjdi).FirstOrDefault(t => t.Bounds2D.Y < Bounds2D.Y); } }
        internal Tile BottomAdj { get { return getAdjacentTiles(adjdi).FirstOrDefault(t => t.Bounds2D.Y > Bounds2D.Y); } }

        internal Vector2 LocalCenter { get { return Center - Bounds2D.Location; } }

        internal Vector2 Center { get { return Bounds2D.ToRectangle().Center.ToVector2(); } }

        internal Stage Parent
        {
            get { return parentstage; }
        }

        internal Tile(TileType type, RectangleF bounds, Stage parent, Point mappos)
        {
            TileType[] valid = TileType.GetValues(typeof(TileType)).Cast<TileType>().ToArray();
            Type = valid.Contains(type) ? (TileType)type : TileType.Unknown;
            Bounds2D = bounds;
            TextureID = DataHandler.TileTextureMap[Type];
            parentstage = parent;
            this.mappos = mappos;
        }

        internal bool isPointOnSurface(Vector2 p)
        {
            return p.X < Bounds2D.Right && p.X > Bounds2D.Left && p.Y < Bounds2D.Bottom && p.Y > Bounds2D.Top;
        }

        internal void SetEffect(OverlayEffect effect)
        {
            ActiveEffect |= effect;
        }
        internal void RemoveEffect(OverlayEffect effect)
        {
            ActiveEffect &= ~effect;
        }
        public void Draw(SpriteBatch batch, Camera cam, Vector2 coordOrigin)
        {
            bool highlight = (ActiveEffect & OverlayEffect.Highlighted) > 0;
            bool grid = (ActiveEffect & OverlayEffect.Grid) > 0;
            batch.Draw(DataHandler.getTexture(TextureID[0].RefKey), cam.Transform(Bounds2D).getSmoothRectangle(cam.GetRecommendedDrawingFuzz() / 2 /*on both sides*/), DataHandler.getTextureSource(TextureID[0]), highlight ? HighlightColor : Color.White);//White for no tinting            
            if (Auxiliary != null)
                batch.Draw(DataHandler.getTexture(Auxiliary.RefKey), cam.Transform(AuxRect.Offset(Bounds2D.Location)).getSmoothRectangle(cam.GetRecommendedDrawingFuzz() / 2 /*on both sides*/), DataHandler.getTextureSource(Auxiliary), highlight ? HighlightColor : Color.White, auxrot, Auxiliary.Center, SpriteEffects.None, 0);//White for no tinting            
            if (hasObject())
                obj.Draw(batch, cam, coordOrigin);
            if (grid)
                batch.Draw(DataHandler.getTexture(TextureID[1].RefKey), cam.Transform(Bounds2D).getSmoothRectangle(cam.GetRecommendedDrawingFuzz() / 2 /*on both sides*/), DataHandler.getTextureSource(TextureID[1]), Color.White);
        }

        public void Update(GameTime time)
        {
            if (hasObject()) obj.Update(time);
        }
        internal void SetObject(StaticObject obj)
        {
            this.obj = obj;
        }
        internal void RemoveObject()
        {
            obj = null;
        }
        internal bool hasObject(Type type = null)
        { return obj != null && (type == null || type == obj.GetType()); }
        internal bool hasObject<T>()
        { return hasObject() && obj is T; }

        internal StaticObject getObject()
        {
            return obj;
        }
        static Random ran = new Random();
        float auxrot = 0;
        internal void SetAuxiliary(TextureID tid, RectangleF bounds = null)
        {
            Auxiliary = tid;
            if (bounds != null) AuxRect = bounds;
            else
            {
                float v = (float)MathHelper.Clamp((float)ran.NextDouble(), 0.2f, 1/(float)Math.Sqrt(2));
                Vector2 size = Bounds2D.Size * v;
                float maxw = size.X * (float)Math.Sqrt(2);
                Vector2 pos = new Vector2(ran.Next((int)(maxw / 2), (int)(Bounds2D.Width - maxw / 2)), ran.Next((int)(maxw / 2), (int)(Bounds2D.Height - maxw / 2)));
                auxrot = (float)(Math.PI * 2 * ran.NextDouble());
                AuxRect = new RectangleF(pos,size);
            }
        }
    }

    enum OverlayEffect { None = 0, Highlighted = 1, Grid = 2 }/*Powers of 2*/
    enum TileState { Default = 0, Glowing = 1 }

    internal enum TileType { Unknown = 0, Default = 1 }
}
