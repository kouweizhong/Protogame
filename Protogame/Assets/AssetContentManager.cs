using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;

namespace Protogame
{
    public class AssetContentManager : ContentManager, IAssetContentManager
    {
        private Dictionary<string, Stream> m_MemoryStreams = new Dictionary<string, Stream>();
    
        public AssetContentManager(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public override T Load<T>(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                throw new ArgumentNullException("assetName");
            }

            T result = default(T);

            // Check for a previously loaded asset first
            object asset = null;
            if (LoadedAssets.TryGetValue(assetName, out asset))
            {
                if (asset is T)
                {
                    return (T)asset;
                }
            }

            // Load the asset.
            result = ReadAsset<T>(assetName, x => {});

            LoadedAssets[assetName] = result;
            return result;
        }
        
        public void Purge(string assetName)
        {
            if (!this.LoadedAssets.ContainsKey(assetName))
                return;
            var asset = this.LoadedAssets[assetName];
            if (asset is IDisposable)
                (asset as IDisposable).Dispose();
            this.LoadedAssets.Remove(assetName);
        }
    
        public void SetStream(string assetName, Stream stream)
        {
            this.m_MemoryStreams[assetName] = stream;
        }
        
        public void UnsetStream(string assetName)
        {
            this.m_MemoryStreams[assetName] = null;
        }
    
        protected override Stream OpenStream(string assetName)
        {
            return this.m_MemoryStreams[assetName];
        }
    }
}
