using Newtonsoft.Json;

namespace Vardirsoft.XApp.Schemes.JSON
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ExtensionEntry
    {
        [JsonRequired]
        [JsonProperty(PropertyName = "key")]
        public string Id { get; set; }

        [JsonRequired]
        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        [JsonProperty(PropertyName = "enabled")]
        public bool IsEnabled { get; set; }
    }
}