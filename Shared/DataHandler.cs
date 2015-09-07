using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;

namespace Inlumino_SHARED
{
    class DataHandler
    {
        public const int TextureUnitDim = 256;

        // The index in this array represents the groupindex used in TextureID
        private static List<string> TextureFiles = new List<string> { "Textures\\background"/*group=0*/, "Textures\\ui", "Textures\\objects"/*,add more files here*/};

        private static string[] FontFiles = new string[] { "Fonts\\MainFont" };

        private static Dictionary<SoundType, string> SoundFiles = new Dictionary<SoundType, string>
        {
            {SoundType.TapSound, "Sounds\\TapSound" },
            { SoundType.RotateSound, "Sounds\\RotateSound" },
            { SoundType.CrystalLit, "Sounds\\CrystalLit" },
            {SoundType.AllCrystalsLit, "Sounds\\AllCrystalsLit" },
            {SoundType.Background, "Sounds\\Background" }
        };

        private static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();

        public static List<SpriteFont> Fonts = new List<SpriteFont>();

        public static Dictionary<SoundType, SoundEffect> Sounds = new Dictionary<SoundType, SoundEffect>();

        /// <summary>
        /// We should define new tiles here.
        /// </summary>
        #region Tile Definition        
        public static Dictionary<TileType, TextureID[]> TileTextureMap = new Dictionary<TileType, TextureID[]>()
        {
            {TileType.Default,new TextureID[] {new TextureID(2,0) /*Normal*/, new TextureID(2,1) /*Glowing*/ } },
        };
        #endregion

        /// <summary>
        /// We should define new objects here.
        /// </summary>
        #region Object Definition
        public static Dictionary<ObjectType, TextureID[]> ObjectTextureMap = new Dictionary<ObjectType, TextureID[]>()
        {            
            // Virtual
            {ObjectType.None, new TextureID[] { new TextureID(1,2)} },
            {ObjectType.Delete, new TextureID[] {new TextureID(1,3)} },
            {ObjectType.Invisible, new TextureID[] {new TextureID(1,0)} },
            // Real
            {ObjectType.LightSource, new TextureID[] { new TextureID(2,12),new TextureID(2,13)} },
            {ObjectType.Block, new TextureID[] { new TextureID(2,4)} },
            {ObjectType.Prism, new TextureID[] { new TextureID(2,8), new TextureID(2,9)} },
            {ObjectType.LightBeam, new TextureID[] { new TextureID(2,7),new TextureID(2,6)} },
            {ObjectType.Crystal, new TextureID[] { new TextureID(2, 3), new TextureID(2, 2) } },
            {ObjectType.Splitter, new TextureID[] {new TextureID(2,11),new TextureID(2,10)} },
            {ObjectType.Portal, new TextureID[] {new TextureID(2,14),new TextureID(2,5)} }
        };

        internal static IEnumerable<string> getSavedLevelNames()
        {
            foreach (string s in savegameStorage.GetFileNames())
            {
                if (s.StartsWith("S_")) yield return s.Split('.')[0].Split('_')[1];
            }
        }
        #endregion
        /// <summary>
        /// We should define ui objects here.
        /// </summary>
        #region UI Items
        public static Dictionary<UIObjectType, TextureID[]> UIObjectsTextureMap = new Dictionary<UIObjectType, TextureID[]>()
        {
            {UIObjectType.PlayBtn, new TextureID[] {new TextureID(1,17, 1, 0.5f) } },
            {UIObjectType.EditModeBtn, new TextureID[] {new TextureID(1,4, 1, 0.5f) } },
            {UIObjectType.OptionsBtn,new TextureID[] {new TextureID(1,8, 1, 0.5f) } },
            {UIObjectType.Cell, new TextureID[] {new TextureID(1,1)} },
            {UIObjectType.MenuButton,new TextureID[] {new TextureID(1,12, 1, 0.5f) } },
            {UIObjectType.RestartButton,new TextureID[] {new TextureID(1,13, 1, 0.5f) } },
            {UIObjectType.ToggleButton,new TextureID[] {new TextureID(1,14, 1, 0.5f) } },
            {UIObjectType.SaveButton,new TextureID[] {new TextureID(1,15, 1, 0.5f)} },
            {UIObjectType.BackButton,new TextureID[] {new TextureID (1,11, 1, 0.5f) } },
            {UIObjectType.MainUser,new TextureID[] {new TextureID(1,7, 1, 0.5f)} },
            {UIObjectType.DeleteBtn,new TextureID[] {new TextureID(1,16, 1, 0.5f)} },
            {UIObjectType.LeftButton,new TextureID[] {new TextureID(1,5, 0.5f,0.5f)} },
            {UIObjectType.RightButton,new TextureID[] {new TextureID(1, 6, 0.5f, 0.5f) } },
            {UIObjectType.UpButton,new TextureID[] {new TextureID(1, 9, 0.5f, 0.5f) } },
            {UIObjectType.DownButton,new TextureID[] {new TextureID(1, 10, 0.5f, 0.5f) } }
        };
        #endregion
        static IsolatedStorageFile savegameStorage = IsolatedStorageFile.GetUserStoreForApplication();

