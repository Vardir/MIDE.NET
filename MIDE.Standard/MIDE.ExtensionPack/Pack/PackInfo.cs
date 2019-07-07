﻿namespace MIDE.ExtensionPack
{
    public struct PackInfo
    {
        public readonly string id;
        public readonly string title;
        public readonly string description;
        public readonly string releaseNotes;

        public PackInfo(string id, string title, string description, string releaseNotes)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.releaseNotes = releaseNotes;
        }
    }
}