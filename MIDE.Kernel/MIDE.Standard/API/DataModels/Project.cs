using MIDE.Schemes.JSON;
using System.Collections.Generic;

namespace MIDE.API.DataModels
{
    public class Project
    {
        public ProjectScheme Scheme { get; }
        public List<ProjectItemTemplate> Items { get; }
        public List<Reference> References { get; }

        public Project(ProjectScheme scheme)
        {
            Scheme = scheme;
            Items = new List<ProjectItemTemplate>(scheme.Items);
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