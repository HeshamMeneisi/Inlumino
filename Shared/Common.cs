using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using Parse;
#if ANDROID
using Android.OS;
using Java.Util;
using Xamarin.Facebook;
#else
using Facebook;
#endif

namespace Inlumino_SHARED
{
    // Common gampelay code affecting multiple classes is kept here
    static class Common
    {
        internal static ObjectType[] EditorObjects = new ObjectType[] { ObjectType.None, ObjectType.Delete, ObjectType.LightSource, ObjectType.Crystal, ObjectType.Prism, ObjectType.Block, ObjectType.Splitter, ObjectType.FourWay, ObjectType.Portal };
        internal static string[] UserPackage = new string[] { };
        internal static string[] OnlinePackage = new string[] { };
        internal static string[] BeachPackage = new string[] { "$BA", "$BB", "$BC", "$BD", "$BE", "$BF", "$BG", "$BH", "$BI", "$BJ", "$BK", "$BL", "$BM", "$BN", "$BO", "$BP", "$BQ", "$BR", "$BS", "$BT", "$BU", "$BV", "$BW", "$BX", "$BY" };
        internal static string[] SpacePackage = new string[] { "$SZA", "$SZB", "$SZC", "$SZD", "$SZE", "$SZF", "$SZG", "$SZH", "$SZI", "$SZJ", "$SZK", "$SZL", "$SZM", "$SZN", "$SZO", "$SZP", "$SZQ", "$SZR", "$SZS", "$SZT", "$SZU", "$SZV", "$SZW", "$SZX", "$SZY" };

        public delegate void HandleFBLogin();
        public static event HandleFBLogin HandleFB;
        public delegate void PostLinkToFacebook(PostEventArgs e);
        public static event PostLinkToFacebook HandlePostLinkFB;
        internal static async Task UnlockWithFacebook(PackageType pack)
        {
            if (ParseUser.CurrentUser == null || !ParseFacebookUtils.IsLinked(ParseUser.CurrentUser))
            {
                await AlertHandler.ShowMessage("Ops", "It seems like you are not logged in or your session has expired. Please login or relogin.", new string[] { "Ok" });
                return;
            }
            string name = "";
            ParseUser.CurrentUser.TryGetValue<string>("name", out name);
            PostEventArgs e = new PostEventArgs(new PostInfo(
            "Inlumino",
            name + " just unlocked the " + pack.ToString() + " package in Inlumino!\nPlay Inlumino now on Android or Windows, it's free!",
            await GetPromotionalLink()));
            e.PostInfo.Tag = pack;
            HandlePostLinkFB(e);
        }

        private static async Task<string> GetPromotionalLink()
        {
            ParseConfig config = null;
            try
            {
                config = await ParseConfig.GetAsync();
            }
            catch (Exception e)
            {
                config = ParseConfig.CurrentConfig;
            }
            string link = "";
#if ANDROID
            config.TryGetValue("ANDROID_promotionlink", out link);
#endif
            return link;
        }

        internal static Dictionary<PackageType, string[]> Packages = new Dictionary<PackageType, string[]>
        {
            {PackageType.Beach,BeachPackage },
            {PackageType.Space,SpacePackage },
            {PackageType.User,UserPackage }
#if !DISABLEONLINE
            ,
            {PackageType.Online,OnlinePackage }
#endif
        };
        internal static void HandleFacebookPressed()
        {
            HandleFB();
        }

        internal static bool IsPackageLocked(PackageType pack)
        {
            return !(pack == PackageType.User || pack == PackageType.Online || (Manager.UserData.PackageAvailability.ContainsKey(pack) && Manager.UserData.PackageAvailability[pack]));
        }

        internal static Dictionary<PackageType, string[]> H = new Dictionary<PackageType, string[]>
        {
            {PackageType.Beach,CommonData.BPH },
            {PackageType.Space,CommonData.SPH }
        };
        internal static TextureID[] Auxiliaries = GetAux().ToArray();

