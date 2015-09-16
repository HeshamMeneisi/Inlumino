using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Xml.Serialization;

namespace Inlumino_SHARED
{
    class DataHandler
    {
        internal const int TextureUnitDim = 256;
        // Files
        // The index in this array represents the groupindex used in TextureID
        private static Dictionary<string, string> TextureFiles;

        private static string[] FontFiles;

        private static Dictionary<SoundType, string> SoundFiles;

        internal static void LoadCurrentTheme()
        {
            UnloadAll();
            string d = GetCurrentThemeDirectory();
            TextureFiles = new Dictionary<string, string>
            {
            { "bg","Textures\\"+d+"\\background" },
            {"mmb","Textures\\"+d+"\\mmbackground"},
            {"ui","Textures\\"+d+"\\ui"},
            {"obj","Textures\\"+d+"\\objects"},
            {"aux","Textures\\"+d+"\\auxiliary"}
             };
            LoadTextures();
            SoundFiles = new Dictionary<SoundType, string>
            {
            {SoundType.TapSound, "Sounds\\"+d+"\\TapSound" },
            { SoundType.RotateSound, "Sounds\\"+d+"\\RotateSound" },
            { SoundType.CrystalLit, "Sounds\\"+d+"\\CrystalLit" },
            {SoundType.AllCrystalsLit, "Sounds\\"+d+"\\AllCrystalsLit" },
            {SoundType.Background, "Sounds\\"+d+"\\Background" }
            };
            LoadSounds();
            FontFiles = new string[] { "Fonts\\" + d + "\\MainFont" };
            LoadFonts();
        }

        private static void UnloadAll()
        {
            Manager.ContentManager.Unload();
            Textures.Clear();
            Sounds.Clear();
            Fonts.Clear();
        }

