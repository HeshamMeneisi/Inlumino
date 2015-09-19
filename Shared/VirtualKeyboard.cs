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
        static UIHud hud;
        static Func<Keys, bool> filterfunc;
        static UITextField target;
        static float mindim;
        static Vector2 priorpos;
        internal static void Show(float minkeydim, UITextField targetfield = null, Func<Keys, bool> filter = null, float x = 0, float y = 0)
        {
            if (target != null) EndInput();
            filterfunc = filter;
            mindim = minkeydim;
            target = targetfield;
            SimulateKeyDownToManager = target == null;
            if (target != null) priorpos = target.Position;
            target.Position = new Vector2(target.Position.X, 0);
            SetupHud();
        }

        private static void SetupHud()
        {
            hud = new UIHud(getcontent(), Orientation.Landscape, mindim, mindim, Screen.Width, Screen.Height - (target == null ? 0 : target.Height));
            if (target != null)
                hud.Position = new Vector2(0, target.BoundingBox.Bottom);
            hud.SnapCameraToCells = false;
            hud.Setup();
        }

        static UICell[] allkeys = null;
        static Keys[] defkeys = new Keys[] {

        Keys.Q, Keys.W, Keys.E, Keys.R, Keys.T, Keys.Y, Keys.U, Keys.I, Keys.O, Keys.P,
        Keys.A, Keys.S, Keys.D, Keys.F, Keys.G, Keys.H, Keys.J, Keys.K, Keys.L,
        Keys.Z, Keys.X, Keys.C, Keys.V, Keys.B, Keys.N, Keys.M,Keys.Back,Keys.LeftShift,Keys.None,Keys.Enter
        };
        static Keys[] defsymb = new Keys[]
        {
            Keys.OemTilde,Keys.D1,Keys.D2,Keys.D3,Keys.D4,Keys.D5,Keys.D6,Keys.D7,Keys.D8,Keys.D9,Keys.D0,Keys.OemMinus,Keys.OemPlus,
            Keys.OemSemicolon,Keys.OemQuotes,Keys.OemPipe,Keys.OemComma,Keys.OemPeriod,Keys.OemQuestion,
            Keys.Back,Keys.LeftShift,Keys.None,Keys.Enter

        };
        static int state = 0;
        private static UICell[] getcontent()
        {
            List<UICell> keys = new List<UICell>();
            Keys[] pool;
            if (filterfunc == null) pool = state == 0 ? defkeys : defsymb;
            else
                pool = (Keys[])Keys.GetValues(typeof(Keys));
            foreach (Keys k in pool)
            {
                if (filterfunc == null || filterfunc(k))
                {
                    string t = "";
                    if (k == Keys.LeftShift) t = "Aa";
                    else if (k == Keys.Enter) t = "GO";
                    else if (k == Keys.None) t = "#&";
                    else if (k == Keys.Back) t = "<-";
                    else if (CommonData.KeyCharMap.ContainsKey(k)) t = CommonData.KeyCharMap[k][Low ? 0 : 1].ToString();
                    else t = k.ToString();
                    UICell key = new UICell(DataHandler.UIObjectsTextureMap[UIObjectType.Cell], k, t, Color.White);
                    key.Pressed += keypressed;
                    keys.Add(key);
                }
            }
            return allkeys = keys.ToArray();
        }
        internal static bool Low = true;

        internal static RectangleF BoundingBox
        {
            get { return hud == null ? null : hud.BoundingBox; }
        }

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
            hud = null;
            if (target != null) { target.Position = priorpos; target.NotifyVKExit(); }
        }

        private static void OnKeyPressed(Keys k)
        {
            if (KeyPressed != null) KeyPressed(k);
            if (target != null)
                if (CommonData.KeyCharMap.ContainsKey(k)) target.Input(CommonData.KeyCharMap[k][Low ? 0 : 1]);
                else Manager.HandleEvent(new KeyDownEvent(k));
            else if (SimulateKeyDownToManager) Manager.HandleEvent(new KeyDownEvent(k));
        }

        internal static void Draw(SpriteBatch batch)
        {
            if (hud != null)
                hud.Draw(batch);
        }
        internal static void Update(GameTime time)
        {
            if (hud != null)
                hud.Update(time);
        }
        internal static void HandleEvent(WorldEvent e)
        {
            if (hud != null)
                hud.HandleEvent(e);
        }

        internal static void Hide()
        {
            EndInput();
        }
    }
}