        public static string CurrentSystem
        {
            get
            {
#if ANDROID
                return "ANDROID";
#elif WINDOWS_UAP
                return "UAP";
#elif WP81
                return "WP81";
#else
                return "Unknown";
#endif
            }
        }

        internal static async Task NotifyPostFinished(PostInfo postInfo)
        {
            if (postInfo.Posted)
            {
                if (postInfo.Tag is PackageType)
                {
                    PackageType name = (PackageType)postInfo.Tag;
                    Manager.UserData.MakeAvailable(name);
                    Manager.SaveUserDataLocal();
                    await AlertHandler.ShowMessage("Congrats", "You have unlocked the " + name.ToString() + " package!", new string[] { "Ok" });
                    Manager.StateManager.SwitchTo(GameState.MainMenu);
                }
                else if (postInfo.Tag is ParseObject)
                {
                    await AlertHandler.ShowMessage("Shared!", "Your level has been shared!", new string[] { "Ok" });
                }
            }
            else
                await AlertHandler.ShowMessage("Ops", "Something went wrong, please try again later.", new string[] { "Ok" });
        }

        private static IEnumerable<TextureID> GetAux() { for (int i = 0; i < 8; i++) yield return new TextureID(PrimaryTexture._Aux.ToString(), i); }

        internal static bool IsMainLevel(string hash)
        {
            foreach (string[] v in Common.H.Values)
                foreach (string h in v)
                    if (h.Equals(hash)) return true;
            return false;
        }

