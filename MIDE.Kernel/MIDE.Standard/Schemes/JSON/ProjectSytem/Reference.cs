using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MIDE.Schemes.JSON
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Reference
    {
        [JsonRequired]
        [JsonProperty(PropertyName = "target")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ReferenceType Type { get; set; }

        [JsonRequired]
        [JsonProperty(PropertyName = "path")]
        public string Name { get; set; }

        [JsonRequired]
        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        [JsonRequired]
        [JsonProperty(PropertyName = "path")]
        public string LoadingPrameters { get; set; }
    }

    public enum ReferenceType
    {
        AttachedProject,
        WebSource
    }
}