using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inlumino_SHARED
{
    // This should be inherited to create objects
    abstract class StaticObject
    {
        protected TextureID[] tID;

        protected Tile parenttile;

        float smoothrotation = 0;

        protected int state = 0; // To be mapped in an enum in every child        

        protected Direction rotation { get; private set; }

        protected Direction targetrotation { get; private set; }

        internal Direction Rotation { get { return rotation; } set { rotation = targetrotation = value; } }

        public Tile ParentTile { get { return parenttile; } }

        internal OverlayEffect ActiveEffect = OverlayEffect.None;

        internal Color HighlightColor = Color.AliceBlue;
        public bool IsInteractable { get; set; }
        internal StaticObject(TextureID[] tid, Tile tile)
        {
            rotation = targetrotation = Direction.North;
            parenttile = tile;
            tID = tid;
            IsInteractable = false;
        }

        public void Draw(SpriteBatch batch, Camera cam, Vector2 coordOrigin)
        {
            if(this is IObstructingObject)
                batch.Draw(DataHandler.getTexture(tID[state]), cam.Transform(parenttile.Bounds2D.Offset(parenttile.LocalCenter)).getSmoothRectangle(cam.GetRecommendedDrawingFuzz() / 2 /*on both sides*/), DataHandler.getTextureSource(parenttile.TextureID[2]), ActiveEffect == OverlayEffect.Highlighted ? HighlightColor : Color.White, getRotationAngle(), parenttile.TextureID[2].Center - new Vector2(1, 1), SpriteEffects.None, 0);//White for no tinting
            batch.Draw(DataHandler.getTexture(tID[state]), cam.Transform(parenttile.Bounds2D.Offset(parenttile.LocalCenter)).getSmoothRectangle(cam.GetRecommendedDrawingFuzz() / 2 /*on both sides*/), DataHandler.getTextureSource(tID[state]), ActiveEffect == OverlayEffect.Highlighted ? HighlightColor : Color.White, getRotationAngle(), tID[state].Center - new Vector2(1, 1), SpriteEffects.None, 0);//White for no tinting
        }
        float sfactor = 0;
        private float getRotationAngle()
        {
            float proper = (float)rotation * MathHelper.PiOver2;
            if (!Common.isSameAngle(proper, smoothrotation, Math.Abs(sfactor)) && sfactor != 0)
                smoothrotation += sfactor;
            else
            { smoothrotation = proper; rotating = false; }
            return smoothrotation;
        }

        internal virtual void Update(GameTime time)
        {
            if (rotation != targetrotation) rotation = targetrotation;
            // Animation will be handled here
        }

        // This is for the datahandler, we could just use instanceof when the type is needed
        internal abstract ObjectType getType();
        bool rotating = false;
        // To be handled by childeren on update
        internal virtual void RotateCW(bool instant, int clicks = 1)
        {
            rotating = true;
            targetrotation = Common.NextDirCW(rotation, clicks);
            if (instant) { rotation = targetrotation; smoothrotation = (float)rotation * MathHelper.PiOver2; }
            else
            {
                sfactor = 0.2f; SoundManager.PlaySound(DataHandler.Sounds[SoundType.RotateSound], SoundCategory.SFX);
            }
        }

        internal virtual void RotateCCW(bool instant, int clicks = 1)
        {
            rotating = true;
            targetrotation = Common.NextDirCCW(rotation, clicks);
            if (instant) { rotation = targetrotation; smoothrotation = (float)rotation * MathHelper.PiOver2; }
            else
            {
                sfactor = -0.2f; SoundManager.PlaySound(DataHandler.Sounds[SoundType.RotateSound], SoundCategory.SFX);
            }
        }
    }
}
/*For saving/loading levels*/
internal enum ObjectType
{
    Default = -2, Delete = -3, // Exclusive to editor
    LightBeam = -1, None = 0,
    Prism = 1, Block = 2,
    LightSource = 3,
    Crystal = 4,
    Splitter = 5,
    Invisible = 6,
    Portal = 7,
    FourWay = 8
}
enum Direction { North = 0, East = 1, South = 2, West = 3 }