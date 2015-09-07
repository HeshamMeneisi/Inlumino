using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlumino_SHARED
{
    class TileMap
    {
        // List of rows
        private List<List<Tile>> map = new List<List<Tile>>();

        public int Width { get { return map.Count == 0 ? 0 : map.Max(t => t.Count); } }
        public int Height { get { return map.Count; } }

        public IEnumerable<Tile> AllTiles
        {
            get
            {
                foreach (List<Tile> row in map)
                {
                    foreach (Tile t in row)
                        yield return t;
                }
            }
        }
        public TileMap(int[,] intmap, Stage parent)
        {
            if (intmap == null) return;
            for (int row = 0; row < intmap.GetLength(0); row++)
            {
                Tile[] rowtiles = new Tile[intmap.GetLength(1)];
                for (int col = 0; col < rowtiles.Length; col++)
                    rowtiles[col] = new Tile((TileType)intmap[row, col], getTileBounds(row, col), parent, new Point(row, col));
                AddRow(rowtiles);
            }
        }

        public TileMap(int rows, int cols, Stage parent)
        {
            for (int r = 0; r < rows; r++)
            {
                AddDefaultRow(parent, cols);
            }
        }

        public void AddRow(Tile[] row, int pos = -1)
        {
            if (pos >= 0)
                map.Insert(pos, row.ToList());
            else
                map.Add(row.ToList());
        }
        public void AddDefaultRow(Stage parent, int cols)
        {
            Tile[] row = new Tile[cols];
            int r = Height;
            for (int c = 0; c < cols; c++)
                row[c] = new Tile(TileType.Default, getTileBounds(r, c), parent, new Point(r, c));
            AddRow(row);
        }
        public void AddCol(Tile[] col, int pos = -1)
        {
            if (pos >= 0)
                for (int i = 0; i < map.Count; i++)
                {
                    Stage parent = map.Count > 0 ? map[0][0].Parent : null;
                    while (map[i].Count < pos)
                        map[i].Add(new Tile(TileType.Default, getTileBounds(i, map[i].Count), parent, new Point(i, map[i].Count)));
                    if (map[i].Count == pos) map[i].Add(col[i]);
                    else map[i].Insert(pos, col[i]);
                }
            else
                for (int i = 0; i < map.Count; i++)
                {
                    map[i].Add(col[i]);
                }
        }
        public void AddDefaultCol(Stage parent, int rows)
        {
            Tile[] col = new Tile[rows];
            int c = Width;
            for (int r = 0; r < rows; r++)
                col[r] = new Tile(TileType.Default, getTileBounds(r, c), parent, new Point(r, c));
            AddCol(col);
        }

        private RectangleF getTileBounds(int row, int col)
        {
            return new RectangleF(col * TextureID.UnitSizeX2D, row * TextureID.UnitSizeY2D, TextureID.UnitSizeX2D, TextureID.UnitSizeY2D);
        }

        public Tile getTileAt(int row, int col)
        {
            return row < Height && col < Width && row >= 0 && col >= 0 ? map[row][col] : default(Tile);
        }

        public Tile[] getAdjacentsOf(Tile t1, bool includeDiagonals)
        {
            int row = t1.MapPos.X, col = t1.MapPos.Y;
            List<Tile> adj = new List<Tile>();
            adj.Add(getTileAt(row + 1, col));
            adj.Add(getTileAt(row, col + 1));
            adj.Add(getTileAt(row - 1, col));
            adj.Add(getTileAt(row, col - 1));
            if (includeDiagonals)
            {
                adj.Add(getTileAt(row + 1, col + 1));
                adj.Add(getTileAt(row + 1, col - 1));
                adj.Add(getTileAt(row - 1, col - 1));
                adj.Add(getTileAt(row - 1, col - 1));
            }
            adj.RemoveAll(t => t == default(Tile));
            return adj.ToArray();
        }

        public int[,] getIntMap()
        {
            int[,] intmap = new int[Height, Width];
            for (int row = 0; row < Height; row++)
                for (int col = 0; col < Width; col++)
                    intmap[row, col] = (int)getTileAt(row, col).Type;
            return intmap;
        }

        internal int[,] getObjectRotationMap()
        {
            int[,] intmap = new int[Height, Width];
            for (int row = 0; row < Height; row++)
                for (int col = 0; col < Width; col++)
                {
                    StaticObject obj = getTileAt(row, col).getObject();
                    intmap[row, col] = (int)(obj == default(StaticObject) ? 0 : obj.Rotation);
                }
            return intmap;
        }

        internal int[,] getObjectMap()
        {
            int[,] intmap = new int[Height, Width];
            for (int row = 0; row < Height; row++)
                for (int col = 0; col < Width; col++)
                {
                    StaticObject obj = getTileAt(row, col).getObject();
                    intmap[row, col] = (int)(obj == default(StaticObject) ? ObjectType.None : obj.getType());
                }
            return intmap;
        }

        internal void RemoveLastRow()
        {
            if (Height > 0) map.RemoveAt(map.Count - 1);
        }

        internal void RemoveLastCol()
        {
            if (Width > 0)
            {
                foreach (List<Tile> row in map)
                    row.RemoveAt(row.Count - 1);
            }
        }

        public IEnumerable<Tile> getRow(int row)
        {
            return row < map.Count ? map[row] : null;
        }
        public IEnumerable<Tile> getColumn(int col)
        {
            foreach (List<Tile> row in map)
                yield return col < row.Count ? row[col] : null;
        }
    }
}
