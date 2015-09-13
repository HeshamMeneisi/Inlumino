﻿using System;
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
        int perfmoves = 0;
        /// GUI //////////////////////////////////////////////////////////////////////////////////////////////
        List<UICell> cells = new List<UICell>();
        UICell[] borderbuttons = null;
        UIMenu genmenu;
        UIHud edithud;
        UIHud borderhud;
        UIButton nextbtn;
        UIVisibleObject log;
        UIButton menubtn;
        UIButton resetbtn;
        UIButton togglebtn;
        UIButton savebtn;
        UIButton delbtn;
        TextureID border = DataHandler.UIObjectsTextureMap[UIObjectType.Border][0];
        float borderwidth = 16;
        bool einitd = false;
        public void InitEditing()
        {
            editing = true;
            if (CurrentLevel == null)
            {
                CurrentLevel = new Stage();
                CurrentLevel.ToggleEditMode();
                CurrentLevel.SetSize(defaultstagewidth, defaultstagewidth);
                CurrentLevel.setBackground(DataHandler.getTexture(0));
                CurrentLevel.LevelWon += levelwon;
            }
            else if (!CurrentLevel.EditMode) CurrentLevel.ToggleEditMode();
            SetupHud();
        }

        internal Stage getCurrentStage()
        {
            return CurrentLevel;
        }

        private void InitHud()
        {
            menubtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.MenuButton], hudbtnpressed, 0, "menu");
            resetbtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.RestartButton], hudbtnpressed, 0, "reset");
            togglebtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.ToggleButton], hudbtnpressed, 0, "toggle");
            savebtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.SaveButton], hudbtnpressed, 0, "save");
            delbtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.DeleteBtn], hudbtnpressed, 0, "del");
            UICell horzexp = new UICell(DataHandler.UIObjectsTextureMap[UIObjectType.RightButton], "he", null, 0, "", default(Color), hudbtnpressed, 0);
            UICell horzshrink = new UICell(DataHandler.UIObjectsTextureMap[UIObjectType.LeftButton], "hs", null, 0, "", default(Color), hudbtnpressed, 0);
            UICell vertexp = new UICell(DataHandler.UIObjectsTextureMap[UIObjectType.DownButton], "ve", null, 0, "", default(Color), hudbtnpressed, 0);
            UICell vertshrink = new UICell(DataHandler.UIObjectsTextureMap[UIObjectType.UpButton], "vs", null, 0, "", default(Color), hudbtnpressed, 0);
            borderbuttons = new UICell[] { horzshrink, vertshrink, horzexp, vertexp };
            UIButton backbtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.BackButton], hudbtnpressed, 0, "back");
            nextbtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.Next], hudbtnpressed, 0, "next");
            genmenu = new UIMenu();
            genmenu.Add(menubtn);
            genmenu.Add(resetbtn);
            genmenu.Add(togglebtn);
            genmenu.Add(savebtn);
            genmenu.Add(delbtn);
            foreach (ObjectType t in Common.EditorObjects)
            {
                UICell cell = new UICell(DataHandler.UIObjectsTextureMap[UIObjectType.Cell], t, DataHandler.ObjectTextureMap[t][0], 0.1f, "", Color.White, cellpressed);
                cells.Add(cell);
            }
        }
        // Transition        
        UICell stars = null;
        bool trans = false;
        bool verified = false;
        private void levelwon()
        {
            verified = true;
            if (editmode) return;
            if (ism)
            {
                trans = true;
                SoundManager.PlaySound(DataHandler.Sounds[SoundType.AllCrystalsLit], SoundCategory.SFX);
                CurrentLevel.Pause();
                SetupHud();
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////
        private void SetupHud()
        {
            togglebtn.Visible = editmode;
            savebtn.Visible = delbtn.Visible = editing;
            genmenu.setAllSizeRelative(0.15f * (Screen.Mode == Orientation.Portrait ? 2 : 1), Screen.Mode);
            genmenu.ArrangeInForm(Common.ReverseOrientation(Screen.Mode));
            Padding minpad;
            if (editing)
            {
                float d = Screen.SmallDim * 0.1f;
                edithud = new UIHud(cells.ToArray(), Common.ReverseOrientation(Screen.Mode), d, d, Screen.Mode == Orientation.Landscape ? 2 * d : Screen.Width, Screen.Mode == Orientation.Landscape ? Screen.Height : 2 * d);
                edithud.Setup();
                edithud.Position = Screen.Mode == Orientation.Landscape ? new Vector2(Screen.Width - edithud.ActualWidth, 0) : new Vector2(0, Screen.Height - edithud.ActualHeight);
                borderhud = new UIHud(borderbuttons, Screen.Mode, d, d, Screen.Mode == Orientation.Landscape ? d : Screen.Width, Screen.Mode == Orientation.Landscape ? Screen.Height : d);
                borderhud.Setup();
                borderhud.Position = Screen.Mode == Orientation.Landscape ?
                    new Vector2(edithud.BoundingBox.Left - borderhud.ActualWidth, (Screen.Height - borderhud.ActualHeight) / 2)
                    : new Vector2((Screen.Width - borderhud.ActualWidth) / 2, edithud.BoundingBox.Top - borderhud.ActualHeight);
                minpad = Screen.Mode == Orientation.Landscape ?
                    new Padding(genmenu.Width, edithud.ActualWidth + borderhud.ActualWidth, 0, 0)
                    : new Padding(0, 0, genmenu.Height, edithud.ActualHeight + borderhud.ActualHeight);
            }
            else
                minpad = new Padding(Screen.Mode == Orientation.Landscape ? genmenu.Width : 0, 0, Screen.Mode == Orientation.Landscape ? 0 : genmenu.Height, 0);
            if (CurrentLevel != null)
                CurrentLevel.SetMinScreenPadding(minpad);
            if (trans)
            {
                float p = Screen.Mode == Orientation.Portrait ? genmenu.Height : 0;
                nextbtn.Visible = SetStars();
                log = new UIVisibleObject(DataHandler.UIObjectsTextureMap[UIObjectType.Log]);
                log.setSizeRelative(0.7f, Screen.Mode);
                float d = log.Width * 0.2f;
                nextbtn.setSizeRelative(0.25f, Screen.Mode);
                log.Position = new Vector2((Screen.Width - log.Width) / 2, p);
                stars.Position = new Vector2((Screen.Width - stars.Width) / 2, log.GlobalPosition.Y + log.Height * 0.5f);
                nextbtn.Position = new Vector2((Screen.Width - nextbtn.Width) / 2, stars.BoundingBox.Bottom);
            }
        }
        int moves = 0;
        private bool SetStars()
        {
            // For now
            //int perfmoves = Common.GetMoves(cln);
            int s = (moves <= perfmoves + 10 ? 1 : 0)
            + (moves <= perfmoves + 5 ? 1 : 0)
            + (moves <= perfmoves + 2 ? 1 : 0);
            Common.SetScore(cln, s);
            stars = new UICell(new TextureID[] { Common.GetStarsTex(s) }, "");
            return Common.GetScore(cln) != 0;
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
                case "back": Reset(); break;
                case "next": NextLevel(); break;
            }
        }

        private void NextLevel()
        {
            Common.NextLevel(cln);
        }

        private void SaveLevel()
        {
            if (CurrentLevel == null) return;
            if (!verified) { MessageBox.Show("Proof of concept", "Please solve the level in play mode then switch to edit mode and try again.", new string[] { "OK" }); return; }
            if (!editing) ToggleMode();
            CurrentLevel.DisableGrid();
            Screen.MakeVirtual(new Vector2(512, 512));
            genmenu.Visible = edithud.Visible = borderhud.Visible = false;
            CurrentLevel.SetMinScreenPadding(new Padding(0, 0, 0, 0));
            Texture2D icon = Manager.Parent.TakeScreenshot();
            Screen.MakeReal();
            genmenu.Visible = edithud.Visible = borderhud.Visible = true;
            CurrentLevel.EnableGrid();
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
            genmenu.Draw(batch);
            if (trans)
            {
                log.Draw(batch);
                stars.Draw(batch);
                nextbtn.Draw(batch);
            }
            if (editing)
            {
                edithud.Draw(batch);
                borderhud.Draw(batch);
            }
            /*
            if (border != null)
            {
                float l = CurrentLevel.Camera.StagePadding.Left,
                    r = Screen.Width - CurrentLevel.Camera.StagePadding.Right,
                    t = CurrentLevel.Camera.StagePadding.Top,
                    b = Screen.Height - CurrentLevel.Camera.StagePadding.Bottom,
                    fuzz = CurrentLevel.Camera.GetRecommendedDrawingFuzz();
                RectangleF bottomrect = new RectangleF(l, b + borderwidth, Screen.Width - l - r, borderwidth);
                batch.Draw(DataHandler.getTexture(border.GroupIndex), bottomrect.getSmoothRectangle(fuzz), DataHandler.getTextureSource(border), Color.White);
            }*/
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
            genmenu.Update(time);
            if (trans)
            {
                stars.Update(time);
                nextbtn.Update(time);
            }
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
            trans = false; moves = 0;
            cln = levelname; ism = ismainlevel;
            CurrentLevel = Common.CreateLevel(levelname, ismainlevel);
            if (CurrentLevel != null)
            {
                CurrentLevel.SetSourceStatus(true);
                if (!ismainlevel)
                {
                    InitEditing();
                    editing = false;
                    editmode = true;
                }
                else
                {
                    editmode = editing = false;
                    CurrentLevel.SetSourceStatus(false);
                    perfmoves = ShuffleLevel();
                    CurrentLevel.SetSourceStatus(true);
                }
                CurrentLevel.LevelWon += levelwon;
                SetupHud();
                Common.SpreadAuxiliaries(CurrentLevel, 0.1f);
            }
            else
                MessageBox.Show("Error", "The requested level was not found.\nGame might be corrupted or the cache was cleared while the game was running.", new string[] { "OK" });
        }

        private int ShuffleLevel()
        {
            Random ran = new Random();
            int moves = 0;
            foreach (Tile t in CurrentLevel.getTileMap().AllTiles)
                if (t.hasObject() && !(t.getObject() is LightBeam) && !(t.getObject() is LightSourceObject) && !(t.getObject() is Crystal) && !(t.getObject() is Block))
                    for (int i = ran.Next(1, 4); i > 0; i--)
                    {
                        moves++;
                        t.getObject().RotateCCW(true);
                    }
            return moves;
        }

        public void HandleEvent(WorldEvent e, bool forcehandle = false)
        {
            genmenu.HandleEvent(e);
            if (trans)
            {
                nextbtn.HandleEvent(e);
            }
            if (editing)
            {
                edithud.HandleEvent(e);
                borderhud.HandleEvent(e);
            }
            if (CurrentLevel == null) return;
            if (trans) return;
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
                        verified = false;
                        target.RemoveObject();
                        target.SetObject(StaticObjectCreator.CreateObject(selected, target));
                    }
                }
                else
                {
                    if (obj != null && !(obj is LightBeam))
                    { verified = false; obj.RotateCW(editing); moves++; }
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
                if (editing && InputManager.isMouseVisible()) CurrentLevel.HighlightTileAt(CurrentLevel.Camera.DeTransform(InputManager.getMousePos().ToVector2()));
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
                verified = false;
                MouseUpEvent ev = (e as MouseUpEvent);
                Tile target = CurrentLevel.getTileAt(CurrentLevel.Camera.DeTransform(ev.Position.ToVector2()));
                StaticObject obj = target == default(Tile) ? null : target.getObject();
                if (target != default(Tile) && editing && selected != ObjectType.None && (obj == null || obj.getType() != selected))
                {
                    if (selected == ObjectType.Delete || ev.Key == InputManager.MouseKey.RightKey) target.RemoveObject();
                    else
                    {
                        verified = false;
                        target.RemoveObject();
                        target.SetObject(StaticObjectCreator.CreateObject(selected, target));
                    }
                }
                else
                {
                    if (obj != null && !(obj is LightBeam))
                    {
                        verified = false;
                        obj.RotateCW(editing);
                        moves++;
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
