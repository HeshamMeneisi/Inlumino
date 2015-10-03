using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Inlumino_SHARED
{
    class UIMenu : UIVisibleObject
    {
        new protected List<UIVisibleObject> children;

        internal override Vector2 Size { get { return new Vector2(Width, Height); } }

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
        internal UIMenu(string id = "", int layer = 0) : base(null, id, layer)
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
        internal void Add(IEnumerable<UIVisibleObject> objects)
        {
            foreach (UIVisibleObject obj in objects)
                Add(obj);
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
            foreach (var obj in children) obj.Draw(batch);
        }
        internal override void NotifyLayerShuffle()
        {
            base.NotifyLayerShuffle();
            children.Sort((a, b) => a.Layer.CompareTo(b.Layer));
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
            foreach (UIVisibleObject obj in children)
                obj.Position = Vector2.Zero;
            if (mode == Orientation.Landscape)
                foreach (UIVisibleObject obj in children)
                {
                    if (!obj.Visible) continue;
                    if (x + obj.Width > maxwidth) { x = 0; y += Height; }
                    obj.Position = new Vector2(x, y);
                    x += obj.Width;
                }
            else
                foreach (UIVisibleObject obj in children)
                {
                    if (!obj.Visible) continue;
                    if (y + obj.Height > maxheight) { y = 0; x += Width; }
                    obj.Position = new Vector2(x, y);
                    y += obj.Height;
                }
        }
    }
}
