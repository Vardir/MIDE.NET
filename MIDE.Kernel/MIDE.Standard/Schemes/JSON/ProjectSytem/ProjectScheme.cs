using Newtonsoft.Json;

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

        [JsonRequired]
        [JsonProperty(PropertyName = "root")]
        public ProjectItem Root { get; set; }
    }
}