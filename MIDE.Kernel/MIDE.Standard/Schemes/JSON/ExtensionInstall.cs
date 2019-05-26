using Newtonsoft.Json;

namespace MIDE.Schemes.JSON
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ExtensionInstall
    {
        [JsonRequired]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonRequired]
        [JsonProperty(PropertyName = "repo")]
        public string Repository { get; set; }

        [JsonProperty(PropertyName = "auto_enable")]
        public string AutoEnable { get; set; }
    }
}