        static string getDataFileName(string stagename)
        {
            return "S_" + stagename + ".xml";
        }
        static string getThumbFileName(string stagename)
        {
            return "T_" + stagename + ".png";
        }
        internal static void SaveStage(Stage currentLevel, string name, Texture2D img)
        {
            LevelData data = new LevelData(currentLevel.getTileMap().getIntMap(), currentLevel.getObjectMap(), currentLevel.getObjectRotationMap());
            SaveData<string>(data.Data, getDataFileName(name));
            Stream s = savegameStorage.CreateFile(getThumbFileName(name));
            img.SaveAsPng(s, img.Width, img.Width);
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
        internal static Stage LoadStage(string name, bool mainlevel)
        {
            LevelData temp;
            string data = "";
            if (mainlevel)
            {
                data = Manager.ContentManager.Load<string>("MainLevels\\MLS_" + name);
            }
            else
                data = LoadData<string>(getDataFileName(name));
            temp = LevelData.CreateNew(data);
            if (temp == null)
                return null;
            return new Stage(temp);
        }
        public static Texture2D GetLevelThumb(string name, bool mainlevel)
        {
            if (mainlevel)
            {
                return Manager.ContentManager.Load<Texture2D>("MainLevels\\T_" + name);
            }
            else
            {
                if (!savegameStorage.FileExists(getThumbFileName(name))) return null;
                Stream s = savegameStorage.OpenFile(getThumbFileName(name), FileMode.Open);
                Texture2D ret = Texture2D.FromStream(Manager.Parent.GraphicsDevice, s);
                s.Dispose();
                return ret;
            }
        }
        public static void LoadTextures()
        {
            foreach (string t in TextureFiles)
                Textures.Add(t, Manager.ContentManager.Load<Texture2D>(t));
        }
        public static void LoadFonts()
        {
            foreach (string f in FontFiles)
                Fonts.Add(Manager.ContentManager.Load<SpriteFont>(f));
        }
        public static void LoadSounds()
        {
            foreach (KeyValuePair<SoundType, string> p in SoundFiles)
                Sounds.Add(p.Key, Manager.ContentManager.Load<SoundEffect>(p.Value));
        }

        public static Rectangle getTextureSource(TextureID id)
        {
            // Textures are expected to be square
            int unitsperrow = Textures[TextureFiles[id.GroupIndex]].Width / TextureUnitDim;
            int texw = (int)(TextureUnitDim * id.WidthUnits);
            int texh = (int)(TextureUnitDim * id.HeightUnits);
            return new Rectangle(id.Index % unitsperrow * TextureUnitDim + 1, id.Index / unitsperrow * TextureUnitDim + 1, texw - 1, texh - 1);
        }

        internal static Texture2D getTexture(int p)
        {
            return Textures[TextureFiles[p]];
        }

        public static void SaveData<T>(T data, string file)
        {
            IsolatedStorageFileStream str = savegameStorage.CreateFile(file);
            string path = "Unkown";
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(str, data);
#if ANDROID
            path = "temp/Inlumino/" + file;
            saveExternal(str, path);
#endif
#if WINDOWS
            string path = str.GetType().GetField("m_FullPath", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(str).ToString();            
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
        public static T LoadData<T>(string file)
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
        public static int getGroupIndexFromName(string name, Texture2D texture = null)
        {
            int r = TextureFiles.IndexOf(name);
            if (r >= 0) return r;
            if (texture != null)
            {
                TextureFiles.Add(name);
                Textures.Add(name, texture);
                return Textures.Count - 1;
            }
            try
            {
                Texture2D temp = Manager.ContentManager.Load<Texture2D>(name);
                TextureFiles.Add(name);
                Textures.Add(name, temp);
                return Textures.Count - 1;
            }
            catch
            {
                return -1;
            }
        }
        internal static bool isValid(TextureID tid)
        {
            return tid != default(TextureID) && tid.GroupIndex >= 0 && tid.GroupIndex < TextureFiles.Count;
        }
    }
    public enum SoundType { TapSound = 0, RotateSound = 1, CrystalLit = 2, AllCrystalsLit = 3, Background = 4 }
}
