using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inlumino_SHARED
{
    internal class TextureID
    {
        internal string RefKey;
        internal int Index;
        internal float WidthUnits;
        internal float HeightUnits;

        // Actual size on stage
        internal static int UnitSizeX2D = 128;
        internal static int UnitSizeY2D = 128;

        internal TextureID(string name, int idx, float wunits = 1, float hunits = 1)
        {
            this.RefKey = name;
            this.Index = idx;
            this.WidthUnits = wunits;
            this.HeightUnits = hunits;
        }
        internal TextureID(Func<Texture2D> texture, string name, int idx, float wunits = 1, float hunits = 1)
        {
            DataHandler.LoadTexture(name, texture);
            this.RefKey = name;
            this.Index = idx;
            Texture2D id = DataHandler.Textures[name];
            this.WidthUnits = wunits > 0 ? wunits : id.Width / DataHandler.TextureUnitDim;
            this.HeightUnits = hunits > 0 ? hunits : id.Height / DataHandler.TextureUnitDim;
        }
        internal float TotalHeight { get { return HeightUnits * DataHandler.TextureUnitDim; } }
        internal float TotalWidth { get { return WidthUnits * DataHandler.TextureUnitDim; } }
        internal Vector2 Center { get { return new Vector2(TotalWidth / 2, TotalHeight / 2); } }
    }
}
