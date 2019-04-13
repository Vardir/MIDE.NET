using Newtonsoft.Json;
using System.Collections.Generic;

namespace MIDE.Schemes.JSON
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ProjectScheme
    {
        [JsonRequired]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonRequired]
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
        
        [JsonProperty(PropertyName = "icon")]
        public string Icon { get; set; }

        [JsonProperty(PropertyName = "templates")]
        public Dictionary<string, string> Templates { get; set; }

        [JsonProperty(PropertyName = "items")]
        public ProjectItemTemplate[] Items { get; set; }

        public string SchemePath { get; set; }

        public IEnumerable<ProjectItemTemplate> GetRequiredItems()
        {
            foreach (var item in Items)
            {
                if (item.IsRequired)
                    yield return item;
            }
        }
    }
}