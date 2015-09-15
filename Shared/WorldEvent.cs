using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Inlumino_SHARED
{
    internal class WorldEvent
    {
        /// <summary>
        /// For parent/child communication.
        /// </summary>
        internal bool Handled { get; set; }
    }

    class MouseDownEvent : WorldEvent
    {
        private InputManager.MouseKey k;
        private Point pos;

        internal MouseDownEvent(InputManager.MouseKey k, Point pos)
        {
            this.k = k;
            this.pos = pos;
        }

        internal Point Position { get { return pos; } }
        internal InputManager.MouseKey Key { get { return k; } }
    }
    class MouseUpEvent : WorldEvent
    {
        private InputManager.MouseKey k;
        private Point pos;

        internal MouseUpEvent(InputManager.MouseKey k, Point pos)
        {
            this.k = k;
            this.pos = pos;
        }

        internal Point Position { get { return pos; } }

        internal InputManager.MouseKey Key { get { return k; } }
    }
    class MouseMovedEvent : WorldEvent
    {
        private Point position;
        private Point offset;

        internal MouseMovedEvent(Point position, Point offset)
        {
            this.position = position;
            this.offset = offset;
        }

        internal Point Position
        { get { return position; } }
        internal Point Offset
        { get { return offset; } }
    }
    class MouseScrollEvent : WorldEvent
    {
        private int value;

        internal MouseScrollEvent(int value)
        {
            this.value = value;
        }
        internal int Value
        { get { return value; } }
    }
    class KeyDownEvent : WorldEvent
    {
        private Keys k;

        internal KeyDownEvent(Keys k)
        {
            this.k = k;
        }
        internal Keys Key { get { return k; } }
    }
    class KeyUpEvent : WorldEvent
    {
        private Keys k;

        internal KeyUpEvent(Keys k)
        {
            this.k = k;
        }
        internal Keys Key { get { return k; } }
    }
    class TouchTapEvent : WorldEvent
    {
        private Vector2 position;

        internal TouchTapEvent(Vector2 position)
        {
            this.position = position;
        }
        internal Vector2 Position { get { return position; } }
    }
    class TouchFreeDragEvent : WorldEvent
    {
        internal TouchFreeDragEvent(Vector2 delta, Vector2 position)
        {
            Delta = delta;
            Postion = position;
        }
        internal Vector2 Delta { get; private set; }

        internal Vector2 Postion { get; private set; }
    }
    class TouchDragCompleteEvent:WorldEvent
    {
        internal Vector2 Postion { get; private set; }
        internal TouchDragCompleteEvent(Vector2 position)
        {        
            Postion = position;
        }
    }
    class TouchPinchEvent : WorldEvent
    {
        private float delta;

        internal TouchPinchEvent(float delta)
        {
            this.delta = delta;
        }
        internal float Delta { get { return delta; } }
    }
    class DisplaySizeChangedEvent : WorldEvent
    {
        Vector2 newsize;

        internal DisplaySizeChangedEvent(Vector2 newsize)
        { this.newsize = newsize; }

        internal Vector2 NewSize { get { return newsize; } }
    }
    class TouchAllFingersOffEvent:WorldEvent
    {

    }
    class OrientationChangedEvent : WorldEvent
    {
        private DisplayOrientation currentOrientation;

        internal OrientationChangedEvent(DisplayOrientation currentOrientation)
        {
            this.currentOrientation = currentOrientation;
        }

        internal DisplayOrientation CurrentOrientation { get { return currentOrientation; } }
    }
}