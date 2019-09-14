using Newtonsoft.Json;

namespace XApp.Schemes.JSON
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ApplicationPathItem
    {
        [JsonRequired]
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }

        [JsonRequired]
        [JsonProperty(PropertyName = "path")]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "required")]
        public bool IsRequired { get; set; }
    }
}