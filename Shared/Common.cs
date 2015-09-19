using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Inlumino_SHARED
{
    // Common gampelay code affecting multiple classes is kept here
    static class Common
    {
        internal static ObjectType[] EditorObjects = new ObjectType[] { ObjectType.None, ObjectType.Delete, ObjectType.LightSource, ObjectType.Crystal, ObjectType.Prism, ObjectType.Block, ObjectType.Splitter, ObjectType.Portal };
        internal static string[] UserPackage = new string[] { };
        internal static string[] BeachPackage = new string[] { "$ba", "$bb", "$bc", "$bd", "$be", "$bf", "$bg", "$bh", "$bi", "$bj", "$bk", "$bl" };
        internal static string[] SpacePackge = new string[] { "$sa", "$sb", "$sc", "$sd", "$se", "$sf", "$sg", "$sh" };
        internal static Dictionary<PackageType, string[]> Packages = new Dictionary<PackageType, string[]>
        {
            {PackageType.Beach,BeachPackage },
            {PackageType.Space,SpacePackge },
            {PackageType.User,UserPackage }
        };
        internal static TextureID[] Auxiliaries = GetAux().ToArray();
        private static IEnumerable<TextureID> GetAux() { for (int i = 0; i < 5; i++) yield return new TextureID("aux", i); }
        internal static void PulseTile(Tile target, bool charge, Direction side, ILightSource source)
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

        internal static Stage CreateLevel(string name, PackageType package = Inlumino_SHARED.PackageType.User)
        {
            Stage level = DataHandler.LoadStage(name, package);

            if (level != null)
                level.setBackground(DataHandler.getTexture("bg"));

            return level;
        }

        internal static Direction ReverseDir(Direction dir)
        { return (Direction)((int)(dir + 2) % 4); }

        internal static Orientation ReverseOrientation(Orientation mode)
        {
            return mode == Orientation.Landscape ? Orientation.Portrait : Orientation.Landscape;
        }

        internal static Direction NextDirCW(Direction dir, int count = 1)
        { return count >= 0 ? (Direction)((int)(dir + count) % 4) : NextDirCCW(dir, -count); }
        internal static Direction NextDirCCW(Direction dir, int count = 1)
        { return count >= 0 ? (Direction)((int)(dir + 3 * count) % 4) : NextDirCW(dir, -count); }
        internal static Direction RelativeDir(Direction dir, Direction neworigin, Direction origin = Direction.North)
        { return NextDirCW(dir, origin - neworigin); }

        internal static bool isDirVertical(Direction dir)
        { return dir == Direction.North || dir == Direction.South; }

        internal static TextureID GetStarsTex(int s)
        {
            TextureID[] star = DataHandler.UIObjectsTextureMap[UIObjectType.Star];
            Texture2D t = Manager.Parent.Concat(star[s > 0 ? 1 : 0], star[s > 1 ? 1 : 0], star[s > 2 ? 1 : 0]);
            return new TextureID(DataHandler.LoadTexture(s + "stars", t), 0, 3, 1);
        }

        internal static bool isDirHorizontal(Direction dir)
        { return dir == Direction.East || dir == Direction.West; }

        internal static void NextLevel(string cln, PackageType package)
        {
            if (package == Inlumino_SHARED.PackageType.User) return;
            string[] MainLevelNames = Packages[package];
            int i = 0;
            for (; i < MainLevelNames.Length; i++)
                if (MainLevelNames[i] == cln) break;
            i++;
            if (i < MainLevelNames.Length)
                Manager.Play(MainLevelNames[i], package);
            else
                GameFinished(package);
        }
        private static void GameFinished(PackageType package)
        {
            if(package == PackageType.Beach)
            {
                Manager.UserData.MakeAvailable(PackageType.Space);
                Manager.SaveUserDataLocal();
                MessageBox.Show("Congratulations!", "You have unlocked the Space package!", new string[] { "OK" });
            }
            Manager.StateManager.SwitchTo(GameState.MainMenu);
        }

        internal static void SpreadAuxiliaries(Stage currentLevel, float v)
        {
            if (Auxiliaries.Length == 0) return;
            Random ran = new Random();
            foreach (Tile t in currentLevel.getTileMap().AllTiles)
            {
                if (ran.NextDouble() < v)
                    t.SetAuxiliary(Auxiliaries[ran.Next(Auxiliaries.Length)]);
            }
        }
        internal static int GetScore(PackageType pack, string name)
        {
            if (pack == Inlumino_SHARED.PackageType.User) return 0;
            int i = 0;
            for (; i < Packages[pack].Length; i++)
                if (Packages[pack][i] == name)
                    return Manager.UserData.getStars(pack, i);
            return 0;
        }
        internal static void SetScore(PackageType pack, string cln, int score)
        {
            int i = 0;
            string[] MainLevelNames = Packages[pack];
            for (; i < MainLevelNames.Length; i++)
                if (MainLevelNames[i] == cln)
                    Manager.UserData.setStars(pack, i, score);
            Manager.SaveUserDataLocal();
        }
    }
    public enum PackageType { None = -2, User = 0, Beach = 1, Space = 2 }

    static class CommonData
    {
        public static Dictionary<Keys, char[]> KeyCharMap = new Dictionary<Keys, char[]>()
        {
            { Keys.OemTilde,new char[] {'`','~' } },

            { Keys.OemMinus,new char[] {'-','_' } },

            { Keys.OemPlus,new char[] {'=','+' } },

            { Keys.Divide,new char[] {'/','\0' } },

            { Keys.Multiply,new char[] {'*','\0' } },

            { Keys.Subtract,new char[] {'-','\0' } },

            { Keys.Add,new char[] {'+','\0' } },

            { Keys.OemSemicolon,new char[] {';',':' } },

            { Keys.OemQuotes,new char[] {'\'','\"' } },

            { Keys.OemPipe,new char[] {'\\','|' } },

            { Keys.OemBackslash,new char[] {'\\','|' } },

            { Keys.OemComma,new char[] {',','<' } },

            { Keys.OemPeriod,new char[] {'.','>' } },

            { Keys.OemQuestion,new char[] {'/','?' } },

            { Keys.Space,new char[] {' ','\0' } },

            { Keys.Decimal,new char[] {'.','\0' } },

            { Keys.D0, new char[] { '0', ')' } },

            { Keys.D1, new char[] { '1', '!' } },

            { Keys.D2, new char[] { '2', '@' } },

            { Keys.D3, new char[] { '3', '#' } },

            { Keys.D4, new char[] { '4', '$' } },

            { Keys.D5, new char[] { '5', '%' } },

            { Keys.D6, new char[] { '6', '^' } },

            { Keys.D7, new char[] { '7', '&' } },

            { Keys.D8, new char[] { '8', '*' } },

            { Keys.D9, new char[] { '9', '(' } },

            { Keys.A, new char[] { 'a', 'A' } },

            { Keys.B, new char[] { 'b', 'B' } },

            { Keys.C, new char[] { 'c', 'C' } },

            { Keys.D, new char[] { 'd', 'D' } },

            { Keys.E, new char[] { 'e', 'E' } },

            { Keys.F, new char[] { 'f', 'F' } },

            { Keys.G, new char[] { 'g', 'G' } },

            { Keys.H, new char[] { 'h', 'H' } },

            { Keys.I, new char[] { 'i', 'I' } },

            { Keys.J, new char[] { 'j', 'J' } },

            { Keys.K, new char[] { 'k', 'K' } },

            { Keys.L, new char[] { 'l', 'L' } },

            { Keys.M, new char[] { 'm', 'M' } },

            { Keys.N, new char[] { 'n', 'N' } },

            { Keys.O, new char[] { 'o', 'O' } },

            { Keys.P, new char[] { 'p', 'P' } },

            { Keys.Q, new char[] { 'q', 'Q' } },

            { Keys.R, new char[] { 'r', 'R' } },

            { Keys.S, new char[] { 's', 'S' } },

            { Keys.T, new char[] { 't', 'T' } },

            { Keys.U, new char[] { 'u', 'U' } },

            { Keys.V, new char[] { 'v', 'V' } },

            { Keys.W, new char[] { 'w', 'W' } },

            { Keys.X, new char[] { 'x', 'X' } },

            { Keys.Y, new char[] { 'y', 'Y' } },

            { Keys.Z, new char[] { 'z', 'Z' } },

            { Keys.NumPad0, new char[] { '0', '\0' } },

            { Keys.NumPad1, new char[] { '1', '\0' } },

            { Keys.NumPad2, new char[] { '2', '\0' } },

            { Keys.NumPad3, new char[] { '3', '\0' } },

            { Keys.NumPad4, new char[] { '4', '\0' } },

            { Keys.NumPad5, new char[] { '5', '\0' } },

            { Keys.NumPad6, new char[] { '6', '\0' } },

            { Keys.NumPad7, new char[] { '7', '\0' } },

            { Keys.NumPad8, new char[] { '8', '\0' } },

            { Keys.NumPad9, new char[] { '9', '\0' } }
        };
    }
}
