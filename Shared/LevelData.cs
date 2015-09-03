using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inlumino_SHARED
{
    public class LevelData
    {
        // public for the serializer
        public int width;

        public string tmap;

        public string omap;

        public string rmap;

        public string Data {
            get
            {
                return width + "$" + tmap + "$" + omap + "$" + rmap;
            }
            private set
            {
                string[] data = value.Split('$');
                width = int.Parse(data[0]);
                tmap = data[1];
                omap = data[2];
                rmap = data[3];
            }
        }

        public int[,] getTileMap()
        {
            return getMatrixFromArray(Encoding.ASCII.GetString(Convert.FromBase64String(tmap)).Split('#').Select((v) => Convert.ToInt32(v)).ToArray(), width);
        }
        public void setTileMap(int[,] value) { tmap = Convert.ToBase64String(Encoding.ASCII.GetBytes(String.Join("#", getArrayFromMatrix(value)))); }
        public int[,] getObjMap()
        {
            return getMatrixFromArray(Encoding.ASCII.GetString(Convert.FromBase64String(omap)).Split('#').Select((v) => Convert.ToInt32(v)).ToArray(), width);
        }
        public void setObjMap(int[,] value) { omap = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Join("#", getArrayFromMatrix(value)))); }
        public int[,] getRotationMap()
        {
            return getMatrixFromArray(Encoding.ASCII.GetString(Convert.FromBase64String(rmap)).Split('#').Select((v) => Convert.ToInt32(v)).ToArray(), width);
        }
        public void setRotationMap(int[,] value) { rmap = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Join("#", getArrayFromMatrix(value)))); }

        public static int[] getArrayFromMatrix(int[,] mat)
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

        public static int[,] getMatrixFromArray(int[] array, int w)
        {
            w = array[0];
            int[,] temp = new int[array.Length / w, w];            
            for (int i = 0; i < (array.Length-1); i++)
                temp[i / w, i % w] = array[i+1];
            return temp;
        }

        public LevelData()
        { }
        public LevelData(int[,] tiles, int[,] objects, int[,] rotations)
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

        public static LevelData CreateNew(string data)
        {
            try
            {
                return new LevelData(data);
            }
            catch { return default(LevelData); }
        }
    }
}
