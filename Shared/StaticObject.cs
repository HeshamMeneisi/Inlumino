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

        protected int state = 0; // To be mapped in an enum in every child        

        protected Direction rotation { get; private set; }

        protected Direction targetrotation { get; private set; }

        public Direction Rotation { get { return rotation; } set { rotation = targetrotation = value; } }

        public Tile ParentTile { get { return parenttile; } }

        public OverlayEffect ActiveEffect = OverlayEffect.None;

        public Color HighlightColor = Color.AliceBlue;

        public StaticObject(TextureID[] tid, Tile tile)
        {
            rotation = targetrotation = Direction.North;
            parenttile = tile;
            tID = tid;
        }

        internal void Draw(SpriteBatch batch, Camera cam, Vector2 coordOrigin)
        {
            batch.Draw(DataHandler.getTexture(tID[state].GroupIndex), cam.Transform(parenttile.Bounds2D.Offset(parenttile.LocalCenter)).getSmoothRectangle(cam.GetRecommendedDrawingFuzz() / 2 /*on both sides*/), DataHandler.getTextureSource(tID[state]), ActiveEffect == OverlayEffect.Highlighted ? HighlightColor : Color.White, getRotationAngle(), tID[state].Center - new Vector2(1, 1), SpriteEffects.None, 0);//White for no tinting
        }

        private float getRotationAngle()
        {
            return (float)rotation * MathHelper.PiOver2;
        }

        public virtual void Update(GameTime time)
        {
            if (rotation != targetrotation) rotation = targetrotation;
            // Animation will be handled here
        }

        // This is for the datahandler, we could just use instanceof when the type is needed
        public abstract ObjectType getType();

        // To be handled by childeren on update
        public virtual void RotateCW(bool instant, int clicks = 1)
        {
            targetrotation = Common.NextDirCW(rotation, clicks);
            if (instant) rotation = targetrotation;
            else SoundManager.PlaySound(DataHandler.Sounds[SoundType.RotateSound], SoundCategory.SFX);
        }

        public virtual void RotateCCW(bool instant, int clicks = 1)
        {
            targetrotation = Common.NextDirCCW(rotation, clicks);
            if (instant) rotation = targetrotation;
            else SoundManager.PlaySound(DataHandler.Sounds[SoundType.RotateSound], SoundCategory.SFX);
        }
    }
}
/*For saving/loading levels*/
public enum ObjectType
{
    Default = -2, Delete = -3, // Exclusive to editor
    LightBeam = -1, None = 0,
    Prism = 1, Block = 2,
    LightSource = 3,
    Crystal = 4,
    Splitter = 5,
    Invisible = 6
}
enum Direction { North = 0, East = 1, South = 2, West = 3 }