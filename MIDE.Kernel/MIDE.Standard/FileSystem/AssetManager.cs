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
            fileManager = appKernel.FileManager;
            configurations = ConfigurationManager.Instance;
        }

        public void LoadAssets()
        {
            appKernel.AppLogger.PushDebug(null, "Loading assets");
            try
            {
                string path = fileManager.Combine(fileManager[FileManager.ASSETS],
                                                  "glyphs",
                                                  (string)configurations["theme"],
                                                  "config.json");
                string glyphsData = fileManager.ReadOrCreate(path, "{}");
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(glyphsData);
                foreach (var kvp in dict)
                {
                    GlyphPool.AddOrUpdate(kvp.Key, Glyph.From(kvp.Value));
                }
            }
            catch (Exception ex)
            {
                appKernel.AppLogger.PushFatal(ex.Message);
            }
            appKernel.AppLogger.PushDebug(null, "Application assets loaded");
        }
    }
}