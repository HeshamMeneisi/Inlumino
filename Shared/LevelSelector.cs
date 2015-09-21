using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Parse;
using System.Text.RegularExpressions;

namespace Inlumino_SHARED
{
    class LevelSelector : IState
    {
        protected UIHud mainlevels;
        protected UIMenu genmenu;
        protected UIButton menubtn;
        protected UIButton backbtn;
        protected UIButton sharebtn;
        protected UIButton deletebtn;
        protected UIButton nextbtn;
        protected UITextField searchquery;
        UITextField loading;
        protected List<UICell> mlcells = new List<UICell>();
        internal LevelSelector()
        {
            menubtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.MenuButton], menupressed);
            backbtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.BackButton], backpressed);
            deletebtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.DeleteBtn], delpressed);
            sharebtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.ShareBtn], sharepressed);
            nextbtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.Next], nextpressed);
            loading = new UITextField(10, Color.White, Color.Black, "Loading...");
            searchquery = new UITextField(10, Color.Black, Color.White, "Search...");
            searchquery.AllowedCharTypes = CharType.Lower | CharType.Upper;
            loading.Text = "Loading...";
            searchquery.SelectedChanged += searchselect;
            genmenu = new UIMenu();
            genmenu.Add(menubtn); genmenu.Add(backbtn); genmenu.Add(nextbtn); genmenu.Add(deletebtn); genmenu.Add(searchquery);
        }

        private void nextpressed(UIButton sender)
        {
            if (busy) return;
            onlinelevelpos++;
            SetupHud();
        }

        private void searchselect(UITextField sender, bool state)
        {
            if (!state)
            {
                onlinelevelpos = 0;
                SetupHud();
            }
        }

        private async void delpressed(UIButton sender)
        {
            if (mainlevels.SnapTarget != null)
            {
                string name = mainlevels.SnapTarget.Tag.ToString();
                int? r = await MessageBox.Show("Deleting", "Are you sure you want to delete this level? \n" + name + " will be deleted from local storage.", new string[] { "No", "Yes" });
                if (r == 1)
                {
                    DataHandler.DeleteStage(name);
                    SetupHud();
                }
            }
        }

        protected virtual void backpressed(UIButton sender)
        {
            Manager.StateManager.SwitchTo(GameState.PackageSelector);
        }

        protected virtual void menupressed(UIButton sender)
        {
            Manager.StateManager.SwitchTo(GameState.MainMenu);
        }

        protected virtual void mlcellpressed(UIButton sender)
        {
            string name = (string)(sender as UICell).Tag;
            if (name == "$$L$$")
            {
                MessageBox.Show("Locked", "Solve more levels to unlcok this level!", new string[] { "Ok" });
                return;
            }
            Manager.Play(name, package);
        }

        public void Draw(SpriteBatch batch)
        {
            Texture2D background = DataHandler.getTexture(PrimaryTexture._MMBG);
            float w = Screen.Height * background.Width / background.Height;
            batch.Draw(background, new Rectangle((int)(Screen.Width - w) / 2, 0, (int)(w), (int)Screen.Height), Color.White);
            genmenu.Draw(batch);
            loading.Draw(batch);
            if (mainlevels == null) return;
            mainlevels.Draw(batch);
            sharebtn.Draw(batch);
        }

        public void HandleEvent(WorldEvent e, bool forcehandle = false)
        {
            genmenu.HandleEvent(e);
            sharebtn.HandleEvent(e);
            if (mainlevels == null) return;
            mainlevels.HandleEvent(e);
            if (e is OrientationChangedEvent || e is DisplaySizeChangedEvent)
                SetupHud(false);
            else
            {
                if (InputManager.isKeyDown(Keys.Right))
                    mainlevels.SlideRight();
                if (InputManager.isKeyDown(Keys.Left))
                    mainlevels.SlideLeft();
            }
        }

        public void Update(GameTime time)
        {
            genmenu.Update(time);
            sharebtn.Update(time);
            if (mainlevels == null) return;
            mainlevels.Update(time);
        }

        PackageType package;
        int onlinelevelpos = 0;
        int levelsperscreen = 25;
        bool busy = false;
        protected virtual async Task SetupHud(bool reload = true)
        {
            busy = true;
            genmenu.setAllSizeRelative(0.16f * (Screen.Mode == Orientation.Portrait ? 2 : 1), Screen.Mode);
            searchquery.ScaleToDefault();
            genmenu.ArrangeInForm(Common.ReverseOrientation(Screen.Mode));
            UICell target = mainlevels == null ? null : mainlevels.SnapTarget;
            if (reload)
            {
                mlcells.Clear();
                bool first = true;
                if (package == PackageType.Online)
                {
                reqeury:
                    var query =
                        (searchquery.Text == "" ? ParseObject.GetQuery("LevelData")
                        : (from level in ParseObject.GetQuery("LevelData") where level.Get<string>("namelc").Contains(searchquery.Text) select level))
                        .OrderByDescending("played").Skip(onlinelevelpos * levelsperscreen).Limit(levelsperscreen);

                    var objects = await query.FindAsync();
                    if (objects.Count() == 0 && onlinelevelpos > 0)
                    { onlinelevelpos--; goto reqeury; }
                    var e = objects.GetEnumerator();
                    while (e.MoveNext())
                    {
                        try
                        {
                            UICell cell = new UICell(DataHandler.UIObjectsTextureMap[UIObjectType.Frame], e.Current.ObjectId, e.Current.Get<string>("name"), Color.White, new TextureID(() => DataHandler.GetLevelThumb(e.Current), e.Current.ObjectId, 0, -1, -1), 0.1f);
                            cell.Pressed += mlcellpressed;
                            mlcells.Add(cell);
                        }
                        catch { }
                    }

                }
                else if (package == PackageType.User)
                {
                    foreach (string name in DataHandler.getSavedLevelNames())
                    {
                        UICell cell = new UICell(DataHandler.UIObjectsTextureMap[UIObjectType.Frame], name, name, Color.White, new TextureID(() => DataHandler.GetLevelThumb(name, package), name, 0, -1, -1), 0.1f);
                        cell.Pressed += mlcellpressed;
                        mlcells.Add(cell);
                    }
                    if (mlcells.Count > 0)
                    {
                        target = mlcells[0];
                        sharebtn.Visible = true;
                        sharebtn.Size = target.Size * 0.25f;
                    }
                    else { sharebtn.Visible = false; }
                }
                else
                    foreach (string name in Common.Packages[package])
                    {
                        int s = Common.GetScore(package, name);
                        TextureID stex = null;
                        bool flag = false;
                        bool scrollflage = false;
                        if (s > 0 || first)
                        {
                            if (s == 0)
                            { first = false; scrollflage = true; }
                            stex = Common.GetStarsTex(s);
                            flag = true;
                        }
                        else stex = DataHandler.UIObjectsTextureMap[UIObjectType.Lock][0];
                        UICell cell = new UICell(DataHandler.UIObjectsTextureMap[UIObjectType.Frame], flag ? name : "$$L$$", "", Color.White, new TextureID(() => DataHandler.GetLevelThumb(name, package), name, 0, -1, -1), 0.1f);
                        if (scrollflage) target = cell;
                        cell.AttachSibling(new UIVisibleObject(new TextureID[] { stex }));
                        cell.Pressed += mlcellpressed;
                        mlcells.Add(cell);
                    }
            }
            float tp = Screen.Mode == Orientation.Portrait ? genmenu.Height : 0;
            if (package != PackageType.Online)
            {
                float d = Math.Min(Screen.SmallDim, Screen.BigDim * 0.6f);
                mainlevels = new UIHud(mlcells.ToArray(), Orientation.Portrait, d, d, d, d);
                mainlevels.SnapCameraToCells = true;
                mainlevels.Position = new Vector2(Screen.Mode == Orientation.Landscape ? genmenu.Width + (Screen.Width - genmenu.Width - d) / 2 : (Screen.Width - d) / 2, (Screen.Height - tp - d) / 2 + tp);
            }
            else
            {
                float lt = Screen.Mode == Orientation.Landscape ? genmenu.Width : 0;
                mainlevels = new UIHud(mlcells.ToArray(), Orientation.Landscape, Screen.SmallDim * 0.2f, Screen.SmallDim * 0.2f, Screen.Width - lt, Screen.Height - tp);
                mainlevels.Position = new Vector2(lt, tp);
            }
            mainlevels.Setup();
            mainlevels.FitCellSiblings();
            if (sharebtn.Visible && target != null)
                sharebtn.Position = target.GlobalPosition + target.Size * 0.1f;
            mainlevels.SnapTarget = target;
            mainlevels.Visible = true;
            loading.Visible = false;
            busy = false;
        }

        private void sharepressed(UIButton sender)
        {
            if (mainlevels.SnapTarget != null)
                Manager.HandleShareReq(mainlevels.SnapTarget.Tag.ToString());
        }
        public void OnActivated(params object[] args)
        {
            onlinelevelpos = 0;
            package = (PackageType)args[0];
            Common.MatchTheme(package);
            mainlevels = null;
            deletebtn.Visible = sharebtn.Visible = package == PackageType.User;
            nextbtn.Visible = package == PackageType.Online;
            if (package == PackageType.Online)
            {
                loading.Visible = searchquery.Visible = true; loading.Position = new Vector2((Screen.Width - loading.Width) / 2, (Screen.Height - loading.Height) / 2);
            }
            else loading.Visible = searchquery.Visible = false;
            SetupHud();
        }
    }
}