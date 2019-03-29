using MIDE.Helpers;
using System.Windows;
using Newtonsoft.Json;
using MIDE.Application;
using MIDE.API.Services;
using System.Windows.Media;
using System.Collections.Generic;
using MIDE.Application.Configuration;
using static System.Windows.Application;

namespace MIDE.WPFApp.Services
{
    public class WpfUIManager : UIManager
    {
        public override void RegisterUIExtension(string path)
        {
            //TODO: detect type of extension
            //TODO: validate extension
            //TODO: attach extension
        }
        
        public ResourceDictionary LoadTheme()
        {
            string id   = ConfigurationManager.Instance["theme"] as string ?? "default";
            string data = AppKernel.Instance.FileManager.ReadOrCreate($"root\\themes\\{id}.json", "{}");
            var items   = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            ResourceDictionary colors = new ResourceDictionary();
            foreach (var kvp in items)
            {
                var brush = ColorConverter.ConvertFromString(kvp.Value);
                colors[kvp.Key] = brush;
            }
            return colors;
        }
    }
}