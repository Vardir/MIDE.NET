using System;
using MIDE.IoC;
using MIDE.API;
using MIDE.Visuals;
using MIDE.Helpers;
using Newtonsoft.Json;
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
        private ConfigurationManager configurations;

        public GlyphPool GlyphPool { get; }

        private AssetManager()
        {
            GlyphPool = new GlyphPool();
            appKernel = AppKernel.Instance;
            configurations = IoCContainer.Resolve<ConfigurationManager>();
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
            var fileManager = IoCContainer.Resolve<IFileManager>();
            string path = buildPath((string)configurations["theme"]) ?? buildPath("default");

            if (path.HasValue())
            {
                string glyphsData = fileManager.ReadOrCreate(path, "{}");
                foreach (var kvp in JsonConvert.DeserializeObject<Dictionary<string, string>>(glyphsData))
                {
                    GlyphPool.AddOrUpdate(kvp.Key, Glyph.From(kvp.Value));
                }
            }
            else
            {
                appKernel.AppLogger.PushWarning($"Can not load glyphs from {source}");
            }

            string buildPath(string theme) => fileManager.Combine(source, "glyphs", theme, "config.json");
        }
    }
}