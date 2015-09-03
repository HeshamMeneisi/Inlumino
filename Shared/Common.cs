using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Inlumino_SHARED
{
    // Common gampelay code affecting multiple classes is kept here
    static class Common
    {
        public static ObjectType[] EditorObjects = new ObjectType[] { ObjectType.None, ObjectType.Delete, ObjectType.LightSource, ObjectType.Crystal, ObjectType.Prism, ObjectType.Block, ObjectType.Splitter };
        public static string[] MainLevelNames = new string[] {"FirstHand" }; // For the selector
        public static void PowerUpTile(Tile target, Direction dir, ILightSource source)
        {
            if (target == default(Tile)) return;
            if (target.hasObject())
            {
                StaticObject obj = target.getObject();
                if (obj is IObstructingObject) (obj as IObstructingObject).HandleOn(source, dir);
                else if (obj is LightBeam)
                {
                    LightBeam beam = obj as LightBeam;
                    if ((int)dir % 2 == 0) // Vertical
                    {
                        if (beam.BeamState == BeamType.Horizontal)
                        {
                            beam.BeamState = BeamType.Cross;
                            beam.VerticalDirection = ReverseDir(dir);
                            beam.Activate();
                        }
                    }
                    else // Horizontal
                    {
                        if (beam.BeamState == BeamType.Vertical)
                        {
                            beam.BeamState = BeamType.Cross;
                            beam.HorzDirection = ReverseDir(dir);
                            beam.Activate();
                        }
                    }
                }
            }
            else
            {
                LightBeam beam = new LightBeam(DataHandler.ObjectTextureMap[ObjectType.LightBeam], target, (int)dir % 2 == 0 ? BeamType.Vertical : BeamType.Horizontal, ReverseDir(dir), ReverseDir(dir));
                target.SetObject(beam);
                beam.Activate();
            }
        }
        public static void PowerOffTile(Tile target, Direction dir, ILightSource source)
        {
            if (target == default(Tile)) return;
            if (target.hasObject())
            {
                StaticObject obj = target.getObject();
                if (obj is IObstructingObject) (obj as IObstructingObject).HandleOff(source, dir);
                else if (obj is LightBeam)
                {
                    if ((int)dir % 2 == 0) (obj as LightBeam).DeleteVertical();
                    else (obj as LightBeam).DeleteHorizontal();
                }
            }
            else
                target.SetObject(new LightBeam(DataHandler.ObjectTextureMap[ObjectType.LightBeam], target, (int)dir % 2 == 0 ? BeamType.Vertical : BeamType.Horizontal, dir, dir));
        }

        internal static Stage CreateLevel(string name, bool ismain)
        {
            Stage level = DataHandler.LoadStage(name, ismain);

            if (level != null)
                level.setBackground(DataHandler.getTexture(0));

            return level;
        }

        public static Direction ReverseDir(Direction dir)
        { return (Direction)((int)(dir + 2) % 4); }
        public static Direction NextDirCW(Direction dir, int count = 1)
        { return (Direction)((int)(dir + count) % 4); }
        public static Direction NextDirCCW(Direction dir, int count = 1)
        { return (Direction)((int)(dir + 3 * count) % 4); }
    }
}
