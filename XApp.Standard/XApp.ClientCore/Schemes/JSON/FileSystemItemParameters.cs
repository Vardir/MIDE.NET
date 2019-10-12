using System.Collections.Generic;

using Newtonsoft.Json;

namespace Vardirsoft.XApp.Schemes.JSON
{
    [JsonObject(MemberSerialization.OptIn)]
    public class FileSystemItemParameters
    {
        [JsonProperty(PropertyName = "file-editors")]
        public Dictionary<string, string> FileEditors { get; set; }

        [JsonProperty(PropertyName = "file-extensions")]
        public Dictionary<string, string> FileExtensions { get; set; }
    }
}