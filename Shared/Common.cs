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
        public static ObjectType[] EditorObjects = new ObjectType[] { ObjectType.None, ObjectType.Delete, ObjectType.LightSource, ObjectType.Crystal, ObjectType.Prism, ObjectType.Block, ObjectType.Splitter, ObjectType.Portal };
        public static string[] MainLevelNames = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t" }; // For the selector
        public static int[] moves = new int[] { 3, 4, 8, 20, 12, 32, 9, 9, 27, 2, 13, 17, 11, 11, 17, 13, 17, 11, 21, 58 };
        public static TextureID[] Auxiliaries = new TextureID[] { new TextureID(4, 0), new TextureID(4, 1), new TextureID(4, 2), new TextureID(4, 3), new TextureID(4, 4) };
        public static void PulseTile(Tile target, bool charge, Direction side, ILightSource source)
        {
            if (target == default(Tile)) return;
            if (target.hasObject<IObstructingObject>()) (target.getObject() as IObstructingObject).HandlePulse(charge, side, source);
            else
            {
                if (!target.hasObject()) target.SetObject(new LightBeam(DataHandler.ObjectTextureMap[ObjectType.LightBeam], target, side == Direction.East || side == Direction.West ? BeamType.Horizontal : BeamType.Vertical));
                (target.getObject() as LightBeam).CarryPulse(charge, Common.ReverseDir(side), source);
            }
        }

        internal static bool isSameAngle(double a, double b, double tolerance = 1e-9)
        {
            a = NormalizeAngle(a);
            b = NormalizeAngle(b);
            if (Math.Abs(a - b) <= tolerance) return true;
            return false;
        }

        private static double NormalizeAngle(double a)
        {
            while (a >= Math.PI * 2)
                a -= Math.PI * 2;
            while (a < 0)
                a += Math.PI * 2;
            return a;
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
        { return count >= 0 ? (Direction)((int)(dir + count) % 4) : NextDirCCW(dir, -count); }
        public static Direction NextDirCCW(Direction dir, int count = 1)
        { return count >= 0 ? (Direction)((int)(dir + 3 * count) % 4) : NextDirCW(dir, -count); }
        public static Direction RelativeDir(Direction dir, Direction neworigin, Direction origin = Direction.North)
        { return NextDirCW(dir, origin - neworigin); }

        public static bool isDirVertical(Direction dir)
        { return dir == Direction.North || dir == Direction.South; }
        public static bool isDirHorizontal(Direction dir)
        { return dir == Direction.East || dir == Direction.West; }

        internal static void NextLevel(string cln)
        {
            int i = 0;
            for (; i < MainLevelNames.Length; i++)
                if (MainLevelNames[i] == cln) break;
            i++;
            if (i < MainLevelNames.Length)
                Manager.Play(MainLevelNames[i], true);
            else
                GameFinished();
        }
        public static int GetMoves(string cln)
        {
            for (int i = 0; i < moves.Length; i++)
                if (MainLevelNames[i] == cln) return moves[i];
            return -1;
            throw new Exception("Levele is not recognized.");
        }
        private static void GameFinished()
        {
            Manager.StateManager.SwitchTo(GameState.MainMenu);
        }

        internal static void SpreadAuxiliaries(Stage currentLevel, float v)
        {
            if (Auxiliaries.Length == 0) return;
            Random ran = new Random();
            foreach (Tile t in currentLevel.getTileMap().AllTiles)
            {
                if (ran.NextDouble() < v)
                    t.SetAuxiliary(Auxiliaries[ran.Next(Auxiliaries.Length - 1)]);
            }
        }

        internal static void SetScore(string cln, int score)
        {
            int i = 0;
            for (; i < MainLevelNames.Length; i++)
                if (MainLevelNames[i] == cln)
                    Manager.GameSettings.stars[i] = Math.Max(Manager.GameSettings.stars[i], score);
            Manager.SaveSettings();
        }
    }
}
