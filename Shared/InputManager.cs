using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input.Touch;

namespace Inlumino_SHARED
{
    static class InputManager
    {
        internal delegate void KeyDownEventHandler(Keys k);
        internal static event KeyDownEventHandler KeyDown;
        internal delegate void KeyUpEventHandler(Keys k);
        internal static event KeyUpEventHandler KeyUp;
        internal delegate void MouseDownEventHandler(MouseKey k, Vector2 position);
        internal static event MouseDownEventHandler MouseDown;
        internal delegate void MouseUpEventHandler(MouseKey k, Vector2 position);
        internal static event MouseUpEventHandler MouseUp;
        internal delegate void ScrollEventHandler(int value);
        internal static event ScrollEventHandler Scrolled;
        internal delegate void MouseMovedEventHandler(Vector2 position, Vector2 offset);
        internal static event MouseMovedEventHandler MouseMoved;
        internal delegate void TouchTapEventHandler(Vector2 position);
        internal static event TouchTapEventHandler Tapped;
        internal delegate void DragEventHandler(Vector2 delta, Vector2 position);
        internal static event DragEventHandler Dragged;
        internal delegate void PinchEventHandler(float delta);
        internal static event PinchEventHandler Pinched;
        internal delegate void DragCompleteHandler(Vector2 position);
        internal static event DragCompleteHandler DragComplete;
        internal delegate void FingerDownHandler(Vector2 position);
        internal static event FingerDownHandler FingerDown;
        internal delegate void FingerOffHandler(Vector2 position);
        internal static event FingerOffHandler FingerOff;
        internal delegate void AllFingersOffHandler();
        internal static event AllFingersOffHandler AllFingersOff;
        private static Func<Vector2, Vector2> adjust = (v) =>
         {
             return
#if WP81
            !Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().IsFullScreen ? new Vector2(
                v.X*Screen.XAdjustRatio, v.Y * Screen.YAdjustRatio
                )
            :
#endif 
            v;
         };
        internal static void init()
        {
            TouchPanel.EnabledGestures = GestureType.Tap | GestureType.FreeDrag | GestureType.Pinch | GestureType.PinchComplete;
#if WINDOWS_UAP
            ms = Mouse.GetState();
            lmx = ms.X;
            lmy = ms.Y;
            lwv = ms.ScrollWheelValue;
            WatchKeys(Keys.GetValues(typeof(Keys)).Cast<Keys>().ToArray());
#endif
        }

        static List<Keys> watchlist = new List<Keys>();
        static Keys[] ltPressed = { };
        static bool[] mousepressed = new bool[] { false, false, false };
        static int lwv, lmx, lmy;
        static GestureSample lpg;
        static bool pinching = false;
        static MouseState ms;
        static KeyboardState ks;
        static bool fo = false;
        static public void Update(GameTime time)
        {
            ms = Mouse.GetState();

            ks = Keyboard.GetState();

            TouchCollection ts = TouchPanel.GetState();

            var gesture = default(GestureSample);

            foreach (TouchLocation tl in ts)
            {
                fo = true;
                if (tl.State == TouchLocationState.Pressed && FingerDown != null) FingerDown(adjust(tl.Position));
                if (tl.State == TouchLocationState.Released && FingerOff != null) FingerOff(adjust(tl.Position));
            }
            if (fo && ts.Count == 0 && AllFingersOff != null)
            {
                fo = false;
                AllFingersOff();
            }
            while (TouchPanel.IsGestureAvailable)
            {
                gesture = TouchPanel.ReadGesture();
                if (gesture.GestureType == GestureType.Tap)
                    Tapped(adjust(gesture.Position));
                else if (gesture.GestureType == GestureType.FreeDrag)
                    Dragged(gesture.Delta, adjust(gesture.Position));
                else if (gesture.GestureType == GestureType.Pinch)
                {
                    if (!pinching) { pinching = true; lpg = gesture; }
                    else
                    {
                        float ld = (lpg.Position - lpg.Position2).Length();
                        float cd = (gesture.Position - gesture.Position2).Length();
                        Pinched(1 - cd / ld);
                        lpg = gesture;
                    }
                }
                else if (gesture.GestureType == GestureType.PinchComplete)
                    pinching = false;
            }
            if (ms.X != lmx || ms.Y != lmy)
            {
                if (MouseMoved != null)
                    MouseMoved(adjust(ms.Position.ToVector2()), new Vector2(lmx - ms.X, lmy - ms.Y));
            }
            if (ms.ScrollWheelValue != lwv)
            {
                if (Scrolled != null)
                    Scrolled(lwv - ms.ScrollWheelValue);
            }
            if (ms.LeftButton == ButtonState.Pressed && !mousepressed[0])
            {
                if (MouseDown != null)
                    MouseDown(MouseKey.LeftKey, adjust(ms.Position.ToVector2()));
            }
            if (ms.LeftButton == ButtonState.Released && mousepressed[0])
            {
                if (MouseUp != null)
                    MouseUp(MouseKey.LeftKey, adjust(ms.Position.ToVector2()));
            }
            if (ms.MiddleButton == ButtonState.Pressed && !mousepressed[1])
            {
                if (MouseDown != null)
                    MouseDown(MouseKey.MiddleKey, adjust(ms.Position.ToVector2()));
            }
            if (ms.MiddleButton == ButtonState.Released && mousepressed[1])
            {
                if (MouseUp != null)
                    MouseUp(MouseKey.MiddleKey, adjust(ms.Position.ToVector2()));
            }
            if (ms.RightButton == ButtonState.Pressed && !mousepressed[2])
            {
                if (MouseDown != null)
                    MouseDown(MouseKey.RightKey, adjust(ms.Position.ToVector2()));
            }
            if (ms.RightButton == ButtonState.Released && mousepressed[2])
            {
                if (MouseUp != null)
                    MouseUp(MouseKey.RightKey, adjust(ms.Position.ToVector2()));
            }
            foreach (Keys k in watchlist)
            {
                if (ks.IsKeyDown(k) && !ltPressed.Contains(k))
                {
                    if (KeyDown != null)
                        KeyDown(k);
                }
            }
            foreach (Keys k in ltPressed)
            {
                if (ks.IsKeyUp(k))
                {
                    if (KeyUp != null)
                        KeyUp(k);
                }
            }
            lmx = ms.X;
            lmy = ms.Y;
            lwv = ms.ScrollWheelValue;
            ltPressed = ks.GetPressedKeys();
            mousepressed[0] = ms.LeftButton == ButtonState.Pressed;
            mousepressed[1] = ms.MiddleButton == ButtonState.Pressed;
            mousepressed[2] = ms.RightButton == ButtonState.Pressed;
        }

        static internal void WatchKeys(Keys[] keys)
        {
            watchlist.AddRange(keys);
        }
        static internal void WatchKey(Keys key)
        {
            if (!watchlist.Contains(key)) watchlist.Add(key);
        }
        static internal void StopWatchingdKey(Keys key)
        {
            if (watchlist.Contains(key)) watchlist.Remove(key);
        }

        static internal bool isKeyDown(Keys k)
        {
            return ks.IsKeyDown(k);
        }

        static internal bool isMouseVisible()
        {
            return Manager.Parent.IsMouseVisible;
        }

        static internal bool isMouseDown(MouseKey button)
        {
            return mousepressed[(int)button];
        }

        internal enum MouseKey
        {
            LeftKey = 0, MiddleKey = 1, RightKey = 2
        }

        internal static Vector2 getMousePos()
        {
            return adjust(ms.Position.ToVector2());
        }

    }
}
