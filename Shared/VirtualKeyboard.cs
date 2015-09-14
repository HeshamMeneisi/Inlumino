using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Inlumino_SHARED
{
    static class VirtualKeyboard
    {
        public delegate void KeyPressedEventHandler(Keys key);
        public static event KeyPressedEventHandler KeyPressed;
        static UIHud hud;
        static Func<Keys, bool> filterfunc;
        public static void Show(Orientation mode, float minkeydim, float width, float height, Func<Keys, bool> filter = null, float x = 0, float y = 0)
        {
            filterfunc = filter;
            hud = new UIHud(getcontent(), mode, minkeydim, minkeydim, width, height);
            hud.Position = new Vector2(x, y);
            hud.SnapCameraToCells = false;
            hud.Setup();
        }
        static UICell[] allkeys = null;
        private static UICell[] getcontent()
        {
            List<UICell> keys = new List<UICell>();
            foreach (Keys k in Keys.GetValues(typeof(Keys)))
            {
                if (filterfunc == null || filterfunc(k))
                {
                    UICell key = new UICell(DataHandler.UIObjectsTextureMap[UIObjectType.Cell], k, k.ToString().Substring(0, Math.Min(2, k.ToString().Length)), Color.White);
                    key.Pressed += keypressed;
                    keys.Add(key);
                }
            }
            return allkeys = keys.ToArray();
        }
        public static bool Low = true;

        public static RectangleF BoundingBox
        {
            get { return hud.BoundingBox; }
        }

        private static void keypressed(UIButton sender)
        {
            Keys k = (Keys)(sender as UICell).Tag;
            if (k == Keys.LeftShift || k == Keys.RightShift) Low = !Low;
            if (KeyPressed != null) KeyPressed(k);
        }

        public static void Draw(SpriteBatch batch)
        {
            hud.Draw(batch);
        }
        public static void Update(GameTime time)
        {
            hud.Update(time);
        }
        public static void HandleEvent(WorldEvent e)
        {
            hud.HandleEvent(e);
        }

        internal static void Show(Orientation landscape, object p1, float width, float height, Func<Keys, bool> p2)
        {
            throw new NotImplementedException();
        }
    }
}