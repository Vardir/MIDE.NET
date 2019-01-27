using Newtonsoft.Json;

namespace MIDE.Schemes.JSON
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ApplicationConfig
    {
        [JsonRequired]
        [JsonProperty(PropertyName = "kernel_version")]
        public string KernelVersion { get; set; }

        [JsonProperty(PropertyName = "paths")]
        public ApplicationPathItem[] Paths { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class ApplicationPathItem
    {
        [JsonRequired]
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }

        [JsonRequired]
        [JsonProperty(PropertyName = "path")]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
    }
}