using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
#if WP81

#else
using System.IO.IsolatedStorage;
#endif
using System.Reflection;
using System.Xml.Serialization;
using Parse;
using System.Net;
#if WINDOWS_UAP || WP81
using System.Net.Http;
using Windows.Storage;
#endif

namespace Inlumino_SHARED
{
    class DataHandler
    {
        static SmartContentManager UIContent = null;

        internal const int TextureUnitDim = 256;
        // Files
        // The index in this array represents the groupindex used in TextureID
        private static Dictionary<PrimaryTexture, string> TextureFiles = new Dictionary<PrimaryTexture, string>();

        private static Dictionary<FontType, string> FontFiles = new Dictionary<FontType, string>();

        private static Dictionary<SoundType, string> SoundFiles = new Dictionary<SoundType, string>();

        static ThemeType loadedtheme = ThemeType.Beach;
        static bool first = true;
        internal static void LoadCurrentTheme()
        {
            if (first) first = false;
            else if (loadedtheme == Manager.GameSettings.CurrentTheme) return;

            if (UIContent != null)
            {
                UIContent.Unload();
                UIContent.Dispose();
                GC.Collect();
            }
            UIContent = new SmartContentManager(Manager.RandomAccessContentManager.ServiceProvider);
            UIContent.RootDirectory = Manager.RandomAccessContentManager.RootDirectory;
            string d = GetCurrentThemeDirectory();

            TextureFiles = new Dictionary<PrimaryTexture, string>();
            foreach (PrimaryTexture t in PrimaryTexture.GetValues(typeof(PrimaryTexture)))
                TextureFiles.Add(t, "Textures\\" + d + "\\" + t.ToString());
            LoadTextures();

            SoundFiles.Clear();
            foreach (SoundType s in SoundType.GetValues(typeof(SoundType)))
                SoundFiles.Add(s, "Sounds\\" + d + "\\" + s.ToString());
            LoadSounds();

            FontFiles.Clear();
            foreach (FontType f in FontType.GetValues(typeof(FontType)))
                FontFiles.Add(f, "Fonts\\" + d + "\\" + f.ToString());
            LoadFonts();
            loadedtheme = Manager.GameSettings.CurrentTheme;
            SoundManager.StopAllLoops();
            SoundManager.PlaySound(DataHandler.Sounds[SoundType.Background], SoundCategory.Music, true);
            Manager.SaveUserDataLocal();

            GC.Collect();
        }

        private static string GetCurrentThemeDirectory()
        {
            return Manager.GameSettings.CurrentTheme.ToString();
        }

