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
        UIHud mainlevels;
        UIHud userlevels;
        UIHud genhud;
        UIButton menubtn;
        UIButton switchbtn;
        List<UICell> mlcells = new List<UICell>();
        UIButton[] genbuttons;

        public LevelSelector()
        {
            menubtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.MenuButton]);
            switchbtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.MainUser]);
            menubtn.Pressed += menupressed;
            switchbtn.Pressed += switchpressed;
            genbuttons = new UIButton[] { menubtn, switchbtn };
            foreach (string name in Common.MainLevelNames)
            {
                UICell cell = new UICell(DataHandler.UIObjectsTextureMap[UIObjectType.Cell], name, name, Color.White, new TextureID(DataHandler.GetLevelThumb(name, true), name, 0), 0.1f);
                cell.Pressed += mlcellpressed;
                mlcells.Add(cell);
            }
        }

        private void switchpressed(UIButton sender)
        {
            mainlevels.Visible = !mainlevels.Visible;
            userlevels.Visible = !userlevels.Visible;
        }

        private void menupressed(UIButton sender)
        {
            Manager.StateManager.SwitchTo(GameState.MainMenu);
        }

        private void mlcellpressed(UIButton sender)
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

        private void SetupHud()
        {
            if (Screen.Width > Screen.Height) // Landscape
            {
                genhud = new UIHud(genbuttons, Orientation.Portrait, 64, 32, 256, 256);
                genhud.Setup();
                List<UICell> ulcells = new List<UICell>();
                foreach (string name in DataHandler.getSavedLevelNames())
                {
                    UICell cell = new UICell(DataHandler.UIObjectsTextureMap[UIObjectType.Cell], name, name, Color.White, new TextureID(DataHandler.GetLevelThumb(name, false), name, 0), 0.1f);
                    cell.Pressed += ulcellpressed;
                    ulcells.Add(cell);
                }
                mainlevels = new UIHud(mlcells.ToArray(), Orientation.Portrait, 64, 64, Screen.Width - genhud.TotalWidth, Screen.Height);
                userlevels = new UIHud(ulcells.ToArray(), Orientation.Portrait, 64, 64, Screen.Width - genhud.TotalWidth, Screen.Height);
                mainlevels.Position = userlevels.Position = new Vector2(genhud.TotalWidth, 0);
                mainlevels.Setup(); userlevels.Setup();
                mainlevels.Visible = true;
                userlevels.Visible = false;
            }
            else
            {
                genhud = new UIHud(genbuttons, Orientation.Landscape, 64, 32, 256, 256);
                genhud.Setup();
                List<UICell> ulcells = new List<UICell>();
                foreach (string name in DataHandler.getSavedLevelNames())
                {
                    UICell cell = new UICell(DataHandler.UIObjectsTextureMap[UIObjectType.Cell], name, name, Color.White, new TextureID(DataHandler.GetLevelThumb(name, false), name, 0), 0.1f);
                    cell.Pressed += ulcellpressed;
                    ulcells.Add(cell);
                }
                mainlevels = new UIHud(mlcells.ToArray(), Orientation.Landscape, 64, 64, Screen.Width, Screen.Height - genhud.TotalHeight);
                userlevels = new UIHud(ulcells.ToArray(), Orientation.Landscape, 64, 64, Screen.Width, Screen.Height - genhud.TotalHeight);
                mainlevels.Position = userlevels.Position = new Vector2(0, genhud.TotalHeight);
                mainlevels.Setup(); userlevels.Setup();
                mainlevels.Visible = true;
                userlevels.Visible = false;
            }
        }

        private void ulcellpressed(UIButton sender)
        {
            Manager.Play((string)(sender as UICell).Tag, false);
        }
    }
}
