using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MIDE.Schemes.JSON
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ExtensionsInstall
    {
        [JsonRequired]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonRequired]
        [JsonProperty(PropertyName = "repo")]
        public string Repository { get; set; }

        [JsonRequired]
        [JsonProperty(PropertyName = "mode")]
        [JsonConverter(typeof(StringEnumConverter))]
        public InstallationMode Mode { get; set; }

        [JsonProperty(PropertyName = "auto_enable")]
        public bool AutoEnable { get; set; }
    }

    public enum InstallationMode
    {
        Install,
        Update,
        Uninstall
    }
}