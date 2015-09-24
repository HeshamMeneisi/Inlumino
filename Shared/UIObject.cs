using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Inlumino_SHARED
{
    abstract class UIObject
    {
        protected UIObject parent;
        protected Vector2 position;
        protected int layer;
        protected string id;
        protected bool visible;
        protected List<UIObject> siblings = new List<UIObject>(); // used with huds to enable camera transformation
        protected List<UIObject> children = new List<UIObject>();
        internal List<UIObject> Siblings { get { return siblings; } }
        internal UIObject(string id = "", int layer = 0)
        {
            this.layer = layer;
            this.id = id;
            this.position = Vector2.Zero;
            this.visible = true;
        }
        internal void AttachSibling(UIObject sibling)
        {
            sibling.Parent = Parent;
            siblings.Add(sibling);
        }
        internal void AttachChild(UIObject child)
        {
            child.Parent = this;
            children.Add(child);
        }
        internal virtual void Update(GameTime time)
        {

        }

        internal virtual void Clear()
        {
            visible = true;
        }

        internal virtual Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        internal virtual Vector2 GlobalPosition
        {
            get
            {                
                if (parent != null)
                    return parent.GlobalPosition + this.Position;
                else
                    return this.Position;
            }
            set
            {
                if (parent == null)
                    Position = value;
                else
                {
                    Position = value - parent.GlobalPosition;
                }
            }
        }

        internal UIObject Root
        {
            get
            {
                if (parent != null)
                    return parent.Root;
                else
                    return this;
            }
        }

        internal UIObject Menu
        {
            get
            {
                return Root as UIMenu;
            }
        }

        internal virtual void HandleEvent(WorldEvent e)
        {
                        
        }

        internal virtual int Layer
        {
            get { return layer; }
            set { layer = value; }
        }

        internal virtual UIObject Parent
        {
            get { return parent; }
            set { parent = value;
                foreach (UIObject obj in siblings) obj.parent = parent;
            }
        }

        internal string ID
        {
            get { return id; }
        }

        internal bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        internal virtual RectangleF BoundingBox
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
    ResetButton,
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
    Log,
    Next,
    Lock,
    Border,
    TryAgain,
    Frame,
    Ropes,
    TopLog,
    ShareBtn
}
