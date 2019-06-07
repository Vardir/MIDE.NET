using System;
using System.Windows;
using MIDE.FileSystem;
using Newtonsoft.Json;
using MIDE.Application;
using MIDE.WPF.Windows;
using MIDE.API.Services;
using System.Reflection;
using WPF.Extensibility;
using MIDE.API.Components;
using System.Windows.Media;
using System.Collections.Generic;
using MIDE.Application.Configuration;

namespace MIDE.WPF.Services
{
    public class WpfUIManager : UIManager
    {
        private LinkedList<UIExtension> loadedUIExtensions;

        public WpfUIManager()
        {
            loadedUIExtensions = new LinkedList<UIExtension>();
        }

        public void RegisterUIExtension(UIExtension extension)
        {
            if (extension == null)
            {
                AppKernel.Instance.AppLogger.PushWarning("Attempting to load empty UI extension");
                return;
            }
            loadedUIExtensions.AddLast(extension);
        }
        public override void RegisterUIExtension(object obj)
        {
            if (obj is UIExtension extension)
                RegisterUIExtension(extension);
            else
                AppKernel.Instance.AppLogger.PushWarning($"Expected [{typeof(UIExtension)}] but got [{obj?.GetType().FullName ?? "null"}]");
        }
        public override void RegisterUIExtension(string path)
        {
            Assembly assembly = null;
            try
            {
                assembly = Assembly.LoadFrom(path);
            }
            catch (Exception ex)
            {
                AppKernel.Instance.AppLogger.PushError(ex, this, $"Failed to load UI extensions assembly: {path}");
                return;
            }
            RegisterUIExtension(assembly);
        }
        public override void RegisterUIExtension(Type type)
        {
            if (!type.IsSubclassOf(typeof(UIExtension)))
            {
                AppKernel.Instance.AppLogger.PushWarning($"Expected type of [{typeof(UIExtension)}] but got [{type}]");
                return;
            }
            UIExtension extension = (UIExtension)Activator.CreateInstance(type);
            RegisterUIExtension(extension);
        }
        public override void RegisterUIExtension(Assembly assembly)
        {
            if (assembly == null)
            {
                AppKernel.Instance.AppLogger.PushWarning("Attempting to load extensions from empty assembly");
                return;
            }
            Type[] types = assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];
                if (!type.IsSubclassOf(typeof(UIExtension)))
                    continue;
                UIExtension extension = (UIExtension)Activator.CreateInstance(type);
                RegisterUIExtension(extension);
            }
        }

        public ResourceDictionary LoadTheme()
        {
            string id = null;
            ResourceDictionary colors = new ResourceDictionary();
            try
            {
                id = ConfigurationManager.Instance["theme"] as string ?? "default";
                string data = FileManager.ReadOrCreate($"root\\themes\\{id}.json", "{}");
                var items = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
                colors = new ResourceDictionary();
                foreach (var kvp in items)
                {
                    var brush = ColorConverter.ConvertFromString(kvp.Value);
                    colors[kvp.Key] = brush;
                }
            }
            catch (Exception ex)
            {
                AppKernel.Instance.AppLogger.PushError(ex, this, $"Failed to load UI theme: {id}");
            }
            return colors;
        }
        public IEnumerable<UIExtension> GetLoadedUIExtensions() => loadedUIExtensions;

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