        internal static Texture2D getTexture(PrimaryTexture key)
        {
            return getTexture(key.ToString());
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////

        internal static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();

        internal static Dictionary<FontType, SpriteFont> Fonts = new Dictionary<FontType, SpriteFont>();

        internal static Dictionary<SoundType, SoundEffect> Sounds = new Dictionary<SoundType, SoundEffect>();

        internal static Texture2D GetPackageThumb(PackageType pack)
        {
            try
            {
                return Manager.RandomAccessContentManager.Load<Texture2D>("Textures\\" + pack.ToString() + "\\Thumb");
            }
            catch { return null; }
        }
        internal static string UIKey { get { return PrimaryTexture._UI.ToString(); } }
        internal static string ObjKey { get { return PrimaryTexture._Obj.ToString(); } }
        /// <summary>
        /// We should define new tiles here.
        /// </summary>
#region Tile Definition        
        internal static Dictionary<TileType, TextureID[]> TileTextureMap = new Dictionary<TileType, TextureID[]>()
        {
            {TileType.Default,new TextureID[] {new TextureID(ObjKey,0) /*Normal*/, new TextureID(ObjKey,1) /*Highlight overlay*/, new TextureID(ObjKey,15) /*Object Board*/} },
        };
        #endregion

        /// <summary>
        /// We should define new objects here.
        /// </summary>
        #region Object Definition
        internal static Dictionary<ObjectType, TextureID[]> ObjectTextureMap = new Dictionary<ObjectType, TextureID[]>()
        {            
            // Virtual
            {ObjectType.None, new TextureID[] { new TextureID(UIKey,2)} },
            {ObjectType.Delete, new TextureID[] {new TextureID(UIKey,3)} },
            {ObjectType.Invisible, new TextureID[] {new TextureID(UIKey,0)} },
            // Real
            {ObjectType.LightSource, new TextureID[] { new TextureID(ObjKey,12),new TextureID(ObjKey,13)} },
            {ObjectType.Block, new TextureID[] { new TextureID(ObjKey,4)} },
            {ObjectType.Prism, new TextureID[] { new TextureID(ObjKey,8), new TextureID(ObjKey,9)} },
            {ObjectType.LightBeam, new TextureID[] { new TextureID(ObjKey,7),new TextureID(ObjKey,6)} },
            {ObjectType.Crystal, new TextureID[] { new TextureID(ObjKey, 3), new TextureID(ObjKey, 2) } },
            {ObjectType.Splitter, new TextureID[] {new TextureID(ObjKey,11),new TextureID(ObjKey,10)} },
            {ObjectType.Portal, new TextureID[] {new TextureID(ObjKey,14),new TextureID(ObjKey,5)} },
            {ObjectType.FourWay, new TextureID[] {new TextureID(ObjKey,16),new TextureID(ObjKey,17)} },
        };
        #endregion
        /// <summary>
        /// We should define ui objects here.
        /// </summary>
        #region UI Items
        internal static Dictionary<UIObjectType, TextureID[]> UIObjectsTextureMap = new Dictionary<UIObjectType, TextureID[]>()
        {
            {UIObjectType.PlayBtn, new TextureID[] {new TextureID(UIKey,17, 1, 0.5f) } },
            {UIObjectType.EditModeBtn, new TextureID[] {new TextureID(UIKey,4, 1, 0.5f) } },
            {UIObjectType.OptionsBtn,new TextureID[] {new TextureID(UIKey,8, 1, 0.5f) } },
            {UIObjectType.Cell, new TextureID[] {new TextureID(UIKey,1)} },
            {UIObjectType.MenuButton,new TextureID[] {new TextureID(UIKey,12, 1, 0.5f) } },
            {UIObjectType.ResetButton,new TextureID[] {new TextureID(UIKey,13, 1, 0.5f) } },
            {UIObjectType.ToggleButton,new TextureID[] {new TextureID(UIKey,14, 1, 0.5f) } },
            {UIObjectType.SaveButton,new TextureID[] {new TextureID(UIKey,15, 1, 0.5f)} },
            {UIObjectType.Next,new TextureID[] {new TextureID(UIKey,18, 2, 1)} },
            {UIObjectType.TryAgain,new TextureID[] {new TextureID(UIKey,22, 2, 1)} },
            {UIObjectType.BackButton,new TextureID[] {new TextureID (UIKey,11, 1, 0.5f) } },
            {UIObjectType.MainUser,new TextureID[] {new TextureID(UIKey,7, 1, 0.5f)} },
            {UIObjectType.DeleteBtn,new TextureID[] {new TextureID(UIKey,16, 1, 0.5f)} },
            {UIObjectType.LeftButton,new TextureID[] {new TextureID(UIKey,5, 0.5f,0.5f)} },
            {UIObjectType.RightButton,new TextureID[] {new TextureID(UIKey, 6, 0.5f, 0.5f) } },
            {UIObjectType.UpButton,new TextureID[] {new TextureID(UIKey, 9, 0.5f, 0.5f) } },
            {UIObjectType.DownButton,new TextureID[] {new TextureID(UIKey, 10, 0.5f, 0.5f) } },
            {UIObjectType.Star,new TextureID[] {new TextureID(UIKey, 20, 1, 1), new TextureID(UIKey, 21, 1, 1) } },
            {UIObjectType.Log,new TextureID[] {new TextureID(UIKey,26,2,1)} },
            {UIObjectType.Lock,new TextureID[] {new TextureID(UIKey,38,2,2)} },
            {UIObjectType.Border,new TextureID[] {new TextureID(UIKey,24,2,1)} },
            {UIObjectType.TopLog,new TextureID[] {new TextureID(UIKey,30,2,1),new TextureID(UIKey,28,2,1)} },
            {UIObjectType.ShareBtn,new TextureID[] {new TextureID(UIKey,32,1,1)} },
            {UIObjectType.Ropes,new TextureID[] {new TextureID(UIKey,34,2,1)} },            
            {UIObjectType.Frame,new TextureID[] {new TextureID(UIKey,36,2,2)} },
            {UIObjectType.FBBtn,new TextureID[] {new TextureID(UIKey,44,2,1)} }
        };


        #endregion

#if WP81
        static StorageFolder savegameStorage = ApplicationData.Current.LocalFolder;
#else
        static IsolatedStorageFile savegameStorage = IsolatedStorageFile.GetUserStoreForApplication();
#endif

        internal static IEnumerable<string> getSavedLevelNames()
        {
#if WP81
            foreach (StorageFile f in savegameStorage.GetFilesAsync().AsTask().ConfigureAwait(false).GetAwaiter().GetResult())
                if (f.Name.StartsWith("S_")) yield return f.Name.Split('.')[0].Split('_')[1];
#else
            foreach (string s in savegameStorage.GetFileNames())
                if (s.StartsWith("S_")) yield return s.Split('.')[0].Split('_')[1];            
#endif
        }
        static string getDataFileName(string stagename)
        {
            return "S_" + stagename + ".xml";
        }
        static string getThumbFileName(string stagename)
        {
            return "T_" + stagename + ".jpg";
        }
        internal static void SaveStage(Stage currentLevel, string name)
        {
            if (currentLevel == null) return;
            //name = "$" + name;//Main levels TODO:REMOVE
            LevelData data = new LevelData(currentLevel.getTileMap().getIntMap(), currentLevel.getObjectMap(), currentLevel.getObjectRotationMap());
            SaveData<string>(data.Data, getDataFileName(name));
#if WP81
            Stream s = savegameStorage.CreateFileAsync(getThumbFileName(name), CreationCollisionOption.ReplaceExisting).AsTask().ConfigureAwait(false).GetAwaiter().GetResult().OpenStreamForWriteAsync().Result;
#else
            Stream s = savegameStorage.CreateFile(getThumbFileName(name));
#endif
            /////// Thumb

            Texture2D img = GenerateLevelThumb(currentLevel);
            img.SaveAsJpeg(s, img.Width, img.Height);
#if ANDROID
            saveExternal(s, "temp/Inlumino/" + getThumbFileName(name));
#endif
            s.Dispose();

            ParseAnalytics.TrackEventAsync("UserSaveLevel", new Dictionary<string, string> { { "name", name } });
        }
        internal static void DeleteStage(string name)
        {
            string data = getDataFileName(name);
            string thumb = getThumbFileName(name);
#if WP81
            try { savegameStorage.GetFileAsync(data).AsTask().ConfigureAwait(false).GetAwaiter().GetResult().DeleteAsync().AsTask().ConfigureAwait(false).GetAwaiter().GetResult(); } catch { }
            try { savegameStorage.GetFileAsync(thumb).AsTask().ConfigureAwait(false).GetAwaiter().GetResult().DeleteAsync().AsTask().ConfigureAwait(false).GetAwaiter().GetResult(); } catch { }
#else
            if (savegameStorage.FileExists(data)) savegameStorage.DeleteFile(data);
            if (savegameStorage.FileExists(thumb)) savegameStorage.DeleteFile(thumb);
#endif
        }
        internal static bool LevelExists(string name)
        {
#if WP81
            try
            { StorageFile f = savegameStorage.GetFileAsync(getDataFileName(name)).AsTask().ConfigureAwait(false).GetAwaiter().GetResult(); return f != null; }
            catch { return false; }
#else
            return savegameStorage.FileExists(getDataFileName(name));
#endif
        }
        internal static Stage LoadStage(string name, PackageType package = PackageType.User)
        {
            LevelData temp = GetLevelData(name, package);
            if (temp == null)
                return null;
            return new Stage(temp);
        }
        internal static LevelData GetLevelData(string name, PackageType package = PackageType.User)
        {
            string data = "";
            if (package == PackageType.Online)
            {
                // name is the objectid
                try
                {
                    ParseObject obj = ParseObject.GetQuery("LevelData").GetAsync(name).Result;
                    data = obj.Get<string>("data");
                    Common.SignalPlayed(obj);
                }
                catch (Exception ex) { }
            }
            else if (package == PackageType.User)
                data = LoadData<string>(getDataFileName(name));
            else if (package != PackageType.None)
            {
                // Main Levels
                data = Manager.RandomAccessContentManager.Load<string>("MainLevels\\" + package.ToString() + "\\MLS_" + name);
                if (!Common.VerifyLevel(name, package, data))
                    return null;
            }

            return LevelData.CreateNew(data);
        }
        internal static Texture2D GetLevelThumb(string name, PackageType package = PackageType.User)
        {
            if (package != PackageType.User)
            {
                try
                {
                    return Manager.RandomAccessContentManager.Load<Texture2D>("MainLevels\\" + package.ToString() + "\\T_" + name);
                }
                catch { return GenerateLevelThumb(name, package); }
            }
            else
            {
                try
                {
#if WP81
                    Stream s;
                    try { s = savegameStorage.GetFileAsync(getThumbFileName(name)).AsTask().ConfigureAwait(false).GetAwaiter().GetResult().OpenStreamForReadAsync().Result; }
                    catch { return GenerateLevelThumb(name, package); }
#else
                    if (!savegameStorage.FileExists(getThumbFileName(name))) return GenerateLevelThumb(name, package);
                    Stream s = savegameStorage.OpenFile(getThumbFileName(name), FileMode.Open);
#endif
                    Texture2D ret = Texture2D.FromStream(Manager.Parent.GraphicsDevice, s);
                    s.Dispose();
                    return ret;
                }
                catch { return GenerateLevelThumb(name, package); }
            }
        }

        private static Texture2D GenerateLevelThumb(string name, PackageType package)
        {
            try
            {
                Stage temp = Common.CreateLevel(name, package);
                return GenerateLevelThumb(temp);
            }
            catch { return null; }
        }

        private static Texture2D GenerateLevelThumb(Stage level)
        {
            try
            {
                Debug.WriteLine("Generating thumbnail.");
                Screen.MakeVirtual(new Vector2(512, 512));
                level.ShuffleLevel();
                level.SetMinScreenPadding(new Padding(0, 0, 0, 0));
                Texture2D img = Manager.Parent.TakeScreenshot(level);
                Screen.MakeReal();
                return img;
            }
            catch { Screen.MakeReal(); return null; }
        }
        private static Texture2D GenerateLevelThumb(ParseObject obj)
        {
            try
            {
                Stage temp = Common.CreateLevel(obj.ObjectId, PackageType.Online);
                return GenerateLevelThumb(temp);
            }
            catch { return null; }
        }
        internal static Texture2D GetLevelThumb(ParseObject current)
        {
            try
            {
                string link = current.Get<string>("thumb");
#if WINDOWS_UAP || WP81
                HttpClient cl = new HttpClient();
                byte[] data = cl.GetByteArrayAsync(link).Result;
#else
                WebClient wc = new WebClient();
                byte[] data = wc.DownloadData(link);
#endif
                return Common.Texture2DFromBytes(data);
            }
            catch(Exception e) { Debug.WriteLine(e.Message); }
            return GenerateLevelThumb(current);
        }
        internal static void LoadTextures()
        {
            foreach (PrimaryTexture t in TextureFiles.Keys)
            {
                string key = t.ToString();
                if (!Textures.ContainsKey(key))
                    Textures.Add(key, UIContent.Load<Texture2D>(TextureFiles[t]));
                else
                {
                    Textures[key] = UIContent.Load<Texture2D>(TextureFiles[t]);
                }
                GC.Collect();
            }
        }
        internal static void LoadSounds()
        {
            foreach (SoundType p in SoundFiles.Keys)
                if (!Sounds.ContainsKey(p))
                    Sounds.Add(p, Manager.RandomAccessContentManager.Load<SoundEffect>(SoundFiles[p]));
                else
                {
                    Sounds[p] = Manager.RandomAccessContentManager.Load<SoundEffect>(SoundFiles[p]);
                }
        }
        internal static void LoadFonts()
        {
            foreach (FontType f in FontFiles.Keys)
                if (!Fonts.ContainsKey(f))
                    Fonts.Add(f, Manager.RandomAccessContentManager.Load<SpriteFont>(FontFiles[f]));
                else Fonts[f] = Manager.RandomAccessContentManager.Load<SpriteFont>(FontFiles[f]);
        }

        internal static Rectangle getTextureSource(TextureID id)
        {
            // Textures are expected to be square
            if (Textures[id.RefKey] == null) return new Rectangle(0, 0, 0, 0);
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
#if WP81    
            StorageFile f;
            int tries = 0;
            Stream str;
            retry:
            try
            {
                f = savegameStorage.CreateFileAsync(file, CreationCollisionOption.ReplaceExisting).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
                str = f.OpenStreamForWriteAsync().Result;
            }
            catch
            {
                if (tries > 10)
                {
                    Debug.WriteLine("__ALERT__: Failed to save file:" + file);
                    return;
                }
                tries++;
                goto retry;        
            }
#else
            IsolatedStorageFileStream str = savegameStorage.CreateFile(file);
#endif
            string path = "Unkown";
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(str, data);
#if ANDROID
            path = "temp/Inlumino/" + file;
            saveExternal(str, path);
#endif
#if WINDOWS_UAP && DEBUG // Causes crashes after release
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
            if (File.Exists(pathToFile)) File.Delete(pathToFile);
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
#if WP81
            StorageFile f = null;
            try { f = savegameStorage.GetFileAsync(file).AsTask().ConfigureAwait(false).GetAwaiter().GetResult(); } catch { return default(T); }
            if (f == null) return default(T);
            using (Stream str = f.OpenStreamForReadAsync().Result)
#else
            if (!savegameStorage.FileExists(file)) return default(T);

            using (Stream str = savegameStorage.OpenFile(file, FileMode.Open))
#endif
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
        internal static string LoadTexture(string name, Func<Texture2D> textureretriever = null)
        {
            if (Textures.ContainsKey(name)) return name;
            if (textureretriever != null)
            { Textures.Add(name, textureretriever()); return name; }
            try
            {
                Texture2D temp = Manager.RandomAccessContentManager.Load<Texture2D>(name);
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
public enum FontType { MainFont = 0 }

public enum PrimaryTexture { _UI = 0, _Obj = 1, _BG = 2, _MMBG = 3, _Aux = 4 }