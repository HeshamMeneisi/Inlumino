using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Inlumino_SHARED
{
    class LevelSelector : IState
    {
        protected UIHud mainlevels;
        protected UIMenu genmenu;
        protected UIButton menubtn;
        protected UIButton backbtn;
        protected List<UICell> mlcells = new List<UICell>();
        Texture2D background;
        internal LevelSelector()
        {
            background = DataHandler.getTexture("mmb");
            menubtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.MenuButton]);
            backbtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.BackButton]);
            menubtn.Pressed += menupressed;
            backbtn.Pressed += backpressed;
            genmenu = new UIMenu();
            genmenu.Add(menubtn); genmenu.Add(backbtn);
        }

        protected void backpressed(UIButton sender)
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
            float w = Screen.Height * background.Width / background.Height;
            batch.Draw(background, new Rectangle((int)(Screen.Width - w) / 2, 0, (int)(w), (int)Screen.Height), Color.White);
            genmenu.Draw(batch);
            mainlevels.Draw(batch);
        }

        public void HandleEvent(WorldEvent e, bool forcehandle = false)
        {
            genmenu.HandleEvent(e);
            mainlevels.HandleEvent(e);
            if (e is OrientationChangedEvent || e is DisplaySizeChangedEvent)
                SetupHud();
        }

        public void Update(GameTime time)
        {
            genmenu.Update(time);
            mainlevels.Update(time);
        }

        public void OnActivated(params object[] args)
        {
            package = (PackageType)args[0];
            SetupHud();
        }
        PackageType package;
        protected virtual void SetupHud()
        {
            genmenu.setAllSizeRelative(0.2f * (Screen.Mode == Orientation.Portrait ? 2 : 1), Screen.Mode);
            genmenu.ArrangeInForm(Common.ReverseOrientation(Screen.Mode));            
            mlcells.Clear();
            bool first = true;
            UICell target = null;
            if (package != PackageType.User)
                foreach (string name in Common.Packages[package])
                {
                    int s = Common.GetScore(package, name);
                    TextureID stex;
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
                    UICell cell = new UICell(DataHandler.UIObjectsTextureMap[UIObjectType.Frame], flag ? name : "$$L$$", "", Color.White, new TextureID(DataHandler.GetLevelThumb(name, package), name, 0, -1, -1), 0.1f);
                    if (scrollflage) target = cell;
                    cell.AttachSibling(new UIVisibleObject(new TextureID[] { stex }));
                    cell.FitSiblings();
                    cell.Pressed += mlcellpressed;
                    mlcells.Add(cell);
                }
            else
                foreach (string name in DataHandler.getSavedLevelNames())
                {
                    UICell cell = new UICell(DataHandler.UIObjectsTextureMap[UIObjectType.Frame], name, name, Color.White, new TextureID(DataHandler.GetLevelThumb(name, package), name, 0, -1, -1), 0.1f);
                    cell.FitSiblings();
                    cell.Pressed += mlcellpressed;
                    mlcells.Add(cell);
                }
            float d = Math.Min(Screen.SmallDim, Screen.BigDim * 0.6f);
            mainlevels = new UIHud(mlcells.ToArray(), Orientation.Portrait, d, d, d, d);            
            mainlevels.SnapCameraToCells = true;
            float tp = Screen.Mode == Orientation.Portrait ? genmenu.Height : 0;
            mainlevels.Position = new Vector2(Screen.Mode == Orientation.Landscape ? genmenu.Width + (Screen.Width - genmenu.Width - d) / 2 : (Screen.Width - d) / 2, (Screen.Height - tp - d) / 2 + tp);
            mainlevels.Setup();
            mainlevels.FitCellSiblings();
            mainlevels.SnapTarget = target;
            mainlevels.Visible = true;            
        }
    }
}
