using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Parse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inlumino_SHARED
{
    internal class LevelSaveOnlineUI : IState
    {
        protected UIButton savebtn, backbtn;
        protected UITextField nametext;
        protected UIMenu menu;

        internal LevelSaveOnlineUI()
        {
            menu = new UIMenu();
            nametext = new UITextField(10, Color.White, Color.Black, "Enter Name Here");
            nametext.AllowedCharTypes = CharType.Lower | CharType.Upper;
            savebtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.SaveButton], savepressed);
            backbtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.BackButton], backpressed);

            //menu.Add(nametext);
            menu.Add(savebtn);
            menu.Add(backbtn);
        }

        private void backpressed(UIButton sender)
        {
            Manager.StateManager.SwitchBack();
        }

        private async void savepressed(UIButton sender)
        {
            savebtn.Visible = false;

            if (ParseUser.CurrentUser == null)
                await AlertHandler.ShowMessage("Hello", "Please connect your facebook account to continue.", new string[] { "Ok" });
            else
            {
                LevelData data = DataHandler.GetLevelData(levelname);
                string hash = SecurityProvider.GetMD5Hash(data.Data);
                if (Common.IsMainLevel(hash))
                {
                    await AlertHandler.ShowMessage("Oh oh!", "You can't share a level that already exists in an official package.", new string[] { "OK" });
                    return;
                }
                Texture2D thumb = DataHandler.GetLevelThumb(levelname);
                string name = nametext.Text;

                ParseObject obj = default(ParseObject);
                try
                {
                    var query = ParseObject.GetQuery("LevelData").WhereEqualTo("hash", hash);
                    IEnumerable<ParseObject> found = await query.FindAsync();
                    obj = found.FirstOrDefault();
                }
                catch { }

                if (obj != null && !obj.ACL.PublicWriteAccess && !obj.ACL.GetWriteAccess(ParseUser.CurrentUser))
                {
                    int? r = await AlertHandler.ShowMessage("Error", "The same level (Design) already exists in the database and you don't have access to it. Would you like to go to it now?", new string[] { "Ok", "No, thanks" });
                    if (r == 0)
                        Manager.Play(hash, PackageType.Online);
                    return;
                }
                bool created = false;
                if (obj == default(ParseObject))
                {
                    obj = new ParseObject("LevelData");
                    obj["hash"] = hash;
                    obj["creator"] = ParseUser.CurrentUser.ObjectId;
                    obj["played"] = 0;
                    obj["thumb"] = "";
                    created = true;
                }
                obj["name"] = name;
                obj["namelc"] = name.ToLower();
                obj["data"] = data.Data;
                obj.ACL = new ParseACL(ParseUser.CurrentUser)
                {
                    PublicReadAccess = true,
                    PublicWriteAccess = false
                };
                try
                {
                    bool savethumb = true;
                    ParseConfig config = null;
                    try
                    {
                        config = await ParseConfig.GetAsync();
                    }
                    catch (Exception e)
                    {
                        config = ParseConfig.CurrentConfig;
                    }
                    config.TryGetValue("uploadthumb", out savethumb);
                    if (savethumb)
                    {
                        await obj.SaveAsync();
                        var thumbytes = Common.Texture2DToBytes(thumb);
                        var filename = Base32Converter.ToBase32String(Encoding.UTF8.GetBytes(obj.ObjectId)) + ".jpg";
                        ParseFile file = new ParseFile(filename, thumbytes);
                        await file.SaveAsync();
                        obj["thumb"] = file.Url.ToString();
                    }
                    await obj.SaveAsync();
                    if (created)
                    {
                        string levels = "";
                        ParseUser.CurrentUser["levels"] = ParseUser.CurrentUser.TryGetValue<string>("levels", out levels);
                        ParseUser.CurrentUser["levels"] = (levels + "," + obj.ObjectId).Trim(',');
                        try
                        {
                            await ParseUser.CurrentUser.SaveAsync();
                        }
                        catch { }
                        int? r = await AlertHandler.ShowMessage("Saved", "Level saved with ID: \n$" + obj.ObjectId, new string[] { "Ok", "Share!" });
                        if (r == 1)
                            Common.ShareLevel(obj);
                    }
                    else
                    {
                        int? r = await AlertHandler.ShowMessage("Saved", "Level " + name + " updated!", new string[] { "Ok", "Share!" });
                        if (r == 1)
                            Common.ShareLevel(obj);
                    }
                }
                catch (Exception ex) { await AlertHandler.ShowMessage("Error", "Failed to save level.\n" + ex.Message, new string[] { "Ok" }); }
                backpressed(backbtn);
            }
        }

        private void SetupMenu()
        {
            nametext.Size = new Vector2(Screen.Width, Screen.Height * 0.2f);
            menu.setAllSizeRelative(0.2f, Orientation.Landscape);
            menu.ArrangeInForm(Orientation.Landscape);
            nametext.Position = new Vector2((Screen.Width - nametext.Width) / 2, 0);
            menu.Position = new Vector2((Screen.Width - menu.Width) / 2, nametext.BoundingBox.Bottom);
        }

        public void Update(GameTime time)
        {
            nametext.Update(time);
            menu.Update(time);
        }

        public void Draw(SpriteBatch batch)
        {
            menu.Draw(batch);
            nametext.Draw(batch);
        }

        public void HandleEvent(WorldEvent e, bool forcehandle = false)
        {
            if (e is DisplaySizeChangedEvent || e is OrientationChangedEvent)
                SetupMenu();
            menu.HandleEvent(e);
            nametext.HandleEvent(e);
        }
        string levelname = "";
        public void OnActivated(params object[] args)
        {
            if (args.Length == 0) Manager.StateManager.SwitchTo(GameState.MainMenu);
            levelname = args[0] as string;
            nametext.Text = levelname;
            savebtn.Visible = true;
            SetupMenu();
        }
    }
}