using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

using XApp.API;
using XApp.IoC;
using XApp.Logging;
using XApp.Helpers;

namespace XApp.Application.Localization
{
    public sealed class LocalizationProvider : ILocalizationProvider
    {
        private static LocalizationProvider instance;
        public static LocalizationProvider Instance => instance ?? (instance = new LocalizationProvider());

        public const string KEY_PATTERN = @"(\([a-zA-Z0-9_-]+:?[a-zA-Z0-9_-]*\))";

        private readonly Regex regex;
        private readonly Dictionary<string, LocalizationNamespace> namespaces;

        public string this[string str]
        {
            get
            {
                if (string.IsNullOrEmpty(str))
                    return str;

                return regex.Replace(str, Resolve);
            }
        }

        private LocalizationProvider()
        {
            regex = new Regex(KEY_PATTERN, RegexOptions.Compiled);
            namespaces = new Dictionary<string, LocalizationNamespace>()
            {
                [string.Empty] = new LocalizationNamespace()
            };
        }

        public void LoadFrom(string file)
        {
            var fileManager = IoCContainer.Resolve<IFileManager>();

            if (fileManager.FileExists(file))
            {
                var fileData = fileManager.TryRead(file);
                if (fileData.HasValue())
                {
                    Dictionary<string, string> pairs;
                    try
                    {
                        pairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileData);
                    }
                    catch (Exception ex)
                    {
                        IoCContainer.Resolve<ILogger>()?.PushError(ex, this, "Couldn't load localization data from a file: invalid syntax");
                        
                        return;
                    }

                    foreach (var kvp in pairs)
                    {
                        var (prefix, key) = Split(kvp.Key);
                        var space = GetOrAddNamespace(prefix);

                        space.Add(key, kvp.Value);
                    }
                }
            }
        }

        private string GetValue(string namespaceKey, string key)
        {
            if (namespaces.TryGetValue(namespaceKey, out var space))
                return space[key];

            return null;
        }
        private string Resolve(Match match)
        {
            var expr = match.Value.Between('(', ')');
            var (prefix, key) = Split(expr);
            var result = GetValue(prefix, key);

            if (result == null && prefix.HasValue())
            {    
                result = GetValue(string.Empty, key);
            }
                
            return result ?? expr;
        }
        private (string prefix, string key) Split(string str)
        {
            var (head, tail) = str.ExtractUntil(0, ':');
            if (string.IsNullOrEmpty(tail))
                return (string.Empty, head);

            return (head, tail);
        }
        private LocalizationNamespace GetOrAddNamespace(string key)
        {
            if (namespaces.TryGetValue(key, out var space))
                return space;
                
            space = new LocalizationNamespace();
            namespaces.Add(key, space);
            
            return space;
        }
    }
}