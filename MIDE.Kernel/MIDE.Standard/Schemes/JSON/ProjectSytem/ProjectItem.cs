using Newtonsoft.Json;
using MIDE.API.DataModels;
using Newtonsoft.Json.Converters;

namespace MIDE.Schemes.JSON
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ProjectItem
    {
        [JsonProperty(PropertyName = "is_hidden")]
        public bool IsHidden { get; set; }

        [JsonProperty(PropertyName = "is_required")]
        public bool Required { get; set; }

        [JsonProperty(PropertyName = "kind")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ProjectItemKind Kind { get; set; }
        
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        
        [JsonProperty(PropertyName = "template")]
        public string Template { get; set; }

        [JsonProperty(PropertyName = "items")]
        public ProjectItem[] Children { get; set; }

        [JsonProperty(PropertyName = "allowed")]
        public ProjectItem[] AllowedItems { get; set; }
    }
}