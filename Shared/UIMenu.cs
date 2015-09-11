using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Inlumino_SHARED
{
    class UIMenu : UIObject
    {
        protected List<UIObject> UIObjects;

        public UIMenu(int layer = 0, string id = "") : base(layer, id)
    {
            UIObjects = new List<UIObject>();
        }

        public void Add(UIObject obj)
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

        public void Remove(UIObject obj)
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

        public override void HandleEvent(WorldEvent e)
        {
            foreach (UIObject obj in UIObjects)
                obj.HandleEvent(e);
            base.HandleEvent(e);
        }

        public List<UIObject> Objects
        {
            get { return UIObjects; }
        }

        public override void Update(GameTime time)
        {
            foreach (UIObject obj in UIObjects)
                obj.Update(time);
        }

        public override void Draw(SpriteBatch batch, Camera cam = null)
        {
            if (!visible)
                return;
            List<UIObject>.Enumerator e = UIObjects.GetEnumerator();
            while (e.MoveNext())
                e.Current.Draw(batch);
        }

        public override void Clear()
        {
            base.Clear();
            foreach (UIObject obj in UIObjects)
                obj.Clear();
        }
    }
}
