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
        Stage CurrentLevel = null;
        bool editmode;
        bool interacted = true;
        const int defaultstagewidth = 4;
        int perfmoves = 0;
        /// GUI //////////////////////////////////////////////////////////////////////////////////////////////
        List<UICell> cells = new List<UICell>();
        UICell[] borderbuttons = null;
        UIMenu genmenu;
        UIMenu popup;
        UIGrid edithud;
        UIGrid borderhud;
        UIButton nextbtn;
        UIVisibleObject toplog;
        UICell log;
        UIVisibleObject ropes1;
        UIVisibleObject ropes2;
        UIVisibleObject stars;
        UIButton trybtn;
        UIButton menubtn;
        UIButton resetbtn;
        UIButton togglebtn;
        UIButton savebtn;
        UITextField counter;
        TextureID border = DataHandler.UIObjectsTextureMap[UIObjectType.Border][0];
        internal void InitEditing()
        {
            pack = PackageType.None;
            editmode = editing = true;
            trans = popup.Visible = false;
            if (CurrentLevel == null)
            {
                CurrentLevel = new Stage();
                CurrentLevel.ToggleEditMode();
                CurrentLevel.SetSize(defaultstagewidth, defaultstagewidth);
                CurrentLevel.setBackground(DataHandler.getTexture(PrimaryTexture._BG));
                CurrentLevel.LevelWon += levelwon;
            }
            else if (!CurrentLevel.EditMode) CurrentLevel.ToggleEditMode();
            CurrentLevel.EnableGrid();
            SetupHud();
        }

        internal Stage getCurrentStage()
        {
            return CurrentLevel;
        }

        private void InitHud()
        {
            menubtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.MenuButton], hudbtnpressed, 0, "menu");
            resetbtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.ResetButton], hudbtnpressed, 0, "reset");
            togglebtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.ToggleButton], hudbtnpressed, 0, "toggle");
            savebtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.SaveButton], hudbtnpressed, 0, "save");
            UICell horzexp = new UICell(DataHandler.UIObjectsTextureMap[UIObjectType.RightButton], "he", null, 0, "", default(Color), hudbtnpressed, 0);
            UICell horzshrink = new UICell(DataHandler.UIObjectsTextureMap[UIObjectType.LeftButton], "hs", null, 0, "", default(Color), hudbtnpressed, 0);
            UICell vertexp = new UICell(DataHandler.UIObjectsTextureMap[UIObjectType.DownButton], "ve", null, 0, "", default(Color), hudbtnpressed, 0);
            UICell vertshrink = new UICell(DataHandler.UIObjectsTextureMap[UIObjectType.UpButton], "vs", null, 0, "", default(Color), hudbtnpressed, 0);
            borderbuttons = new UICell[] { horzshrink, vertshrink, horzexp, vertexp };
            UIButton backbtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.BackButton], hudbtnpressed, 0, "back");
            nextbtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.Next], hudbtnpressed, 0, "next");
            trybtn = new UIButton(DataHandler.UIObjectsTextureMap[UIObjectType.TryAgain], hudbtnpressed, 0, "reset");
            log = new UICell(DataHandler.UIObjectsTextureMap[UIObjectType.Log], "");
            stars = new UICell(new TextureID[] { Common.GetStarsTex(0), Common.GetStarsTex(1), Common.GetStarsTex(2), Common.GetStarsTex(3) }, "");
            log.AttachSibling(stars);
            ropes1 = new UIVisibleObject(DataHandler.UIObjectsTextureMap[UIObjectType.Ropes]);
            ropes2 = new UIVisibleObject(DataHandler.UIObjectsTextureMap[UIObjectType.Ropes]);
            toplog = new UIVisibleObject(DataHandler.UIObjectsTextureMap[UIObjectType.TopLog]);
            counter = new UITextField(16, Color.Gold, Color.Black);
            genmenu = new UIMenu();
            genmenu.Add(menubtn);
            genmenu.Add(resetbtn);
            genmenu.Add(counter);
            genmenu.Add(togglebtn);
            genmenu.Add(savebtn);
            popup = new UIMenu();
            popup.Add(nextbtn);
            popup.Add(trybtn);
            popup.Add(ropes2);
            popup.Add(log);
            popup.Add(ropes1);
            popup.Add(toplog);
            foreach (ObjectType t in Common.EditorObjects)
            {
                UICell cell = new UICell(DataHandler.UIObjectsTextureMap[UIObjectType.Cell], t, DataHandler.ObjectTextureMap[t][0], 0.1f, "", Color.White, cellpressed);
                cells.Add(cell);
            }
        }
        // Transition        
        bool trans = false;
        bool verified = false;
        private void levelwon()
        {
            if (loading || editing) return;
            verified = true;
            if (pack == PackageType.None) return;
            trans = true;
            SoundManager.PlaySound(DataHandler.Sounds[SoundType.AllCrystalsLit], SoundCategory.SFX);
            CurrentLevel.Pause();
            SetupHud();
        }
        ////////////////////////////////////////// Setup Hud ///////////////////////////////////////////////////////
        private void SetupHud()
        {
            togglebtn.Visible = editmode;
            counter.Visible = !editing && pack != PackageType.None;
            savebtn.Visible = editing;
            genmenu.setAllSizeRelative(0.15f * (Screen.Mode == Orientation.Portrait ? 2 : 1), Screen.Mode);
            counter.Size = menubtn.Size;
            genmenu.ArrangeInForm(Common.ReverseOrientation(Screen.Mode));
            Padding minpad;
            if (editing)
            {
                float d = Screen.SmallDim / 6;
                edithud = new UIGrid(cells.ToArray(), Common.ReverseOrientation(Screen.Mode), Screen.Width, Screen.Height, 8,true,d,d);
                //edithud.ShowEntireRowCol();
                edithud.TrimGridToVisible();
                edithud.Position = Screen.Mode == Orientation.Landscape ? new Vector2(Screen.Width - edithud.Width, 0) : new Vector2(0, Screen.Height - edithud.Height);
                d = Screen.SmallDim / 8;
                borderhud = new UIGrid(borderbuttons, Common.ReverseOrientation(Screen.Mode), Screen.Width, Screen.Height, 4, true, d, d);
                borderhud.TrimGridToVisible();
                borderhud.Position = Screen.Mode == Orientation.Landscape ?
                    new Vector2(edithud.BoundingBox.Left - borderhud.Width, (Screen.Height - borderhud.Height) / 2)
                    : new Vector2((Screen.Width - borderhud.Width) / 2, edithud.BoundingBox.Top - borderhud.Height);
                minpad = Screen.Mode == Orientation.Landscape ?
                    new Padding(genmenu.Width, edithud.Width + borderhud.Width, 0, 0)
                    : new Padding(0, 0, genmenu.Height, edithud.Height + borderhud.Height);
            }
            else
                minpad = new Padding(Screen.Mode == Orientation.Landscape ? genmenu.Width : 0, 0, Screen.Mode == Orientation.Landscape ? 0 : genmenu.Height, 0);
            if (CurrentLevel != null)
                CurrentLevel.SetMinScreenPadding(minpad);
            if (trans)
            {
                float p = Screen.Mode == Orientation.Portrait ? genmenu.Height : 0,
                ah = Screen.Height - p;
                bool passed = SetStars();
                toplog.State = passed ? 0 : 1;
                nextbtn.Visible = passed && IsCurrentMain;
                trybtn.Visible = !passed && IsCurrentMain;
                ropes2.Visible = nextbtn.Visible || trybtn.Visible;
                popup.setAllSizeRelative(ah / 5 / Screen.Height, Orientation.Landscape);
                log.FitSiblings();
                //  Element positions
                toplog.Position = new Vector2(0, 0);
                ropes1.Position = new Vector2(0, toplog.LocalBoundingBox.Bottom - toplog.Height * 0.2f);
                log.Position = new Vector2(0, ropes1.LocalBoundingBox.Bottom - ropes1.Height * 0.4f);
                ropes2.Position = new Vector2(0, log.LocalBoundingBox.Bottom - log.Height * 0.2f);
                nextbtn.Position = trybtn.Position = new Vector2(0, ropes2.LocalBoundingBox.Bottom - ropes2.Height * 0.2f);
                log.CentralizeSiblings();
                ///
                if (Screen.Mode == Orientation.Landscape)
                    popup.Position = new Vector2((Screen.Width - genmenu.Width - popup.Width) / 2 + genmenu.Width, 0);
                else popup.Position = new Vector2((Screen.Width - popup.Width) / 2, genmenu.Height);
                popup.Visible = true;
            }
        }
        int moves = 0;
        private bool SetStars()
        {
            // For now
            //int perfmoves = Common.GetMoves(cln);
            int s = (moves <= (int)(perfmoves * 1.5) ? 1 : 0)
            + (moves <= (int)(perfmoves * 1.25) ? 1 : 0)
            + (moves <= (int)(perfmoves * 1.1) ? 1 : 0);
            Common.SetScore(pack, cln, s);
            stars.State = s;
            if (!IsCurrentMain) return s != 0;
            return Common.GetScore(pack, cln) != 0;
        }
        private void UpdateCounter()
        {
            counter.ForegroundColor = moves <= (int)(perfmoves * 1.1) ? Color.Gold : moves <= (int)(perfmoves * 1.25) ? Color.Silver : moves <= (int)(perfmoves * 1.5) ? Color.Brown : Color.DarkRed;
            counter.Text = moves + "/" + perfmoves;
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
            Common.NextLevel(cln, pack);
        }

        private void SaveLevel()
        {
            if (CurrentLevel == null) return;
            if (!verified) { MessageBox.Show("Proof of concept", "Please solve the level in play mode then switch to edit mode and try again.", new string[] { "OK" }); return; }
            if (!editing) ToggleMode();
            CurrentLevel.DisableGrid();
            Manager.StateManager.SwitchTo(GameState.SaveLevel, null, CurrentLevel, IsCurrentMain ? "" : cln);
        }

        private void ToggleMode()
        {
            moves = 0; UpdateCounter(); trans = popup.Visible = false; pack = PackageType.None;
            editing = !editing;
            CurrentLevel.ToggleEditMode();
            interacted = true;
            SetupHud();
        }
        private void Reset()
        {
            if (editing)
                CurrentLevel.Clear();
            else
                if (cln != null)
                loadLevel(cln, pack);
        }

        /// //////////////////////////////////////////////////////////////////////////////////////////////////        
        internal StageContainer(bool editmode)
        {
            this.editmode = editmode;
            InitHud();
        }

        internal void Clear()
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
                popup.Draw(batch);
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
                if (interacted)
                {
                    interacted = false;
                    CurrentLevel.CheckWin();
                }
            }
            genmenu.Update(time);
            if (trans)
                popup.Update(time);
            if (editing)
            {
                edithud.Update(time);
                borderhud.Update(time);
            }
        }
        private bool editing = false;

        string cln = null;
        bool IsCurrentMain { get { return pack != PackageType.User && pack != PackageType.Online; } }
        bool loading = false;
        PackageType pack = PackageType.User;
        internal void loadLevel(string levelname, PackageType package = PackageType.User)
        {
            loading = true;
            trans = false; moves = 0; UpdateCounter(); pack = package;
            cln = levelname;
            CurrentLevel = Common.CreateLevel(levelname, pack);
            if (CurrentLevel != null)
            {
                if (!IsCurrentMain)
                {
                    InitEditing();
                    pack = package;
                    editing = false;
                }
                else
                    editmode = editing = false;
                if (!CurrentLevel.EditMode)
                    CurrentLevel.SetSourceStatus(false);
                perfmoves = ShuffleLevel();
                UpdateCounter();
                if (CurrentLevel.EditMode) CurrentLevel.ToggleEditMode();
                else CurrentLevel.SetSourceStatus(true);
                CurrentLevel.LevelWon += levelwon;
                SetupHud();
                Common.SpreadAuxiliaries(CurrentLevel, 0.1f);
            }
            else
                MessageBox.Show("Error", "The requested level was not found.\nGame might be corrupted or the cache was cleared while the game was running.", new string[] { "OK" });
            loading = false;
            interacted = true;
        }

        private int ShuffleLevel()
        {
            Random ran = new Random();
            int mv = 0;
            foreach (Tile t in CurrentLevel.getTileMap().AllTiles)
                if (t.hasObject() && t.getObject().IsInteractable)
                    for (int i = ran.Next(1, 4); i > 0; i--)
                    {
                        mv++;
                        t.getObject().RotateCCW(true);
                    }
            return mv;
        }
        bool dragging = false;
        public void HandleEvent(WorldEvent e, bool forcehandle = false)
        {
            genmenu.HandleEvent(e);
            if (trans)
                popup.HandleEvent(e);
            if (editing)
            {
                edithud.HandleEvent(e);
                borderhud.HandleEvent(e);
            }
            if (e is DisplaySizeChangedEvent || e is OrientationChangedEvent)
                SetupHud();
            if (CurrentLevel == null) return;
            if (trans) return;
            if (e.Handled && !forcehandle)
                return;

            if (e is TouchPinchEvent)
            {
                CurrentLevel.Camera.Zoom((e as TouchPinchEvent).Delta);
                return;
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
                    if (obj != null && (obj.IsInteractable || editing))
                    {
                        verified = false; interacted = true;
                        obj.RotateCW(editing);
                        moves++;
                        UpdateCounter();
                    }
                }
                return;
            }

            if (e is TouchFreeDragEvent)
            {
                Vector2 delta = (e as TouchFreeDragEvent).Delta;
                CurrentLevel.Camera.StepHorizontal(-delta.X);
                CurrentLevel.Camera.StepVertical(-delta.Y);
                return;
            }

            if (e is MouseMovedEvent)
            {
                if (editing && InputManager.isMouseVisible()) CurrentLevel.HighlightTileAt(CurrentLevel.Camera.DeTransform(InputManager.getMousePos()));
                if (InputManager.isMouseDown(InputManager.MouseKey.LeftKey))
                {
                    dragging = true;
                    Vector2 offset = (e as MouseMovedEvent).Offset;
                    CurrentLevel.Camera.StepHorizontal(offset.X);
                    CurrentLevel.Camera.StepVertical(offset.Y);
                }
                return;
            }

            if (e is MouseScrollEvent)
            {
                CurrentLevel.Camera.Zoom((e as MouseScrollEvent).Value / (float)Screen.Height);
                return;
            }

            if (e is MouseUpEvent)
            {
                if (dragging) { dragging = false; return; }
                MouseUpEvent ev = (e as MouseUpEvent);
                Tile target = CurrentLevel.getTileAt(CurrentLevel.Camera.DeTransform(ev.Position));
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
                    if (obj != null && (obj.IsInteractable || editing))
                    {
                        verified = false; interacted = true;
                        obj.RotateCW(editing);
                        moves++;
                        UpdateCounter();
                    }
                }
            }
        }

        public void OnActivated(params object[] args)
        {
            if (args.Length > 1)
                loadLevel(args[0].ToString(), (PackageType)args[1]);
            SetupHud();
        }
    }
}
