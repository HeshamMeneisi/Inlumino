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
        //internal static int Width{get{return GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;}}
        //internal static int Height { get { return GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height; } }
        internal static float Width
        {
            get
            {
                return isVirtual ? virtualbounds.X :
#if WP81
                window.ClientBounds.Width * ((window.CurrentOrientation == DisplayOrientation.LandscapeLeft || window.CurrentOrientation == DisplayOrientation.LandscapeRight) && !Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().IsFullScreen ? 0.94f : 1);
#else
                window.ClientBounds.Width;
#endif
            }
        }
        internal static float Height
        {
            get
            {
                return isVirtual ? virtualbounds.Y :
#if WP81
                    window.ClientBounds.Height * ((window.CurrentOrientation == DisplayOrientation.Portrait || window.CurrentOrientation == DisplayOrientation.PortraitDown) && !Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().IsFullScreen ? 0.94f : 1);
#else
                    window.ClientBounds.Height;
#endif
            }
        }
        // window.CurrentOrientation also works, but this is more literal representation of the concept.
        internal static Orientation Mode { get { return Width > Height ? Orientation.Landscape : Orientation.Portrait; } }

        internal static float BigDim { get { return MathHelper.Max(Width, Height); } }
        internal static float SmallDim { get { return MathHelper.Min(Width, Height); } }

        internal static float YAdjustRatio
        {
            get
            {
                return
                    Height / window.ClientBounds.Height;
            }
        }
        internal static float XAdjustRatio
        {
            get
            {
                return
                    Width / window.ClientBounds.Width;
            }
        }
        internal static void SetUp(GameWindow gamewindow, GraphicsDeviceManager devicemanager)
        {
            device = devicemanager;
            window = gamewindow;
        }
        internal static void SetFullScreen(bool state)
        {
            if (device.IsFullScreen != state)
                device.ToggleFullScreen();
        }

        internal static void MakeVirtual(Vector2 virtualbounds)
        {
            Screen.virtualbounds = virtualbounds;
            isVirtual = true;
            Manager.HandleEvent(new DisplaySizeChangedEvent(virtualbounds));
        }

        internal static void MakeReal()
        {
            isVirtual = false;
            Manager.HandleEvent(new DisplaySizeChangedEvent(new Vector2(Width, Height)));
        }
    }
}
