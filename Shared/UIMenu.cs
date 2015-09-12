using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Inlumino_SHARED
{
    class UIMenu : UIVisibleObject
    {
        protected List<UIVisibleObject> UIObjects;
        public UIMenu(int layer = 0, string id = "") : base(DataHandler.ObjectTextureMap[ObjectType.Invisible], layer, id)
        {
            UIObjects = new List<UIVisibleObject>();
        }

        public void Add(UIVisibleObject obj)
        {
            obj.Parent = this;
            for (int i = 0; i < UIObjects.Count; i++)
            {
                if (UIObjects[i].Layer > obj.Layer)
                {
                    UIObjects.Insert(i, obj);
                    return;
                }
            }
            UIObjects.Add(obj);
        }

        public void Remove(UIVisibleObject obj)
        {
            UIObjects.Remove(obj);
            obj.Parent = null;
        }

        public UIObject Find(string id)
        {
            foreach (UIObject obj in UIObjects)
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
            foreach (UIVisibleObject obj in UIObjects) obj.setSizeRelative(v, mode);
        }

        public override void HandleEvent(WorldEvent e)
        {
            if (!visible) return;
            base.HandleEvent(e);
            foreach (UIObject obj in UIObjects)
                obj.HandleEvent(e);
            base.HandleEvent(e);
        }

        public List<UIVisibleObject> Objects
        {
            get { return UIObjects; }
        }

        public override void Update(GameTime time)
        {
            if (!visible) return;
            foreach (UIObject obj in UIObjects)
                obj.Update(time);
        }

        public override void Draw(SpriteBatch batch, Camera cam = null)
        {
            if (!visible)
                return;
            List<UIVisibleObject>.Enumerator e = UIObjects.GetEnumerator();
            while (e.MoveNext())
                if (e.Current is UIVisibleObject) (e.Current as UIVisibleObject).Draw(batch);
        }

        public override void Clear()
        {
            base.Clear();
            foreach (UIObject obj in UIObjects)
                obj.Clear();
        }
        public void ArrangeInForm(Orientation mode, float maxwidth = -1, float maxheight = -1)
        {
            maxwidth = maxwidth > 0 ? maxwidth : Screen.Width - GlobalPosition.X;
            maxheight = maxheight > 0 ? maxheight : Screen.Height - GlobalPosition.Y;
            float x = 0, y = 0, w = 0, h = 0;
            if (mode == Orientation.Landscape)
                foreach (UIVisibleObject obj in UIObjects)
                {
                    if (x + obj.Width > maxwidth) { x = 0; y += obj.Height; }
                    obj.Position = new Vector2(x, y);
                    x += obj.Width;
                    w = Math.Max(w, x);
                    h = Math.Max(h, obj.Height);
                }
            else
                foreach (UIVisibleObject obj in UIObjects)
                {
                    if (y + obj.Height > maxheight) { y = 0; x += obj.Width; }
                    obj.Position = new Vector2(x, y);
                    y += obj.Height;
                    h = Math.Max(h, y);
                    w = Math.Max(w, obj.Width);
                }
            size = new Vector2(w, h);
        }
    }
}