        internal static async Task ShareLevel(ParseObject obj)
        {
            if (ParseUser.CurrentUser == null || !ParseFacebookUtils.IsLinked(ParseUser.CurrentUser))
            {
                await AlertHandler.ShowMessage("Ops", "It seems like you are not logged in or your session has expired. Please login or relogin.", new string[] { "Ok" });
                return;
            }
            string name = "";
            ParseUser.CurrentUser.TryGetValue<string>("name", out name);
            PostEventArgs e = new PostEventArgs(new PostInfo(
            "Inlumino",
            name + " just created a new level named \"" + obj.Get<string>("name") + "\"\nPlay it directly by searching this code: $" + obj.ObjectId,
            await GetPromotionalLink()));
            e.PostInfo.Tag = obj;
            HandlePostLinkFB(e);
        }

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
#if ANDROID
        internal static DateTime JavaDateToDateTime(Date d)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(d.Time);
        }
#endif
        internal static void FBLoggedIn(string id, string token, DateTime expires)
        {
            ParseFacebookUtils.LogInAsync(id, token, expires).ConfigureAwait(false).GetAwaiter().GetResult();
            try
            {
                string name = "", email = "";
#if ANDROID
                var callback = new GraphCallback();

                callback.RequestCompleted += (GraphResponse r) =>
                {
                    var j = r.JSONObject;
                    name = j.Get("name").ToString();
                    email = j.Get("email").ToString();
                };
                var parm = new Bundle();
                parm.PutString("fields", "name,email");
                var request = new GraphRequest(AccessToken.CurrentAccessToken, "/" + id, parm, HttpMethod.Get, callback);
                var t = new Task(() => request.ExecuteAndWait());
                t.Start();
                t.ConfigureAwait(false).GetAwaiter().GetResult();
#elif WP81 || WINDOWS_UAP
                FacebookClient fc = new FacebookClient(token);
                var obj = fc.GetTaskAsync<IDictionary<string, object>>("me?fields=name,email").ConfigureAwait(false).GetAwaiter().GetResult();
                object n, e;
                if (obj.TryGetValue("name", out n))
                    name = n.ToString();
                if (obj.TryGetValue("email", out e))
                    email = e.ToString();
#endif
                ParseUser.CurrentUser["username"] = id;
                if (email != "")
                    ParseUser.CurrentUser["email"] = email;
                if (name != "")
                    ParseUser.CurrentUser["name"] = name;
                if (ParseUser.CurrentUser.IsNew)
                    ParseUser.CurrentUser["signupsys"] = CurrentSystem;
                ParseUser.CurrentUser.SaveAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to update user info from facebook: " + ex.Message);
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

        internal static Stage CreateLevel(string name, PackageType package = PackageType.User)
        {
            Stage level = DataHandler.LoadStage(name, package);

            if (level != null)
                level.setBackground(DataHandler.getTexture(PrimaryTexture._BG));

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

        internal static byte[] Texture2DToBytes(Texture2D thumb)
        {
            MemoryStream s = new MemoryStream();
            thumb.SaveAsJpeg(s, thumb.Width, thumb.Height);
            byte[] data = new byte[s.Length];
            s.Position = 0;
            s.Read(data, 0, data.Length);
            s.Dispose();
            return data;
        }
        internal static Texture2D Texture2DFromBytes(byte[] data)
        {
            MemoryStream s = new MemoryStream();
            s.Write(data, 0, data.Length);
            s.Position = 0;
            Texture2D ret = Texture2D.FromStream(Manager.Parent.GraphicsDevice, s);
            s.Dispose();
            return ret;
        }
        internal static Direction NextDirCCW(Direction dir, int count = 1)
        { return count >= 0 ? (Direction)((int)(dir + 3 * count) % 4) : NextDirCW(dir, -count); }
        internal static Direction RelativeDir(Direction dir, Direction neworigin, Direction origin = Direction.North)
        { return NextDirCW(dir, origin - neworigin); }

        internal static bool isDirVertical(Direction dir)
        { return dir == Direction.North || dir == Direction.South; }

        internal static TextureID GetStarsTex(int s)
        {
            return new TextureID(() =>
            {
                TextureID[] star = DataHandler.UIObjectsTextureMap[UIObjectType.Star];
                return Manager.Parent.Concat(star[s > 0 ? 1 : 0], star[s > 1 ? 1 : 0], star[s > 2 ? 1 : 0]);
            }, "_" + s + "stars", 0, 3, 1);
        }

        internal static void MatchTheme(PackageType package)
        {
            bool flag;
            switch (package)
            {
                case PackageType.Beach:
                    Manager.GameSettings.CurrentTheme = ThemeType.Beach;
                    DataHandler.LoadCurrentTheme(); break;
                case PackageType.Space:
                    Manager.GameSettings.CurrentTheme = ThemeType.Space;
                    DataHandler.LoadCurrentTheme(); break;
                default: return;
            }
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
            if (package == PackageType.Beach && IsPackageLocked(PackageType.Space))
            {
                Manager.UserData.MakeAvailable(PackageType.Space);
                Manager.SaveUserDataLocal();
                AlertHandler.ShowMessage("Congratulations!", "You have unlocked the Space package! If you like the game consider rating us on the store :)", new string[] { "OK" });
            }
            else
            {
                AlertHandler.ShowMessage("Congratulations!", "You finisehd the space package! Check for updates soon for more!\nIf you liked the game consider rating us on the store :)", new string[] { "OK" });
            }

            Manager.StateManager.SwitchTo(GameState.MainMenu);
#if !DISABLEONLINE
            ParseAnalytics.TrackEventAsync("PackFinished", new Dictionary<string, string> { { "name", package.ToString() } });
#endif
        }

        internal static void SpreadAuxiliaries(Stage currentLevel, float v)
        {
            if (Auxiliaries.Length == 0) return;
            var ran = new System.Random();
            foreach (Tile t in currentLevel.getTileMap().AllTiles)
            {
                if (ran.NextDouble() < v)
                    t.SetAuxiliary(Auxiliaries[ran.Next(Auxiliaries.Length)]);
            }
        }
        internal static int GetScore(PackageType pack, string name)
        {
            if (pack == PackageType.User) return 0;
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

        internal static async Task SignalPlayed(ParseObject obj)
        {
            try
            {
                int count = obj.Get<int>("played");
                obj["played"] = count + 1;
                obj.SaveAsync();
            }
            catch { }
        }

        internal static bool VerifyLevel(string name, PackageType pack, string data)
        {
            int i = 0;
            string[] MainLevelNames = Packages[pack];
            for (; i < MainLevelNames.Length; i++)
                if (MainLevelNames[i] == name)
                {
                    string h = SecurityProvider.GetMD5Hash(data);
                    return h.Equals(Common.H[pack][i]);
                }
            return false;
        }
    }
    public enum PackageType
    {
        None = -2, User = 0, Beach = 1, Space = 2,
        Online = 3
    }
#if ANDROID
    class GraphCallback : Java.Lang.Object, GraphRequest.ICallback
    {
        public delegate void RequestCompletedEventHandler(GraphResponse r);
        // Event to pass the response when it's completed
        public event RequestCompletedEventHandler RequestCompleted;

        public void OnCompleted(GraphResponse response)
        {
            RequestCompleted(response);
        }
    }
#endif

    public class PostEventArgs
    {
        public bool Handled = false;

        public PostInfo PostInfo { get; set; }

        public PostEventArgs(PostInfo info)
        { PostInfo = info; }
    }

    static class CommonData
    {
        internal static string[] SPH = new string[] { "8PvAJQUx6J33knnHVaSecA==", "GK+t2sBr4zNwtCUgffFXCw==", "x8tdDAc3r5uCdzp18/EMrQ==", "9QZditpqmYdWC5ChozR5uQ==", "tiifEN0A+FZATZTLdzvRsA==", "0455fMlYN5hUkKZsr1etLQ==", "0ujO099GHYURU2Hgll6i/A==", "ndGBHDg7tu16LuG/FTQqVw==", "2eUQpjOQTCT//PN3qJJPmg==", "36K5WA+iX8gByEJzouQ/jw==", "rSDd5miYZKuhpZV1Kw1Oqw==", "kbb1nQ/loM1ENPzw0QZWXQ==", "Sm0IDwyVkStm0Mj9+IUqkA==", "8xqQRMlwfUi0QVCG7MV+1Q==", "YJvu3yJaJWrTBAgMtx4AmA==", "9oGHYqXnEVCRrKouIu4bVg==", "rJuTocMMrRUOfPPyn5asAA==", "MlWIf6XJNNvODAgqHjvSiQ==", "WR3dIKFly9xCEMk84k5rxQ==", "7Uf8no0Fsk/ElZeane4wgg==", "vOC1zuCcQ0TmahtB45vX0g==", "SSijqUSUSjVkCjzZu1rhKw==", "BaCX9XH+G4HXc5WFX+wGNw==", "xPLOWRZ3YCbIfN8rQDYjBw==", "XYzHNSVulvO1lCYgHIVVaQ==" };
        internal static string[] BPH = new string[] { "svFMgqgbsNqpGmptucZvew==", "h0vHbNd044njuTV3PCMBrw==", "64FNLHlarDpooGG3PC6NGQ==", "BTA+PIdwsxdqQXxcWirXOQ==", "Pzkb2fBXj1zXcZWaVyf8vw==", "s6arXO5YJ8QEa+fVFjc33A==", "9/osIu3FoERT4+CQFlCjIg==", "wOr16mVtYHGJ0tBcL+y/qw==", "DAHVHCPlaCOXcBPQpSKUQw==", "34Tc5uL85VXCGrxm+il7Vg==", "/jFLCmDBUfWFFH6Lx11sMQ==", "OlYdAkhOPH7Lq4GOQkJ58Q==", "j4ChfnCyvAXdgUkpEO+xKg==", "VxPUKRby+dyB8v26Em3yRA==", "j4ChfnCyvAXdgUkpEO+xKg==", "HvsoXD5/oo+/UlcjdkBUBg==", "PFpSoDfo3BvG2b7ERREijQ==", "hg73AWXT3pVaLCxMKNI1Rw==", "yIU7DX60kP66VAl4lMfBkw==", "RxuHT/3wz4UHuWGFnd92/g==", "PW+XsrsHChhozYFarhaegw==", "myUaVuZXtRGXZ/eNTecSIA==", "G5mD1gQCs2kcLzRYIFM5jg==", "WbMkRKeSRezg789uB0mGnA==", "Veyyb+Jmle6bMn7Q0OhIZA==" };

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
