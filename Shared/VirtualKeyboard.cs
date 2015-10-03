using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Inlumino_SHARED
{
    static class VirtualKeyboard
    {
        internal delegate void KeyPressedEventHandler(Keys key);
        internal static event KeyPressedEventHandler KeyPressed;
        internal static bool SimulateKeyDownToManager = true;
        internal static bool ShiftEnabled = true;
        internal static bool SymbolsEnabled = true;
        static UIMenu keyboard;
        static Func<Keys, bool> filterfunc;
        static UITextField target;
        static Vector2 priorpos;
        static int priolayer = 0;
        static internal Action<bool> VisibilityChanged;
        internal static void Show(UITextField targetfield = null, Func<Keys, bool> filter = null, float x = 0, float y = 0)
        {
            if (target == targetfield) return;
            if (target != null) EndInput();
            filterfunc = filter;
            target = targetfield;
            priolayer = target.Layer;
            target.Layer = int.MaxValue;
            priorpos = target.Position;
            SimulateKeyDownToManager = target == null;
            SetupHud();
            OnVisibilityChanged(true);
        }        
        private static void OnVisibilityChanged(bool v)
        {
            if (VisibilityChanged != null) VisibilityChanged(v);
        }

        private static void SetupHud()
        {
            keyboard = new UIMenu("VK");
            keyboard.Add(getcontent());
            keyboard.ArrangeInForm(Orientation.Portrait);
            keyboard.Position = new Vector2(0, Screen.Height - keyboard.Height);
            changingtarget = true;
            target.GlobalPosition = new Vector2(0, keyboard.Position.Y - target.Height);
            changingtarget = false;
        }
        static bool changingtarget = false;
        internal static void NotifyPosChanged()
        {
            if (target != null && !changingtarget)
            {
                priorpos = target.Position;
                changingtarget = true;
                target.GlobalPosition = new Vector2(0, keyboard.GlobalPosition.Y - target.Height);
                changingtarget = false;
            }
        }
        static Keys[][] defkeys = new Keys[][] {

        new Keys[] { Keys.Q, Keys.W, Keys.E, Keys.R, Keys.T, Keys.Y, Keys.U, Keys.I, Keys.O, Keys.P },
        new Keys[] { Keys.A, Keys.S, Keys.D, Keys.F, Keys.G, Keys.H, Keys.J, Keys.K, Keys.L },
        new Keys[] { Keys.Z, Keys.X, Keys.C, Keys.V, Keys.B, Keys.N, Keys.M },
        new Keys[] { Keys.Back, Keys.LeftShift,Keys.Space, Keys.None, Keys.Enter }
        };
        static Keys[][] defsymb = new Keys[][]
        {
            new Keys[] { Keys.D1,Keys.D2,Keys.D3,Keys.D4,Keys.D5,Keys.D6,Keys.D7,Keys.D8,Keys.D9,Keys.D0 },
            new Keys[] { Keys.OemTilde,Keys.OemSemicolon,Keys.OemQuotes,Keys.OemPipe,Keys.OemMinus,Keys.OemPlus },
            new Keys[] {Keys.Back,Keys.LeftShift, Keys.OemComma, Keys.OemPeriod, Keys.OemQuestion,Keys.None,Keys.Enter }

        };
        static int state = 0;
        private static IEnumerable<UIGrid> getcontent()
        {
            Keys[][] pool = state == 0 ? defkeys : defsymb;
            foreach (Keys[] row in pool)
            {
                List<UICell> keys = new List<UICell>();
                foreach (Keys k in row)
                    if (filterfunc == null || filterfunc(k))
                    {
                        string t = "";
                        if (k == Keys.LeftShift) t = state == 0 ? "Aa" : "0#";
                        else if (k == Keys.Enter) t = "Go";
                        else if (k == Keys.None) t = state == 0 ? "1?" : "AZ";
                        else if (k == Keys.Back) t = "<-";
                        else if (CommonData.KeyCharMap.ContainsKey(k)) t = CommonData.KeyCharMap[k][Low ? 0 : 1].ToString();
                        else t = k.ToString();
                        UICell key = new UICell(DataHandler.UIObjectsTextureMap[UIObjectType.Cell], k, t, Color.White);
                        key.Pressed += keypressed;
                        keys.Add(key);
                    }
                float h = (Screen.Height - (target == null ? 0 : target.Height)) / pool.Length;
                UIGrid rhud = new UIGrid(keys, Orientation.Landscape, Screen.Width, h, row.Length);
                rhud.ShowEntireRowCol();
                rhud.TrimGridToVisible();
                rhud.TrimAllToGrid();
                //rhud.LockCamera = true;
                yield return rhud;
            }
        }
        internal static bool Low = true;

        internal static RectangleF BoundingBox
        {
            get { return keyboard == null ? null : keyboard.BoundingBox; }
        }

        public static bool IsVisible { get { return keyboard != null; } }

        private static void keypressed(UIButton sender)
        {
            Keys k = (Keys)(sender as UICell).Tag;
            switch (k)
            {
                default:
                    OnKeyPressed(k); break;
                case Keys.LeftShift:
                case Keys.RightShift:
                    Low = !Low; SetupHud(); break;
                case Keys.None: state++; state %= 2; SetupHud(); break;
                case Keys.Enter: EndInput(); break;

            }
        }

        private static void EndInput()
        {
            if (target != null) { target.NotifyVKExit(); target.Position = priorpos; target.Layer = priolayer; target = null; }
            keyboard = null;
            OnVisibilityChanged(false);
        }

        private static void OnKeyPressed(Keys k)
        {
            if (KeyPressed != null) KeyPressed(k);
            if (target != null)
                if (CommonData.KeyCharMap.ContainsKey(k)) target.Input(CommonData.KeyCharMap[k][Low ? 0 : 1]);
                else Manager.HandleEvent(new KeyDownEvent(k));
            else if (SimulateKeyDownToManager) Manager.HandleEvent(new KeyDownEvent(k));
            target.ScaleToText();
        }

        internal static void Draw(SpriteBatch batch)
        {
            if (keyboard != null)
                keyboard.Draw(batch);
        }
        internal static void Update(GameTime time)
        {
            if (keyboard != null)
                keyboard.Update(time);
        }
        internal static void HandleEvent(WorldEvent e)
        {
            if (keyboard != null)
            {
                keyboard.HandleEvent(e);
                if (e is DisplaySizeChangedEvent || e is OrientationChangedEvent)
                    SetupHud();
            }
        }

        internal static void Hide()
        {
            EndInput();
        }
    }
}