using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inlumino_SHARED
{
    internal class LevelData
    {
        // internal for the serializer
        internal int width;

        internal string tmap;

        internal string omap;

        internal string rmap;

        internal string Data {
            get
            {
                return SecurityProvider.Encrypt(width + "|" + tmap + "|" + omap + "|" + rmap);
            }
            private set
            {
                string[] data = SecurityProvider.Decrypt(value).Split('|');
                width = int.Parse(data[0]);
                tmap = data[1];
                omap = data[2];
                rmap = data[3];
            }
        }

        internal int[,] getTileMap()
        {
            return getMatrixFromArray(tmap.Split(',').Select((v) => Convert.ToInt32(v)).ToArray(), width);
        }
        internal void setTileMap(int[,] value) { tmap = String.Join(",", getArrayFromMatrix(value)); }
        internal int[,] getObjMap()
        {
            return getMatrixFromArray(omap.Split(',').Select((v) => Convert.ToInt32(v)).ToArray(), width);
        }
        internal void setObjMap(int[,] value) { omap = string.Join(",", getArrayFromMatrix(value)); }
        internal int[,] getRotationMap()
        {
            return getMatrixFromArray(rmap.Split(',').Select((v) => Convert.ToInt32(v)).ToArray(), width);
        }
        internal void setRotationMap(int[,] value) { rmap = string.Join(",", getArrayFromMatrix(value)); }

        internal static int[] getArrayFromMatrix(int[,] mat)
        {
            int[] temp = new int[mat.GetLength(0) * mat.GetLength(1)+1];
            int w = temp[0] = mat.GetLength(1);
            for (int i = 0; i < mat.GetLength(0); i++)
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    temp[i * w + j + 1] = mat[i, j];
                }
            return temp;
        }

        internal static int[,] getMatrixFromArray(int[] array, int w)
        {
            w = array[0];
            int[,] temp = new int[array.Length / w, w];            
            for (int i = 0; i < (array.Length-1); i++)
                temp[i / w, i % w] = array[i+1];
            return temp;
        }

        internal LevelData()
        { }
        internal LevelData(int[,] tiles, int[,] objects, int[,] rotations)
        {
            width = tiles.GetLength(1);
            setTileMap(tiles);
            setObjMap(objects);
            setRotationMap(rotations);
        }
        private LevelData(string data)
        {
            Data = data;
        }

        internal static LevelData CreateNew(string data)
        {
            try
            {
                return new LevelData(data);
            }
            catch { return default(LevelData); }
        }
    }
}
