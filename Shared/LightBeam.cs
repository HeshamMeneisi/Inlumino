using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlumino_SHARED
{
    class LightBeam : StaticObject, ILightSource
    {
        private BeamType type;
        public Direction HorzDirection { get; set; }
        public Direction VerticalDirection { get; set; }
        public BeamType BeamState
        {
            get { return type; }
            set { type = value; if (value == BeamType.Cross) state = 1; else { state = 0; Rotation = value == BeamType.Horizontal ? Direction.North : Direction.East; } }
        }
        bool on = true;
        public bool IsOn
        {
            get
            {
                return on;
            }
        }

        internal bool isHorizontal()
        {
            return type == BeamType.Horizontal || type == BeamType.Cross;
        }

        internal bool isVertical()
        {
            return type == BeamType.Vertical || type == BeamType.Cross;
        }

        public override ObjectType getType()
        {
            return ObjectType.LightBeam;
        }

        public LightBeam(TextureID[] tid, Tile parent, BeamType type, Direction hd, Direction vd) : base(tid, parent)
        {
            BeamState = type;
            HorzDirection = hd;
            VerticalDirection = vd;
        }

        public void Activate()
        {
            // Horizontal
            if (isHorizontal())
            {
                Tile target;
                if (HorzDirection == Direction.East)
                    target = parenttile.RightAdj;
                else
                    target = parenttile.LeftAdj;

                Common.PowerUpTile(target, Common.ReverseDir(HorzDirection), this);
            }
            // Vertical
            if (isVertical())
            {
                Tile target;
                if (VerticalDirection == Direction.South)
                    target = parenttile.BottomAdj;
                else
                    target = parenttile.TopAdj;
                Common.PowerUpTile(target, Common.ReverseDir(VerticalDirection), this);
            }
        }

        public void DeleteVertical()
        {
            if (BeamState == BeamType.Cross)
                BeamState = BeamType.Horizontal;
            else if (this.type == BeamType.Vertical)
            { parenttile.RemoveObject(); on = false; }
            Tile target;
            if (VerticalDirection == Direction.South)
                target = parenttile.BottomAdj;
            else
                target = parenttile.TopAdj;
            Common.PowerOffTile(target, Common.ReverseDir(VerticalDirection), this);
        }

        public void DeleteHorizontal()
        {
            if (BeamState == BeamType.Cross)
                BeamState = BeamType.Vertical;
            else if (this.type == BeamType.Horizontal)
            { parenttile.RemoveObject(); on = false; }
            Tile target;
            if (HorzDirection == Direction.East)
                target = parenttile.RightAdj;
            else
                target = parenttile.LeftAdj;
            Common.PowerOffTile(target, Common.ReverseDir(HorzDirection), this);
        }

        public bool IsFeedingDirection(Direction dir)
        {
            switch (dir)
            {
                case Direction.North: return isVertical() && VerticalDirection == Direction.North;
                case Direction.East: return isHorizontal() && HorzDirection == Direction.East;
                case Direction.South: return isVertical() && VerticalDirection == Direction.South;
                case Direction.West: return isHorizontal() && HorzDirection == Direction.West;
                default: return false; //unreachable
            }
        }
    }
    // state should be set to this and textures linked
    enum BeamType { Horizontal = 0, Vertical = 1, Cross = 2 }
}
