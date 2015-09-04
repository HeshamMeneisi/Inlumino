using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Inlumino_SHARED
{
    class UIVisibleObject : UIObject
    {
        // First, you can switch to RectangleF for more accuracy.
        protected TextureID[] sprite;
        protected Vector2 size = Vector2.Zero; // I told you the size must be custom, texture size is irrelevant to actual size :P
        protected Vector2 origin = Vector2.Zero;

        protected int state = 0;
        public UIVisibleObject(TextureID[] tid, int layer = 0, string id = "")
        : base(layer, id)
        {
            // Use TextureID to describe the texture.   new VisibleUIObject(new TextureID(
            // int groupIndex   // Filename index in the array Datahandler.TextureFiles
            // , int idx        // Index in the sheet
            //  , int wunits=1, int hunits=1  width and hight units, default 1*1))

            if (!DataHandler.isValid(tid[0])) sprite = null;
            else
            {
                sprite = tid;
                size = new Vector2(tid[0].TotalWidth, tid[0].TotalHeight);
            }
        }

        public override void Draw(SpriteBatch batch) // You don't need a camera, draw ontop of the stage
        {
            if (!visible || sprite == null)
                return;
            batch.Draw(DataHandler.getTexture(sprite[state].GroupIndex)/*Texture2D from file*/, BoundingBox.getRectangle()/*on-screen box*/, DataHandler.getTextureSource(sprite[state])/*Rectange on the sheet*/, Color.White/*white=no tint*/);
        }

        public Vector2 Center
        {
            // Real center is found this way (on screen)
            get { return BoundingBox.Center; }
        }

        public Vector2 Size
        {
            get { return size; }
            set { size = value; }
        }

        public Vector2 Origin
        {
            get { return this.origin; }
            set { this.origin = value; }
        }

        public override RectangleF BoundingBox
        {
            get
            {
                float left = (int)(GlobalPosition.X - origin.X);
                float top = (int)(GlobalPosition.Y - origin.Y);
                return new RectangleF(new Vector2(left, top), size);
            }
        }

        public void setSizeRelativeToWidth(float perc)
        {
            float w = Screen.Width * perc;
            float h = size.Y / size.X * w;
            size = new Vector2(w, h);
        }
        public void setSizeRelativeToHeight(float perc)
        {
            float h = Screen.Height * perc;
            float w = size.X / size.Y * h;
            size = new Vector2(w, h);
        }

        public void setSizeRelative(float perc, Orientation mode)
        {
            if (mode == Orientation.Landscape)
                setSizeRelativeToHeight(perc);
            else
                setSizeRelativeToWidth(perc);
        }
    }
}
