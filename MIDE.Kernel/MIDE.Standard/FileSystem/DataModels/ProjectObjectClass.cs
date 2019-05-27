using System;
using MIDE.FileSystem;
using MIDE.API.Visuals;

namespace MIDE.Application
{
    /// <summary>
    /// A class to describe a project system objects such as folders and files
    /// </summary>
    public class ProjectObjectClass
    {
        private string extension;

        public bool IsFile => Extension != FileSystemInfo.FOLDER_EXTENSION;
        public bool IsFolder => Extension == FileSystemInfo.FOLDER_EXTENSION;
        public string Id { get; }
        public string Extension
        {
            get => extension;
            private set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(extension));
                if (!FileSystemInfo.IsValidExtension(value))
                    throw new FormatException("Extension was of invalid format");

                extension = value;
            }
        }
        public string ObjectTemplate { get; private set; }
        public string Editor { get; private set; }
        public Glyph ObjectGlyph { get; private set; }

        public ProjectObjectClass(string objectClass, string extension)
            : this(objectClass, extension, null, null) { }

        public ProjectObjectClass(string objectClass, string extension, Glyph objectGlyph)
            : this(objectClass, extension, null, objectGlyph) { }

        public ProjectObjectClass(string objectClass, string extension, string editor)
            : this(objectClass, extension, editor, null) { }

        public ProjectObjectClass(string objectClass, string extension, string editor, Glyph objectGlyph)
            : this(objectClass, extension, editor, null, objectGlyph) { }

        public ProjectObjectClass(string objectClass, string extension, string editor, string template, Glyph objectGlyph)
        {
            if (objectClass == null)
                throw new ArgumentNullException(nameof(objectClass));

            Editor = editor;
            Extension = extension;
            Id = objectClass;
            ObjectTemplate = template;
            ObjectGlyph = objectGlyph;
        }

        public override string ToString() => $"{Id}: {Extension}";

        internal ProjectObjectClass With(string extension = null, string editor = null, string template = null, Glyph glyph = null)
        {
            string ext = extension ?? Extension;
            string edt = editor ?? Editor;
            string tpl = template ?? ObjectTemplate;
            Glyph glh = glyph ?? ObjectGlyph;
            return new ProjectObjectClass(Id, ext, edt, tpl, glh);
        }
    }
}