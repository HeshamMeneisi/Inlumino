using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

namespace Inlumino_SHARED
{
    class SmartContentManager : ContentManager
    {
        public SmartContentManager(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }


        Dictionary<string, object> loadedAssets = new Dictionary<string, object>();
        List<IDisposable> disposableAssets = new List<IDisposable>();


        public override T Load<T>(string assetName)
        {
            if (loadedAssets.ContainsKey(assetName))
                return (T)loadedAssets[assetName];

            T asset = default(T); int t = 0;
        TryAgain:
            t++;
            try
            {
                asset = ReadAsset<T>(assetName, RecordDisposableAsset);
                loadedAssets.Add(assetName, asset);
            }
            catch { GC.Collect(); if (t < 10) goto TryAgain; }


            return asset;
        }
        public override void Unload()
        {
            foreach (IDisposable disposable in disposableAssets)
                disposable.Dispose();

            loadedAssets.Clear();
            disposableAssets.Clear();
        }
        public void Unload(string assetname)
        {
            if (!loadedAssets.ContainsKey(assetname)) return;
            if (loadedAssets[assetname] is IDisposable)
            {
                ((IDisposable)loadedAssets[assetname]).Dispose();
                disposableAssets.Remove((IDisposable)loadedAssets[assetname]);
            }
            loadedAssets.Remove(assetname);
        }
        void RecordDisposableAsset(IDisposable disposable)
        {
            disposableAssets.Add(disposable);
        }
    }
}
