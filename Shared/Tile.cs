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
        public RectangleF Bounds2D;
        public TileType Type;
        public TileState CurrentState = TileState.Default;
        public OverlayEffect ActiveEffect;
        private Stage parentstage;

        bool adjdi = false;
        Tile[] adj = null;
        public Tile[] getAdjacentTiles(bool includediagonals)
        {
            // DP caching because the function is used alot per frame.            
            return adj != null && (adjdi == includediagonals) ? adj : adj = parentstage.getAdjacentTiles(this, adjdi = includediagonals);
        }

        private StaticObject obj = null;
        public Color HighlightColor = Color.Cyan;

        int columncount = 0;

        public TextureID[] TextureID;

        public Point MapPos { get { return mappos; } }

        public int ColumnCount { get { return columncount; } }

        public Tile RightAdj { get { return getAdjacentTiles(adjdi).FirstOrDefault(t => t.Bounds2D.X > Bounds2D.X); } }

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

        public Tile LeftAdj { get { return getAdjacentTiles(adjdi).FirstOrDefault(t => t.Bounds2D.X < Bounds2D.X); } }
        public Tile TopAdj { get { return getAdjacentTiles(adjdi).FirstOrDefault(t => t.Bounds2D.Y < Bounds2D.Y); } }
        public Tile BottomAdj { get { return getAdjacentTiles(adjdi).FirstOrDefault(t => t.Bounds2D.Y > Bounds2D.Y); } }

        public Vector2 LocalCenter { get { return Center - Bounds2D.Location; } }

        public Vector2 Center { get { return Bounds2D.getRectangle().Center.ToVector2(); } }

        public Stage Parent
        {
            get { return parentstage; }
        }

        public Tile(TileType type, RectangleF bounds, Stage parent, Point mappos)
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

        internal void Highlight()
        {
            ActiveEffect = OverlayEffect.Highlighted;
        }
        internal void UnHighlight()
        {
            ActiveEffect = OverlayEffect.None;
        }
        internal void Draw(SpriteBatch batch, Camera cam, Vector2 coordOrigin)
        {
            batch.Draw(DataHandler.getTexture(TextureID[0].GroupIndex), cam.Transform(Bounds2D).getSmoothRectangle(cam.GetRecommendedDrawingFuzz() / 2 /*on both sides*/), DataHandler.getTextureSource(TextureID[0]), ActiveEffect == OverlayEffect.Highlighted ? HighlightColor : Color.White);//White for no tinting            
            if (hasObject())
                obj.Draw(batch, cam, coordOrigin);            
            if (ActiveEffect == OverlayEffect.Highlighted)
                batch.Draw(DataHandler.getTexture(TextureID[1].GroupIndex), cam.Transform(Bounds2D).getSmoothRectangle(cam.GetRecommendedDrawingFuzz() / 2 /*on both sides*/), DataHandler.getTextureSource(TextureID[1]), Color.White);
        }

        internal void Update(GameTime time)
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
        public bool hasObject(Type type = null)
        { return obj != null && (type == null || type == obj.GetType()); }
        internal bool hasObject<T>()
        { return hasObject() && obj is T; }

        internal StaticObject getObject()
        {
            return obj;
        }
    }

    enum OverlayEffect { None, Highlighted }
    enum TileState { Default = 0, Glowing = 1 }

    public enum TileType { Unknown = 0, Default = 1 }
}
