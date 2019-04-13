using Newtonsoft.Json;

namespace MIDE.Schemes.JSON
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ProjectItemTemplate
    {
        [JsonProperty(PropertyName = "required")]
        public bool IsRequired { get; set; }

        [JsonProperty(PropertyName = "hidden")]
        public bool IsHidden { get; set; }

        [JsonProperty(PropertyName = "save_as")]
        public string SaveAs { get; set; }

        [JsonProperty(PropertyName = "template")]
        public string Template { get; set; }

        [JsonProperty(PropertyName = "required")]
        public string SaveOptions { get; set; }
        public string[] Allowed { get; set; }
    }
}