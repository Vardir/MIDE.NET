using Newtonsoft.Json;

namespace MIDE.Schemes.JSON
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ExtensionsInit
    {
        [JsonRequired]
        [JsonProperty(PropertyName = "register")]
        public RegisterItem[] Items { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class RegisterItem
    {
        [JsonProperty(PropertyName = "enabled")]
        public bool Enabled { get; set; } = true;

        [JsonRequired]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonRequired]
        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }
    }
}