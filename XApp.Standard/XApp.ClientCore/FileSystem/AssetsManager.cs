using System;
using Newtonsoft.Json;
using System.Collections.Generic;

using Vardirsoft.Shared.Helpers;

using Vardirsoft.XApp.IoC;
using Vardirsoft.XApp.API;
using Vardirsoft.XApp.Visuals;
using Vardirsoft.XApp.Logging;
using Vardirsoft.XApp.Application;
using Vardirsoft.XApp.Application.Configuration;

namespace Vardirsoft.XApp.FileSystem
{
    public class AssetsManager
    {
        private AppKernel appKernel;

        public GlyphPool GlyphPool { get; }

        public AssetsManager()
        {
            GlyphPool = new GlyphPool();
            appKernel = AppKernel.Instance;
        }

        public void LoadAssets(string source)
        {
            var logger = IoCContainer.Resolve<ILogger>();
            logger.PushDebug(null, $"Loading assets from {source}");

            try
            {
                LoadGlyphs(source);
                //TODO: add another types of assets to load
            }
            catch (Exception ex)
            {
                logger.PushFatal(ex.Message);
            }

            logger.PushDebug(null, "Assets loading finished");
        }

        private void LoadGlyphs(string source)
        {
            var fileManager = IoCContainer.Resolve<IFileManager>();
            var configurations = IoCContainer.Resolve<ConfigurationManager>();
            var path = buildPath((string)configurations["theme"]) ?? buildPath("default");

            if (path.HasValue())
            {
                var glyphsData = fileManager.ReadOrCreate(path, "{}");
                foreach (var kvp in JsonConvert.DeserializeObject<Dictionary<string, string>>(glyphsData))
                {
                    GlyphPool.AddOrUpdate(kvp.Key, Glyph.From(kvp.Value));
                }
            }
            else
            {
                IoCContainer.Resolve<ILogger>().PushWarning($"Can not load glyphs from {source}");
            }

            string buildPath(string theme) => fileManager.Combine(source, "glyphs", theme, "config.json");
        }
    }
}