        private static string GetCurrentThemeDirectory()
        {
            return Manager.GameSettings.CurrentTheme.ToString();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////

        private static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();

        internal static Texture2D GetPackageThumb(PackageType pack)
        {
            try {
                return Manager.ContentManager.Load<Texture2D>("Textures\\"+pack.ToString()+"\\Thumb");
            }
            catch { return null; }
        }

        internal static List<SpriteFont> Fonts = new List<SpriteFont>();

        internal static Dictionary<SoundType, SoundEffect> Sounds = new Dictionary<SoundType, SoundEffect>();
        /// <summary>
        /// We should define new tiles here.
        /// </summary>
        #region Tile Definition        
        internal static Dictionary<TileType, TextureID[]> TileTextureMap = new Dictionary<TileType, TextureID[]>()
        {
            {TileType.Default,new TextureID[] {new TextureID("obj",0) /*Normal*/, new TextureID("obj",1) /*Highlight overlay*/, new TextureID("obj",15) /*Object Board*/} },
        };
        #endregion

        /// <summary>
        /// We should define new objects here.
        /// </summary>
        #region Object Definition
        internal static Dictionary<ObjectType, TextureID[]> ObjectTextureMap = new Dictionary<ObjectType, TextureID[]>()
        {            
            // Virtual
            {ObjectType.None, new TextureID[] { new TextureID("ui",2)} },
            {ObjectType.Delete, new TextureID[] {new TextureID("ui",3)} },
            {ObjectType.Invisible, new TextureID[] {new TextureID("ui",0)} },
            // Real
            {ObjectType.LightSource, new TextureID[] { new TextureID("obj",12),new TextureID("obj",13)} },
            {ObjectType.Block, new TextureID[] { new TextureID("obj",4)} },
            {ObjectType.Prism, new TextureID[] { new TextureID("obj",8), new TextureID("obj",9)} },
            {ObjectType.LightBeam, new TextureID[] { new TextureID("obj",7),new TextureID("obj",6)} },
            {ObjectType.Crystal, new TextureID[] { new TextureID("obj", 3), new TextureID("obj", 2) } },
            {ObjectType.Splitter, new TextureID[] {new TextureID("obj",11),new TextureID("obj",10)} },
            {ObjectType.Portal, new TextureID[] {new TextureID("obj",14),new TextureID("obj",5)} }
        };
        #endregion
        /// <summary>
        /// We should define ui objects here.
        /// </summary>
        #region UI Items
        internal static Dictionary<UIObjectType, TextureID[]> UIObjectsTextureMap = new Dictionary<UIObjectType, TextureID[]>()
        {
            {UIObjectType.PlayBtn, new TextureID[] {new TextureID("ui",17, 1, 0.5f) } },
            {UIObjectType.EditModeBtn, new TextureID[] {new TextureID("ui",4, 1, 0.5f) } },
            {UIObjectType.OptionsBtn,new TextureID[] {new TextureID("ui",8, 1, 0.5f) } },
            {UIObjectType.Cell, new TextureID[] {new TextureID("ui",1)} },
            {UIObjectType.MenuButton,new TextureID[] {new TextureID("ui",12, 1, 0.5f) } },
            {UIObjectType.RestartButton,new TextureID[] {new TextureID("ui",13, 1, 0.5f) } },
            {UIObjectType.ToggleButton,new TextureID[] {new TextureID("ui",14, 1, 0.5f) } },
            {UIObjectType.SaveButton,new TextureID[] {new TextureID("ui",15, 1, 0.5f)} },
            {UIObjectType.Next,new TextureID[] {new TextureID("ui",18, 2, 1)} },
            {UIObjectType.TryAgain,new TextureID[] {new TextureID("ui",22, 2, 1)} },
            {UIObjectType.BackButton,new TextureID[] {new TextureID ("ui",11, 1, 0.5f) } },
            {UIObjectType.MainUser,new TextureID[] {new TextureID("ui",7, 1, 0.5f)} },
            {UIObjectType.DeleteBtn,new TextureID[] {new TextureID("ui",16, 1, 0.5f)} },
            {UIObjectType.LeftButton,new TextureID[] {new TextureID("ui",5, 0.5f,0.5f)} },
            {UIObjectType.RightButton,new TextureID[] {new TextureID("ui", 6, 0.5f, 0.5f) } },
            {UIObjectType.UpButton,new TextureID[] {new TextureID("ui", 9, 0.5f, 0.5f) } },
            {UIObjectType.DownButton,new TextureID[] {new TextureID("ui", 10, 0.5f, 0.5f) } },
            {UIObjectType.Star,new TextureID[] {new TextureID("ui", 20, 1, 1), new TextureID("ui", 21, 1, 1) } },
            {UIObjectType.Log,new TextureID[] {new TextureID("ui",26,2,1)} },
            {UIObjectType.Lock,new TextureID[] {new TextureID("ui",38,2,2)} },
            {UIObjectType.Border,new TextureID[] {new TextureID("ui",24,2,1)} },
            {UIObjectType.TopLog,new TextureID[] {new TextureID("ui",30,2,1),new TextureID("ui",28,2,1)} },
            {UIObjectType.Ropes,new TextureID[] {new TextureID("ui",34,2,1)} },
            {UIObjectType.Frame,new TextureID[] {new TextureID("ui",36,2,2)} }
        };
        #endregion
        static IsolatedStorageFile savegameStorage = IsolatedStorageFile.GetUserStoreForApplication();
        internal static IEnumerable<string> getSavedLevelNames()
        {
            foreach (string s in savegameStorage.GetFileNames())
            {
                if (s.StartsWith("S_")) yield return s.Split('.')[0].Split('_')[1];
            }
        }        
        static string getDataFileName(string stagename)
        {
            return "S_" + stagename + ".xml";
        }
        static string getThumbFileName(string stagename)
        {
            return "T_" + stagename + ".png";
        }
        internal static void SaveStage(Stage currentLevel, string name)
        {
            LevelData data = new LevelData(currentLevel.getTileMap().getIntMap(), currentLevel.getObjectMap(), currentLevel.getObjectRotationMap());
            SaveData<string>(data.Data, getDataFileName(name));
            Stream s = savegameStorage.CreateFile(getThumbFileName(name));
            /////// Thumb
            Screen.MakeVirtual(new Vector2(512, 512));
            currentLevel.ShuffleLevel();
            currentLevel.SetMinScreenPadding(new Padding(0, 0, 0, 0));
            Texture2D img = Manager.Parent.TakeScreenshot(currentLevel);
            Screen.MakeReal();
            //img.SaveAsPng(s, img.Width, img.Width);
            img.SaveAsJpeg(s, img.Width, img.Height);
#if ANDROID
            saveExternal(s, "temp/Inlumino/" + getThumbFileName(name));
#endif           
            s.Dispose();
        }
        internal static void DeleteStage(string name)
        {
            string data = getDataFileName(name);
            string thumb = getThumbFileName(name);
            if (savegameStorage.FileExists(data)) savegameStorage.DeleteFile(data);
            if (savegameStorage.FileExists(thumb)) savegameStorage.DeleteFile(thumb);
        }
        internal static bool LevelExists(string name)
        {
            return savegameStorage.FileExists(getDataFileName(name));
        }
        internal static Stage LoadStage(string name, PackageType package = PackageType.User)
        {
            LevelData temp;
            string data = "";
            if (package != PackageType.User)
            {
                data = Manager.ContentManager.Load<string>("MainLevels\\" + package.ToString() + "\\MLS_" + name);
            }
            else
                data = LoadData<string>(getDataFileName(name));
            temp = LevelData.CreateNew(data);
            if (temp == null)
                return null;
            return new Stage(temp);
        }
        internal static Texture2D GetLevelThumb(string name,PackageType package = PackageType.User)
        {
            if (package != PackageType.User)
            {
                try {
                    return Manager.ContentManager.Load<Texture2D>("MainLevels\\" + package.ToString() + "\\T_" + name);
                }
                catch { return null; }
            }
            else
            {
                try
                {
                    if (!savegameStorage.FileExists(getThumbFileName(name))) return null;
                    Stream s = savegameStorage.OpenFile(getThumbFileName(name), FileMode.Open);
                    Texture2D ret = Texture2D.FromStream(Manager.Parent.GraphicsDevice, s);
                    s.Dispose();
                    return ret;
                }
                catch { return null; }
            }
        }
        internal static void LoadTextures()
        {
            foreach (string t in TextureFiles.Keys)
                Textures.Add(t, Manager.ContentManager.Load<Texture2D>(TextureFiles[t]));
        }
        internal static void LoadFonts()
        {
            foreach (string f in FontFiles)
                Fonts.Add(Manager.ContentManager.Load<SpriteFont>(f));
        }
        internal static void LoadSounds()
        {
            foreach (KeyValuePair<SoundType, string> p in SoundFiles)
                Sounds.Add(p.Key, Manager.ContentManager.Load<SoundEffect>(p.Value));
        }

        internal static Rectangle getTextureSource(TextureID id)
        {
            // Textures are expected to be square
            int unitsperrow = Textures[id.RefKey].Width / TextureUnitDim;
            int texw = (int)(TextureUnitDim * id.WidthUnits);
            int texh = (int)(TextureUnitDim * id.HeightUnits);
            return new Rectangle(id.Index % unitsperrow * TextureUnitDim + 1, id.Index / unitsperrow * TextureUnitDim + 1, texw - 1, texh - 1);
        }

        internal static Texture2D getTexture(string key)
        {
            return Textures[key];
        }
        internal static Texture2D getTexture(TextureID tid)
        {
            return Textures[tid.RefKey];
        }
        internal static void SaveData<T>(T data, string file)
        {
            IsolatedStorageFileStream str = savegameStorage.CreateFile(file);
            string path = "Unkown";
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(str, data);
#if ANDROID
            path = "temp/Inlumino/" + file;
            saveExternal(str, path);
#endif
#if WINDOWS_UAP
            path = str.GetType().GetField("m_FullPath", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(str).ToString();            
#endif
            Debug.WriteLine("______ALERT______SavedFileTo: " + path);

            str.Dispose();
        }

        private static void saveExternal(Stream str, string file)
        {
#if ANDROID
            str.Seek(0, SeekOrigin.Begin);
            string pathToFile = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, file);
            if (!Directory.Exists(Path.GetDirectoryName(pathToFile))) Directory.CreateDirectory(Path.GetDirectoryName(pathToFile));
            using (var fileStream = new FileStream(pathToFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                int read;
                byte[] buffer = new byte[1024];
                while ((read = str.Read(buffer, 0, buffer.Length)) > 0)
                    fileStream.Write(buffer, 0, read);
            }
#endif
        }

        /// <summary>
        /// Returns default(T) on failure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <param name="debugmode"></param>
        /// <returns></returns>
        internal static T LoadData<T>(string file)
        {
            if (!savegameStorage.FileExists(file)) return default(T);

            using (Stream str = savegameStorage.OpenFile(file, FileMode.Open))
            {

                XmlSerializer serializer = new XmlSerializer(typeof(T));

                try
                {
                    return (T)serializer.Deserialize(str);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                    return default(T);
                }
            }
        }
        /// <summary>
        /// Gets the group index using the file name.
        /// </summary>
        /// <param name="name">File name without extension. (e.g "ObjectTex")</param>
        /// <returns></returns>
        internal static string LoadTexture(string name, Texture2D texture = null)
        {
            if (Textures.ContainsKey(name)) return name;
            if (texture != null)
            { Textures.Add(name, texture); return name; }
            try
            {
                Texture2D temp = Manager.ContentManager.Load<Texture2D>(name);
                if (temp != null)
                    Textures.Add(name, temp);
                return name;
            }
            catch { return ""; }
        }
        internal static bool isValid(TextureID tid)
        {
            return tid != default(TextureID) && Textures.ContainsKey(tid.RefKey);
        }
    }
    internal enum SoundType { TapSound = 0, RotateSound = 1, CrystalLit = 2, AllCrystalsLit = 3, Background = 4 }
}
public enum ThemeType { Beach = 0, Space = 1 }