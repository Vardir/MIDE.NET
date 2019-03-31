using MIDE.Schemes.JSON;
using System.Collections.Generic;

namespace MIDE.API.DataModels
{
    public class Project
    {
        public List<Reference> References { get; }

        public Project()
        {
            References = new List<Reference>();
        }
    }

    public enum ProjectItemKind
    {
        Any,
        Virtual,
        File,
        Folder
    }
}