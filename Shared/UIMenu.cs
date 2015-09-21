using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Inlumino_SHARED
{
    class UIMenu : UIVisibleObject
    {
        protected List<UIVisibleObject> children;

        internal override Vector2 Size
        {
            get
            {
                return new Vector2(Width, Height);
            }
        }
        internal override float Height
        {
            get
            {
                float miny = float.MaxValue, maxy = float.MinValue;
                foreach (UIVisibleObject obj in children)
                {
                    if (!obj.Visible) continue;
                    RectangleF temp = obj.BoundingBox;                    
                    miny = Math.Min(miny, temp.Top);
                    maxy = Math.Max(maxy, temp.Bottom);
                }
                return maxy - miny;                    
            }
        }
        internal override float Width
        {
            get
            {
                float minx = float.MaxValue, maxx = float.MinValue;
                foreach (UIVisibleObject obj in children)
                {
                    if (!obj.Visible) continue;
                    RectangleF temp = obj.BoundingBox;
                    minx = Math.Min(minx, temp.Left);
                    maxx = Math.Max(maxx, temp.Right);
                }
                return maxx - minx;
            }
        }
        internal UIMenu(int layer = 0, string id = "") : base(DataHandler.ObjectTextureMap[ObjectType.Invisible], layer, id)
        {
            children = new List<UIVisibleObject>();
        }

        internal void Add(UIVisibleObject obj)
        {
            obj.Parent = this;
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].Layer > obj.Layer)
                {
                    children.Insert(i, obj);
                    return;
                }
            }
            children.Add(obj);
        }

        internal void Remove(UIVisibleObject obj)
        {
            children.Remove(obj);
            obj.Parent = null;
        }

        internal UIObject Find(string id)
        {
            foreach (UIObject obj in children)
            {
                if (obj.ID == id)
                    return obj;
                if (obj is UIMenu)
                {
                    UIMenu menu = obj as UIMenu;
                    UIObject subobj = menu.Find(id);
                    if (subobj != null)
                        return subobj;
                }
            }
            return null;
        }

        internal void setAllSizeRelative(float v, Orientation mode)
        {
            foreach (UIVisibleObject obj in children) obj.setSizeRelative(v, mode);
        }

        internal override void HandleEvent(WorldEvent e)
        {
            if (!visible) return;
            base.HandleEvent(e);
            foreach (UIObject obj in children)
                obj.HandleEvent(e);
            base.HandleEvent(e);
        }

        internal List<UIVisibleObject> Objects
        {
            get { return children; }
        }

        internal override void Update(GameTime time)
        {
            if (!visible) return;
            foreach (UIObject obj in children)
                obj.Update(time);
        }

        internal override void Draw(SpriteBatch batch, Camera cam = null)
        {
            if (!visible)
                return;
            List<UIVisibleObject>.Enumerator e = children.GetEnumerator();
            while (e.MoveNext())
                if (e.Current is UIVisibleObject) (e.Current as UIVisibleObject).Draw(batch);
        }

        internal override void Clear()
        {
            base.Clear();
            foreach (UIObject obj in children)
                obj.Clear();
        }
        internal void ArrangeInForm(Orientation mode, float maxwidth = -1, float maxheight = -1)
        {
            maxwidth = maxwidth > 0 ? maxwidth : Screen.Width - GlobalPosition.X;
            maxheight = maxheight > 0 ? maxheight : Screen.Height - GlobalPosition.Y;
            float x = 0, y = 0;
            if (mode == Orientation.Landscape)
                foreach (UIVisibleObject obj in children)
                {
                    if (!obj.Visible) continue;
                    if (x + obj.Width > maxwidth) { x = 0; y += obj.Height; }
                    obj.Position = new Vector2(x, y);
                    x += obj.Width;
                }
            else
                foreach (UIVisibleObject obj in children)
                {
                    if (!obj.Visible) continue;
                    if (y + obj.Height > maxheight) { y = 0; x += obj.Width; }
                    obj.Position = new Vector2(x, y);
                    y += obj.Height;
                }
        }
    }
}
