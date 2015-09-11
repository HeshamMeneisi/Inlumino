using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Inlumino_SHARED
{
    class LevelSelector : IState
    {
        protected UIHud mainlevels;
        protected UIHud userlevels;
        protected UIHud genhud;
        protected UIButton menubtn;
        protected UIButton switchbtn;
        protected List<UICell> mlcells = new List<UICell>();

        public LevelSelector()
        {
            menubtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.MenuButton]);
            switchbtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.MainUser]);
            menubtn.Pressed += menupressed;
            switchbtn.Pressed += switchpressed;
            foreach (string name in Common.MainLevelNames)
            {
                UICell cell = new UICell(DataHandler.UIObjectsTextureMap[UIObjectType.Cell], name, "", Color.White, new TextureID(DataHandler.GetLevelThumb(name, true), name, 0), 0.1f);
                cell.Pressed += mlcellpressed;
                mlcells.Add(cell);
            }
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
            Manager.Play((string)(sender as UICell).Tag, true);
        }

        public void Draw(SpriteBatch batch)
        {
            genhud.Draw(batch);
            mainlevels.Draw(batch);
            userlevels.Draw(batch);
        }

        public void HandleEvent(WorldEvent e, bool forcehandle = false)
        {
            genhud.HandleEvent(e);
            mainlevels.HandleEvent(e);
            userlevels.HandleEvent(e);
            if (e is OrientationChangedEvent || e is DisplaySizeChangedEvent)
                SetupHud();
        }

        public void Update(GameTime time)
        {
            genhud.Update(time);
            mainlevels.Update(time);
            userlevels.Update(time);
        }

        public void OnActivated(params object[] args)
        {
            SetupHud();
        }

        protected virtual void SetupHud()
        {
            if (Screen.Mode == Orientation.Landscape) // Landscape
            {
                genhud = new UIHud(new UIButton[] { menubtn, switchbtn }, Orientation.Portrait, Screen.Height * 0.4f, Screen.Height * 0.2f, 0, Screen.Height * 0.4f);
                genhud.Setup();
                List<UICell> ulcells = new List<UICell>();
                foreach (string name in DataHandler.getSavedLevelNames())
                {
                    UICell cell = new UICell(DataHandler.UIObjectsTextureMap[UIObjectType.Cell], name, name, Color.White, new TextureID(DataHandler.GetLevelThumb(name, false), name, 0), 0.1f);
                    cell.Pressed += ulcellpressed;
                    ulcells.Add(cell);
                }
                mainlevels = new UIHud(mlcells.ToArray(), Orientation.Landscape, Screen.SmallDim * 0.2f, Screen.SmallDim * 0.2f, Screen.Width - genhud.TotalWidth, Screen.Height);
                userlevels = new UIHud(ulcells.ToArray(), Orientation.Landscape, Screen.SmallDim * 0.2f, Screen.SmallDim * 0.2f, Screen.Width - genhud.TotalWidth, Screen.Height);
                mainlevels.Position = userlevels.Position = new Vector2(genhud.TotalWidth, 0);
                mainlevels.Setup(); userlevels.Setup();
                mainlevels.Visible = true;
                userlevels.Visible = false;
            }
            else
            {
                genhud = new UIHud(new UIButton[] { menubtn, switchbtn }, Orientation.Landscape, Screen.Width * 0.4f, Screen.Width * 0.2f, Screen.Width * 0.8f, 0);
                genhud.Setup();
                List<UICell> ulcells = new List<UICell>();
                foreach (string name in DataHandler.getSavedLevelNames())
                {
                    UICell cell = new UICell(DataHandler.UIObjectsTextureMap[UIObjectType.Cell], name, name, Color.White, new TextureID(DataHandler.GetLevelThumb(name, false), name, 0), 0.1f);
                    cell.Pressed += ulcellpressed;
                    ulcells.Add(cell);
                }
                mainlevels = new UIHud(mlcells.ToArray(), Orientation.Landscape, Screen.SmallDim * 0.2f, Screen.SmallDim * 0.2f, Screen.Width, Screen.Height - genhud.TotalHeight);
                userlevels = new UIHud(ulcells.ToArray(), Orientation.Landscape, Screen.SmallDim * 0.2f, Screen.SmallDim * 0.2f, Screen.Width, Screen.Height - genhud.TotalHeight);
                mainlevels.Position = userlevels.Position = new Vector2(0, genhud.TotalHeight);
                mainlevels.Setup(); userlevels.Setup();
                mainlevels.Visible = true;
                userlevels.Visible = false;
            }
        }

        protected virtual void ulcellpressed(UIButton sender)
        {
            Manager.Play((string)(sender as UICell).Tag, false);
        }
    }
}
