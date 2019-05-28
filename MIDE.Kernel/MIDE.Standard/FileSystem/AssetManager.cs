using System;
using Newtonsoft.Json;
using MIDE.API.Visuals;
using MIDE.Application;
using System.Collections.Generic;
using MIDE.Application.Configuration;

namespace MIDE.FileSystem
{
    public class AssetManager
    {
        private static AssetManager instance;
        public static AssetManager Instance => instance ?? (instance = new AssetManager());

        private AppKernel appKernel;
        private FileManager fileManager;
        private ConfigurationManager configurations;

        public GlyphPool GlyphPool { get; }

        private AssetManager()
        {
            GlyphPool = new GlyphPool();
            appKernel = AppKernel.Instance;
            fileManager = FileManager.Instance;
            configurations = ConfigurationManager.Instance;
        }

        public void LoadAssets(string source)
        {
            appKernel.AppLogger.PushDebug(null, $"Loading assets from {source}");
            try
            {
                LoadGlyphs(source);
                //TODO: add another types of assets to load
            }
            catch (Exception ex)
            {
                appKernel.AppLogger.PushFatal(ex.Message);
            }
            appKernel.AppLogger.PushDebug(null, "Assets loading finished");
        }

        private void LoadGlyphs(string source)
        {
            string path = buildPath((string)configurations["theme"]) ?? buildPath("default");
            if (path == null)
            {
                appKernel.AppLogger.PushWarning($"Can not load glyphs from {source}");
                return;
            }
            string glyphsData = fileManager.ReadOrCreate(path, "{}");
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(glyphsData);
            foreach (var kvp in dict)
            {
                GlyphPool.AddOrUpdate(kvp.Key, Glyph.From(kvp.Value));
            }
            string buildPath(string theme) => fileManager.Combine(source, "glyphs", theme, "config.json");
        }
    }
}