using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Inlumino_SHARED
{
    class StageContainer : IState
    {
        static Stage CurrentLevel = null;
        bool editmode;
        const int defaultstagewidth = 4;
        /// GUI //////////////////////////////////////////////////////////////////////////////////////////////
        List<UICell> cells = new List<UICell>();
        UIButton[] mainbuttons = null;
        UIButton[] editbuttons = null;
        UIButton[] borderbuttons = null;
        UIHud genhud;
        UIHud edithud;
        UIHud borderhud;
        bool einitd = false;
        public void InitEditing()
        {
            editing = true;
            if (einitd) { SetupHud(); return; }
            CurrentLevel = new Stage();
            CurrentLevel.ToggleEditMode();
            CurrentLevel.SetSize(defaultstagewidth, defaultstagewidth);
            CurrentLevel.setBackground(DataHandler.getTexture(0));
            einitd = true;
        }

        internal Stage getCurrentStage()
        {
            return CurrentLevel;
        }

        private void InitHud()
        {
            UIButton menubtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.MenuButton], 0, "menu");
            UIButton resetbtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.RestartButton], 0, "reset");
            mainbuttons = new UIButton[] { menubtn, resetbtn };
            UIButton togglebtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.ToggleButton], 0, "toggle");
            UIButton savebtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.SaveButton], 0, "save");
            UIButton delbtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.DeleteBtn], 0, "del");
            editbuttons = new UIButton[] { togglebtn, savebtn, delbtn };
            UIButton horzexp = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.RightButton], 0, "he");
            UIButton horzshrink = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.LeftButton], 0, "hs");
            UIButton vertexp = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.DownButton], 0, "ve");
            UIButton vertshrink = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.UpButton], 0, "vs");
            borderbuttons = new UIButton[] { horzshrink, vertshrink, horzexp, vertexp };
            foreach (UIButton b in mainbuttons) b.Pressed += hudbtnpressed;
            foreach (UIButton b in editbuttons) b.Pressed += hudbtnpressed;
            foreach (UIButton b in borderbuttons) b.Pressed += hudbtnpressed;
            foreach (ObjectType t in Common.EditorObjects)
            {
                UICell cell = new UICell(DataHandler.UIObjectsTextureMap[UIObjectType.Cell], t, DataHandler.ObjectTextureMap[t][0], 0.1f);
                cell.Pressed += cellpressed;
                cells.Add(cell);
            }
        }

        private void SetupHud()
        {
            IEnumerable<UIButton> allbuttons = getAllButtons();
            if (Screen.Mode == Orientation.Landscape)
            {
                genhud = new UIHud(allbuttons, Orientation.Portrait, Screen.Height * 0.4f, Screen.Height * 0.2f, 0, Screen.Height * (editmode ? 1 : 0.4f));
                genhud.Setup();
                Padding minpad;
                if (editing)
                {
                    edithud = new UIHud(cells.ToArray(), Orientation.Portrait, Screen.Height * 0.1f, Screen.Height * 0.1f, 0, Screen.Height);
                    edithud.Setup();
                    edithud.Position = new Vector2(Screen.Width - edithud.TotalWidth, 0);
                    borderhud = new UIHud(borderbuttons, Orientation.Portrait, Screen.Height * 0.1f, Screen.Height * 0.1f, 0, Screen.Height);
                    borderhud.Position = new Vector2(edithud.BoundingBox.Left - borderhud.TotalWidth, (Screen.Height - borderhud.TotalHeight) / 2);
                    minpad = new Padding(genhud.TotalWidth, edithud.TotalWidth + borderhud.TotalWidth, 0, 0);
                }
                else
                    minpad = new Padding(genhud.TotalWidth, 0, 0, 0);
                if (CurrentLevel != null)
                    CurrentLevel.SetMinScreenPadding(minpad);
            }
            else
            {
                genhud = new UIHud(allbuttons, Orientation.Landscape, Screen.Width * 0.32f, Screen.Width * 0.16f, Screen.Width * (editmode ? 1 : 0.64f), 0);
                genhud.Setup();
                Padding minpad;
                if (editing)
                {
                    selected = ObjectType.None;
                    edithud = new UIHud(cells.ToArray(), Orientation.Landscape, Screen.Width * 0.1f, Screen.Width * 0.1f, Screen.Width, 0);
                    edithud.Position = new Vector2(0, Screen.Height - edithud.TotalHeight);
                    edithud.Setup();
                    borderhud = new UIHud(borderbuttons, Orientation.Landscape, Screen.Width * 0.1f, Screen.Width * 0.1f, Screen.Width, 0);
                    borderhud.Position = new Vector2((Screen.Width - borderhud.TotalWidth) / 2, edithud.BoundingBox.Top - borderhud.TotalHeight);
                    borderhud.Setup();
                    minpad = new Padding(0, 0, genhud.TotalHeight, edithud.TotalHeight + borderhud.TotalHeight);
                }
                else
                    minpad = new Padding(0, 0, genhud.TotalHeight, 0);
                if (CurrentLevel != null) CurrentLevel.SetMinScreenPadding(minpad);
            }
        }

        private IEnumerable<UIButton> getAllButtons()
        {
            foreach (UIButton b in mainbuttons) yield return b;
            if (editmode) foreach (UIButton b in editbuttons) yield return b;
        }

        ObjectType selected = ObjectType.None;
        private void cellpressed(UIButton sender)
        {
            selected = (ObjectType)(sender as UICell).Tag;
        }
        private void hudbtnpressed(UIButton sender)
        {
            switch (sender.ID)
            {
                case "menu":
                    Manager.StateManager.SwitchTo(GameState.MainMenu); break;
                case "reset":
                    Reset(); break;
                case "toggle":
                    ToggleMode(); break;
                case "save":
                    SaveLevel(); break;
                case "del":
                    Manager.StateManager.SwitchTo(GameState.DeleteLevel); break;
                case "he":
                    CurrentLevel.SetSize(CurrentLevel.Width + 1, CurrentLevel.Height); break;
                case "hs":
                    CurrentLevel.SetSize(CurrentLevel.Width - 1, CurrentLevel.Height); break;
                case "ve":
                    CurrentLevel.SetSize(CurrentLevel.Width, CurrentLevel.Height + 1); break;
                case "vs":
                    CurrentLevel.SetSize(CurrentLevel.Width, CurrentLevel.Height - 1); break;
            }
        }

        private void SaveLevel()
        {
            if (CurrentLevel == null) return;
            Screen.MakeVirtual(new Vector2(256, 256));
            genhud.Visible = edithud.Visible = borderhud.Visible = false;
            CurrentLevel.SetMinScreenPadding(new Padding(0, 0, 0, 0));
            Texture2D icon = Manager.Parent.TakeScreenshot();
            Screen.MakeReal();
            Manager.StateManager.SwitchTo(GameState.SaveLevel, null, icon);
        }

        private void ToggleMode()
        {
            editing = !editing;
            CurrentLevel.ToggleEditMode();

            SetupHud();
        }

        private void Reset()
        {
            if (editing)
                CurrentLevel.Clear();
            else
                if (cln != null)
                loadLevel(cln, ism);
        }

        /// //////////////////////////////////////////////////////////////////////////////////////////////////        
        public StageContainer(bool editmode)
        {
            this.editmode = editmode;
            InitHud();
        }

        public void Clear()
        {
            if (CurrentLevel != null)
            {
                CurrentLevel.Clear();
                SetupHud();
            }
        }
        public void Draw(SpriteBatch batch)
        {
            if (CurrentLevel != null)
                CurrentLevel.Draw(batch);
            genhud.Draw(batch);
            if (editing)
            {
                edithud.Draw(batch);
                borderhud.Draw(batch);
            }
        }

        public void Update(GameTime time)
        {
            if (CurrentLevel != null)
            {
                if (InputManager.isKeyDown(Keys.Right))
                    CurrentLevel.Camera.StepHorizontal(5);
                if (InputManager.isKeyDown(Keys.Left))
                    CurrentLevel.Camera.StepHorizontal(-5);
                if (InputManager.isKeyDown(Keys.Down))
                    CurrentLevel.Camera.StepVertical(5);
                if (InputManager.isKeyDown(Keys.Up))
                    CurrentLevel.Camera.StepVertical(-5);
                if (InputManager.isKeyDown(Keys.Add))
                    CurrentLevel.Camera.Zoom(-0.02f);
                if (InputManager.isKeyDown(Keys.Subtract))
                    CurrentLevel.Camera.Zoom(0.02f);
                CurrentLevel.Update(time);
            }
            genhud.Update(time);
            if (editing)
            {
                edithud.Update(time);
                borderhud.Update(time);
            }
        }
        private bool editing = false;

        string cln = null;
        bool ism;
        public void loadLevel(string levelname, bool ismainlevel)
        {
            cln = levelname; ism = ismainlevel;
            CurrentLevel = Common.CreateLevel(levelname, ismainlevel);
            if (CurrentLevel != null)
                CurrentLevel.SetSourceStatus(true);
            else
                MessageBox.Show("Error", "The requested level was not found.\nGame might be corrupted or the cache was cleared while the game was running.", new string[] { "OK" });
            SetupHud();
        }

        public void HandleEvent(WorldEvent e, bool forcehandle = false)
        {
            genhud.HandleEvent(e);
            if (editing)
            {
                edithud.HandleEvent(e);
                borderhud.HandleEvent(e);
            }
            if (CurrentLevel == null) return;
            if (e is DisplaySizeChangedEvent || e is OrientationChangedEvent)
                SetupHud();
            if (e.Handled && !forcehandle) return;
            if (e is TouchPinchEvent)
            {
                CurrentLevel.Camera.Zoom((e as TouchPinchEvent).Delta);
            }

            if (e is TouchTapEvent)
            {
                Tile target = CurrentLevel.getTileAt(CurrentLevel.Camera.DeTransform((e as TouchTapEvent).Position));
                StaticObject obj = target == default(Tile) ? null : target.getObject();
                if (target != default(Tile) && editing && selected != ObjectType.None && (obj == null || obj.getType() != selected))
                {
                    if (selected == ObjectType.Delete) target.RemoveObject();
                    else
                    {
                        target.RemoveObject();
                        target.SetObject(StaticObjectCreator.CreateObject(selected, target));
                    }
                }
                else
                {
                    if (obj != null && !(obj is LightBeam))
                        obj.RotateCW(editing);
                }
            }

            if (e is TouchFreeDragEvent)
            {
                Vector2 delta = (e as TouchFreeDragEvent).Delta;
                CurrentLevel.Camera.StepHorizontal(-delta.X);
                CurrentLevel.Camera.StepVertical(-delta.Y);
            }

            if (e is MouseMovedEvent)
            {
                if (InputManager.isMouseVisible()) CurrentLevel.HighlightTileAt(CurrentLevel.Camera.DeTransform(InputManager.getMousePos().ToVector2()));
                if (InputManager.isMouseDown(InputManager.MouseKey.LeftKey))
                {
                    Point offset = (e as MouseMovedEvent).Offset;
                    CurrentLevel.Camera.StepHorizontal(offset.X);
                    CurrentLevel.Camera.StepVertical(offset.Y);
                }
            }

            if (e is MouseScrollEvent)
            {
                CurrentLevel.Camera.Zoom((e as MouseScrollEvent).Value / (float)Screen.Height);
            }

            if (e is MouseUpEvent)
            {
                MouseUpEvent ev = (e as MouseUpEvent);
                Tile target = CurrentLevel.getTileAt(CurrentLevel.Camera.DeTransform(ev.Position.ToVector2()));
                StaticObject obj = target == default(Tile) ? null : target.getObject();
                if (target != default(Tile) && editing && selected != ObjectType.None && (obj == null || obj.getType() != selected))
                {
                    if (selected == ObjectType.Delete || ev.Key == InputManager.MouseKey.RightKey) target.RemoveObject();
                    else
                    {
                        target.RemoveObject();
                        target.SetObject(StaticObjectCreator.CreateObject(selected, target));
                    }
                }
                else
                {
                    if (obj != null && !(obj is LightBeam))
                    {
                        if (ev.Key == InputManager.MouseKey.LeftKey) obj.RotateCCW(editing);
                        else if (ev.Key == InputManager.MouseKey.RightKey) obj.RotateCW(editing);
                    }
                }
            }
        }

        public void OnActivated(params object[] args)
        {
            if (args.Length > 1)
                loadLevel(args[0].ToString(), (bool)args[1]);
            SetupHud();
        }
    }
}
