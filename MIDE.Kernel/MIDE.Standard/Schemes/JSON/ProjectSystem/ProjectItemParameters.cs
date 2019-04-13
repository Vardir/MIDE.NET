using Newtonsoft.Json;
using System.Collections.Generic;

namespace MIDE.Schemes.JSON.ProjectSystem
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ProjectItemParameters
    {
        [JsonProperty(PropertyName = "icons")]
        public Dictionary<string, string> Icons { get; set; }

        [JsonProperty(PropertyName = "file-templates")]
        public Dictionary<string, string> FileTemplates { get; set; }

        [JsonProperty(PropertyName = "file-extensions")]
        public Dictionary<string, string> FileExtensions { get; set; }
    }
}
