using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Inlumino_SHARED
{
    class Screen
    {
        private static GameWindow window;
        private static GraphicsDeviceManager device;
        private static bool isVirtual = false;
        private static Vector2 virtualbounds;
        // This is deprecated
        //public static int Width{get{return GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;}}
        //public static int Height { get { return GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height; } }
        public static float Width { get { return isVirtual ? virtualbounds.X : window.ClientBounds.Width; } }
        public static float Height { get { return isVirtual ? virtualbounds.Y : window.ClientBounds.Height; } }

        public static void SetUp(GameWindow gamewindow, GraphicsDeviceManager devicemanager)
        {
            device = devicemanager;
            window = gamewindow;
        }
        internal static void SetFullScreen(bool state)
        {
            if (device.IsFullScreen != state)
                device.ToggleFullScreen();
        }

        public static void MakeVirtual(Vector2 virtualbounds)
        {
            Screen.virtualbounds = virtualbounds;
            isVirtual = true;
            Manager.HandleEvent(new DisplaySizeChangedEvent(virtualbounds));
        }

        public static void MakeReal()
        {
            isVirtual = false;
            Manager.HandleEvent(new DisplaySizeChangedEvent(new Vector2(Width, Height)));
        }
    }
}
