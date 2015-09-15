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
        protected UIHud userlevels;
        protected UIMenu genmenu;
        protected UIButton menubtn;
        protected UIButton switchbtn;
        protected List<UICell> mlcells = new List<UICell>();

        internal LevelSelector()
        {
            menubtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.MenuButton]);
            switchbtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.MainUser]);
            menubtn.Pressed += menupressed;
            switchbtn.Pressed += switchpressed;
            genmenu = new UIMenu();
            genmenu.Add(menubtn); genmenu.Add(switchbtn);
        }

        protected void switchpressed(UIButton sender)
        {
            mainlevels.Visible = !mainlevels.Visible;
            userlevels.Visible = !userlevels.Visible;
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
            Manager.Play(name, true);
        }

        public void Draw(SpriteBatch batch)
        {
            genmenu.Draw(batch);
            mainlevels.Draw(batch);
            userlevels.Draw(batch);
        }

        public void HandleEvent(WorldEvent e, bool forcehandle = false)
        {
            genmenu.HandleEvent(e);
            mainlevels.HandleEvent(e);
            userlevels.HandleEvent(e);
            if (e is OrientationChangedEvent || e is DisplaySizeChangedEvent)
                SetupHud();
        }

        public void Update(GameTime time)
        {
            genmenu.Update(time);
            mainlevels.Update(time);
            userlevels.Update(time);
        }

        public void OnActivated(params object[] args)
        {
            SetupHud();
        }

        protected virtual void SetupHud()
        {
            genmenu.setAllSizeRelative(0.2f * (Screen.Mode == Orientation.Portrait ? 2 : 1), Screen.Mode);
            genmenu.ArrangeInForm(Common.ReverseOrientation(Screen.Mode));
            List<UICell> ulcells = new List<UICell>();
            mlcells.Clear();
            bool first = true;
            UICell target = null;
            foreach (string name in Common.MainLevelNames)
            {
                int s = Common.GetScore(name);
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
                UICell cell = new UICell(DataHandler.UIObjectsTextureMap[UIObjectType.Frame], flag ? name : "$$L$$", "", Color.White, new TextureID(DataHandler.GetLevelThumb(name, true), name, 0, -1, -1), 0.1f);
                if (scrollflage) target = cell;
                cell.AttachSibling(new UIVisibleObject(new TextureID[] { stex }));
                cell.FitSiblings();
                cell.Pressed += mlcellpressed;
                mlcells.Add(cell);
            }
            foreach (string name in DataHandler.getSavedLevelNames())
            {
                UICell cell = new UICell(DataHandler.UIObjectsTextureMap[UIObjectType.Frame], name, name, Color.White, new TextureID(DataHandler.GetLevelThumb(name, false), name, 0, -1, -1), 0.1f);
                cell.FitSiblings();
                cell.Pressed += ulcellpressed;
                ulcells.Add(cell);
            }
            float d = Math.Min(Screen.SmallDim, Screen.BigDim * 0.6f);
            mainlevels = new UIHud(mlcells.ToArray(), Orientation.Portrait, d, d, d, d);
            userlevels = new UIHud(ulcells.ToArray(), Orientation.Portrait, d, d, d, d);
            mainlevels.SnapCameraToCells = userlevels.SnapCameraToCells = true;
            float tp = Screen.Mode == Orientation.Portrait ? genmenu.Height : 0;
            mainlevels.Position = userlevels.Position = new Vector2(Screen.Mode == Orientation.Landscape ? genmenu.Width + (Screen.Width - genmenu.Width - d) / 2 : (Screen.Width - d) / 2, (Screen.Height - tp - d) / 2 + tp);
            mainlevels.Setup(); userlevels.Setup();
            mainlevels.FitCellSiblings();
            mainlevels.SnapTarget = target;
            mainlevels.Visible = true;
            userlevels.Visible = false;
        }

        protected virtual void ulcellpressed(UIButton sender)
        {
            string name = (string)(sender as UICell).Tag;
            Manager.Play(name, false);
        }
    }
}
