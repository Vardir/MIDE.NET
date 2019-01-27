using Newtonsoft.Json;
using MIDE.API.Services;
using Newtonsoft.Json.Converters;

namespace MIDE.Schemes.JSON
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ExtensionConfig
    {
        [JsonRequired]
        [JsonProperty(PropertyName = "self")]
        public string DllPath { get; set; }

        [JsonProperty(PropertyName = "include")]
        public ExtensionMember[] ExtensionMembers { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class ExtensionMember
    {
        [JsonRequired]
        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        [JsonRequired]
        [JsonProperty(PropertyName = "target")]
        [JsonConverter(typeof(StringEnumConverter))]
        public MemberTarget Target { get; set; }

        [JsonRequired]
        [JsonProperty(PropertyName = "platform")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Platform Platform { get; set; }

        [JsonRequired]
        [JsonProperty(PropertyName = "role")]
        [JsonConverter(typeof(StringEnumConverter))]
        public MemberRole Role { get; set; }
    }

    public enum MemberTarget
    {
        UI, Kernel
    }
    public enum MemberRole
    {
        Extension, Resource //TODO: add more roles
    }
}
