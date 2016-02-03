using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Parse;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inlumino_SHARED
{
    class PackageSelector : IState
    {
        UIVisibleObject mem = null;
        protected UIGrid packages;
        protected UIMenu genmenu;
        protected UIButton menubtn;
        protected List<UICell> cells = new List<UICell>();
        internal PackageSelector()
        {
            menubtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.MenuButton]);
            menubtn.Pressed += menupressed;
            genmenu = new UIMenu();
            genmenu.Add(menubtn);
        }

        protected virtual void menupressed(UIButton sender)
        {
            Manager.StateManager.SwitchTo(GameState.MainMenu);
        }

        protected async void mlcellpressed(UIButton sender)
        {
            PackageType name = (PackageType)(sender as UICell).Tag;
            if(name == PackageType.Online && ParseUser.CurrentUser == null)
            {
                AlertHandler.ShowMessage("Ops", "You are not logged in or your session has expired. Please login or relogin to use online features.", new string[] { "Ok" });
                return;
            }
            if (Common.IsPackageLocked(name))
            {
#if ANDROID && !DISABLEONLINE
                int? r = await AlertHandler.ShowMessage("Locked", "Finish other packages to unlock this package. Or help us spread the word on facebook and unlock now!", new string[] { "Ok", "Unlock Now!" });
                if (r == 1)
                    await Common.UnlockWithFacebook(name);
#else
                AlertHandler.ShowMessage("Locked", "Finish other packages to unlock this package!", new string[] { "Ok" });
#endif
                return;
            }
            mem = packages.SnapTarget;
            Manager.StateManager.SwitchTo(GameState.SelectLevel, null, name);
        }

        public void Draw(SpriteBatch batch)
        {
            Texture2D background = DataHandler.getTexture(PrimaryTexture._MMBG);
            float w = Screen.Height * background.Width / background.Height;
            batch.Draw(background, new Rectangle((int)(Screen.Width - w) / 2, 0, (int)(w), (int)Screen.Height), Color.White);
            genmenu.Draw(batch);
            packages.Draw(batch);
        }

        public void HandleEvent(WorldEvent e, bool forcehandle = false)
        {
            genmenu.HandleEvent(e);
            packages.HandleEvent(e);
            if (e is OrientationChangedEvent || e is DisplaySizeChangedEvent)
                SetupHud();
            else
            {
                if (InputManager.isKeyDown(Keys.Right))
                    packages.SlideRight();
                if (InputManager.isKeyDown(Keys.Left))
                    packages.SlideLeft();
            }
        }

        public void Update(GameTime time)
        {
            genmenu.Update(time);
            packages.Update(time);
        }

        public void OnActivated(params object[] args)
        {
            SetupHud();
        }
        protected virtual void SetupHud()
        {
            genmenu.setAllSizeRelative(0.2f * (Screen.Mode == Orientation.Portrait ? 2 : 1), Screen.Mode);
            genmenu.ArrangeInForm(Common.ReverseOrientation(Screen.Mode));
            //List<UICell> ulcells = new List<UICell>();
            cells.Clear();
            foreach (PackageType pack in Common.Packages.Keys)
            {                
                UICell cell = new UICell(DataHandler.UIObjectsTextureMap[UIObjectType.Frame], pack, "", Color.White, new TextureID(() => DataHandler.GetPackageThumb(pack), pack.ToString(), 0, -1, -1), 0.1f);
                cell.Pressed += mlcellpressed;
                if (Common.IsPackageLocked(pack)) cell.AttachSibling(new UIVisibleObject(new TextureID[] { DataHandler.UIObjectsTextureMap[UIObjectType.Lock][0] }));
                cell.FitSiblings();
                cells.Add(cell);
            }
            float d = Math.Min(Screen.SmallDim, Screen.BigDim * 0.6f);
            packages = new UIGrid(cells.ToArray(), Orientation.Portrait, d, d, 1, true, d, d);
            float tp = Screen.Mode == Orientation.Portrait ? genmenu.Height : 0;
            packages.Position = new Vector2(Screen.Mode == Orientation.Landscape ? genmenu.Width + (Screen.Width - genmenu.Width - d) / 2 : (Screen.Width - d) / 2, (Screen.Height - tp - d) / 2 + tp);
            packages.FitCellSiblings();
            packages.SnapCameraToCells = true;
            packages.Visible = true;
            packages.SnapTarget = mem;
        }
    }
}
