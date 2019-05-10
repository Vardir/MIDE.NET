using System;
using System.Windows;
using Newtonsoft.Json;
using MIDE.WPF.Windows;
using MIDE.Application;
using MIDE.API.Services;
using System.Reflection;
using MIDE.API.Components;
using System.Windows.Media;
using System.Collections.Generic;
using MIDE.Application.Configuration;

namespace MIDE.WPF.Services
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

        public override void RegisterUIExtension(object obj)
        {
            throw new NotImplementedException();
        }
        public override void RegisterUIExtension(Type type)
        {
            throw new NotImplementedException();
        }
        public override void RegisterUIExtension(Assembly assembly)
        {
            
        }

        protected override (DialogResult result, T value) OpenDialog_Impl<T>(BaseDialogBox<T> dialogBox)
        {
            if (dialogBox == null)
            {
                AppKernel.Instance.AppLogger.PushWarning("Attempting to open empty dialog box");
                return default;
            }
            return DialogWindow.Show(dialogBox);
        }
    }
}