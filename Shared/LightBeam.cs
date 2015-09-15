using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlumino_SHARED
{
    class LightBeam : StaticObject
    {
        private BeamType type;

        private List<ILightSource> horzsources = new List<ILightSource>();
        private List<ILightSource> vertsources = new List<ILightSource>();
        internal BeamType BeamState
        {
            get { return type; }
            set { type = value; if (value == BeamType.Cross) state = 1; else { state = 0; Rotation = value == BeamType.Horizontal ? Direction.North : Direction.East; } }
        }

        internal void CarryPulse(bool charge, Direction dir, ILightSource source)
        {
            // Logic            
            if (charge)
            {
                if (Common.isDirVertical(dir) && !vertsources.Contains(source))
                    vertsources.Add(source);
                else if (Common.isDirHorizontal(dir) && !horzsources.Contains(source))
                    horzsources.Add(source);
            }
            else
            {
                if (Common.isDirVertical(dir) && vertsources.Contains(source))
                    vertsources.Remove(source);
                else if (Common.isDirHorizontal(dir) && horzsources.Contains(source))
                    horzsources.Remove(source);
            }
            if (horzsources.Count > 0 && vertsources.Count > 0) BeamState = BeamType.Cross;
            else if (vertsources.Count > 0) BeamState = BeamType.Vertical;
            else if (horzsources.Count > 0) BeamState = BeamType.Horizontal;
            else
                parenttile.RemoveObject();
            // Pass on
            Tile target = parenttile.getAdjacentTile(dir);
            Common.PulseTile(target, charge, Common.ReverseDir(dir), source);
        }

        internal override ObjectType getType()
        {
            return ObjectType.LightBeam;
        }

        internal LightBeam(TextureID[] tid, Tile parent, BeamType type) : base(tid, parent)
        {
            BeamState = type;
        }

    }
    // state should be set to this and textures linked
    enum BeamType { Horizontal = 0, Vertical = 1, Cross = 2 }
}
