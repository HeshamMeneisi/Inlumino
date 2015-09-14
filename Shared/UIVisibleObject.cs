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

        public int State { get { return state; } set { state = value; } }
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

        public virtual void Draw(SpriteBatch batch, Camera cam = null) // You don't need a camera, draw ontop of the stage
        {
            if (!visible || sprite == null)
                return;
            if (cam == null)
                batch.Draw(DataHandler.getTexture(sprite[state].RefKey)/*Texture2D from file*/, BoundingBox.ToRectangle()/*on-screen box*/, DataHandler.getTextureSource(sprite[state])/*Rectange on the sheet*/, Color.White/*white=no tint*/);
            else
            {
                if (cam.isInsideView(LocalBoundingBox))
                {
                    RectangleF nocrop;
                    RectangleF cropped = cam.TransformWithCropping(LocalBoundingBox, out nocrop);
                    RectangleF source = DataHandler.getTextureSource(sprite[state]);
                    // rect is an intersection of nocrop, thus contained by it
                    source = source.Mask(nocrop, cropped);
                    batch.Draw(DataHandler.getTexture(sprite[state].RefKey)/*Texture2D from file*/,
                        cropped.Offset(parent.GlobalPosition).ToRectangle()/*on-screen box*/,
                        source.ToRectangle()/*Rectange on the sheet*/,
                        Color.White/*white=no tint*/);
                }
            }
        }

        public Vector2 Center
        {
            // Real center is found this way (on screen)
            get { return BoundingBox.Center; }
        }

        public virtual float Width { get { return Size.X; } }
        public virtual float Height { get { return Size.Y; } }
        public virtual Vector2 Size
        {
            get { return size; }
            set { size = value; }
        }

        public Vector2 Origin
        {
            get { return this.origin; }
            set { this.origin = value; }
        }

        public RectangleF LocalBoundingBox
        {
            get
            {
                return new RectangleF(Position.X - Origin.X, Position.Y - Origin.Y, Size.X, Size.Y);
            }
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

        public virtual void setSizeRelative(float perc, Orientation mode)
        {
            if (mode == Orientation.Landscape)
                setSizeRelativeToHeight(perc);
            else
                setSizeRelativeToWidth(perc);
        }
        public override void HandleEvent(WorldEvent e)
        {
        }
    }
}
