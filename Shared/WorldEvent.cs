using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Inlumino_SHARED
{
    public class WorldEvent
    {
        /// <summary>
        /// For parent/child communication.
        /// </summary>
        public bool Handled { get; set; }
    }

    class MouseDownEvent : WorldEvent
    {
        private InputManager.MouseKey k;
        private Point pos;

        public MouseDownEvent(InputManager.MouseKey k, Point pos)
        {
            this.k = k;
            this.pos = pos;
        }

        public Point Position { get { return pos; } }
        public InputManager.MouseKey Key { get { return k; } }
    }
    class MouseUpEvent : WorldEvent
    {
        private InputManager.MouseKey k;
        private Point pos;

        public MouseUpEvent(InputManager.MouseKey k, Point pos)
        {
            this.k = k;
            this.pos = pos;
        }

        public Point Position { get { return pos; } }

        public InputManager.MouseKey Key { get { return k; } }
    }
    class MouseMovedEvent : WorldEvent
    {
        private Point position;
        private Point offset;

        public MouseMovedEvent(Point position, Point offset)
        {
            this.position = position;
            this.offset = offset;
        }

        public Point Position
        { get { return position; } }
        public Point Offset
        { get { return offset; } }
    }
    class MouseScrollEvent : WorldEvent
    {
        private int value;

        public MouseScrollEvent(int value)
        {
            this.value = value;
        }
        public int Value
        { get { return value; } }
    }
    class KeyDownEvent : WorldEvent
    {
        private Keys k;

        public KeyDownEvent(Keys k)
        {
            this.k = k;
        }
        public Keys Key { get { return k; } }
    }
    class KeyUpEvent : WorldEvent
    {
        private Keys k;

        public KeyUpEvent(Keys k)
        {
            this.k = k;
        }
        public Keys Key { get { return k; } }
    }
    class TouchTapEvent : WorldEvent
    {
        private Vector2 position;

        public TouchTapEvent(Vector2 position)
        {
            this.position = position;
        }
        public Vector2 Position { get { return position; } }
    }
    class TouchFreeDragEvent : WorldEvent
    {
        public TouchFreeDragEvent(Vector2 delta, Vector2 position)
        {
            Delta = delta;
            Postion = position;
        }
        public Vector2 Delta { get; private set; }

        public Vector2 Postion { get; private set; }
    }
    class TouchDragCompleteEvent:WorldEvent
    {
        public Vector2 Postion { get; private set; }
        public TouchDragCompleteEvent(Vector2 position)
        {        
            Postion = position;
        }
    }
    class TouchPinchEvent : WorldEvent
    {
        private float delta;

        public TouchPinchEvent(float delta)
        {
            this.delta = delta;
        }
        public float Delta { get { return delta; } }
    }
    class DisplaySizeChangedEvent : WorldEvent
    {
        Vector2 newsize;

        public DisplaySizeChangedEvent(Vector2 newsize)
        { this.newsize = newsize; }

        public Vector2 NewSize { get { return newsize; } }
    }
    class TouchAllFingersOffEvent:WorldEvent
    {

    }
    class OrientationChangedEvent : WorldEvent
    {
        private DisplayOrientation currentOrientation;

        public OrientationChangedEvent(DisplayOrientation currentOrientation)
        {
            this.currentOrientation = currentOrientation;
        }

        public DisplayOrientation CurrentOrientation { get { return currentOrientation; } }
    }
}