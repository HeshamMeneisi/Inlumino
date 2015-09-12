using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inlumino_SHARED
{
    public class TextureID
    {        
        public int GroupIndex;
        public int Index;
        public float WidthUnits;
        public float HeightUnits;

        // Actual size on stage
        public static int UnitSizeX2D = 128;
        public static int UnitSizeY2D = 128;        

        public TextureID(int groupIndex, int idx, float wunits=1, float hunits=1)
        {
            this.GroupIndex = groupIndex;
            this.Index = idx;
            this.WidthUnits = wunits;
            this.HeightUnits = hunits;
        }
        public TextureID(string name, int idx, float wunits = 1, float hunits = 1)
        {
            this.GroupIndex = DataHandler.getGroupIndexFromName(name);
            this.Index = idx;
            this.WidthUnits = wunits;
            this.HeightUnits = hunits;
        }
        public TextureID(Texture2D texture,string name, int idx, float wunits = 1, float hunits = 1)
        {
            this.GroupIndex = DataHandler.getGroupIndexFromName(name, texture);
            this.Index = idx;
            this.WidthUnits = wunits;
            this.HeightUnits = hunits;
        }
        public float TotalHeight { get { return HeightUnits *  DataHandler.TextureUnitDim; } }
        public float TotalWidth { get { return WidthUnits * DataHandler.TextureUnitDim; } }
        public Vector2 Center { get { return new Vector2(TotalWidth / 2, TotalHeight / 2); } }
    }
}
