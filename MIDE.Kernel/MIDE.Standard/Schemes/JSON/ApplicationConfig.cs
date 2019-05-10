using Newtonsoft.Json;
using MIDE.Application.Logging;
using Newtonsoft.Json.Converters;

namespace MIDE.Schemes.JSON
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ApplicationConfig
    {
        [JsonRequired]
        [JsonProperty(PropertyName = "kernel_version")]
        public string KernelVersion { get; set; }
        
        [JsonProperty(PropertyName = "theme")]
        public string Theme { get; set; }

        [JsonProperty(PropertyName = "log_levels", ItemConverterType = typeof(StringEnumConverter))]
        public LoggingLevel[] LoggingLevels { get; set; }

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