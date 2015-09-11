using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Inlumino_SHARED
{
    abstract class UIObject
    {
        protected UIObject parent;
        protected Vector2 position;
        protected int layer;
        protected string id;
        protected bool visible;

        public UIObject(int layer = 0, string id = "")
        {
            this.layer = layer;
            this.id = id;
            this.position = Vector2.Zero;
            this.visible = true;
        }

        public virtual void Update(GameTime time)
        {

        }

        public virtual void Draw(SpriteBatch batch, Camera cam = null) // No need for camera, gui is always visible. Draw ontop.
        {
        }

        public virtual void Clear()
        {
            visible = true;
        }

        public virtual Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public virtual Vector2 GlobalPosition
        {
            get
            {                
                if (parent != null)
                    return parent.GlobalPosition + this.Position;
                else
                    return this.Position;
            }
        }

        public UIObject Root
        {
            get
            {
                if (parent != null)
                    return parent.Root;
                else
                    return this;
            }
        }

        public UIObject Menu
        {
            get
            {
                return Root as UIMenu;
            }
        }

        public virtual void HandleEvent(WorldEvent e)
        {
                        
        }

        public virtual int Layer
        {
            get { return layer; }
            set { layer = value; }
        }

        public virtual UIObject Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public string ID
        {
            get { return id; }
        }

        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        public virtual RectangleF BoundingBox
        {
            get
            {
                return new RectangleF(GlobalPosition.X, GlobalPosition.Y, 0, 0);
            }
        }
    }
}
enum UIObjectType
{
    PlayBtn,EditModeBtn,OptionsBtn,
    Cell,
    MenuButton,
    RestartButton,
    ToggleButton,
    SaveButton,
    LeftButton,
    RightButton,
    UpButton,
    DownButton,
    BackButton,
    MainUser,
    DeleteBtn,
    Star,
    Log
}
