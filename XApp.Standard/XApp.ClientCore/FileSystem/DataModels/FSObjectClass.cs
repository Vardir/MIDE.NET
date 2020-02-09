using System;

using Vardirsoft.XApp.Helpers;
using Vardirsoft.XApp.Visuals;

namespace Vardirsoft.XApp.FileSystem
{
    /// <summary>
    /// A class to describe a file system objects such as files and directories
    /// </summary>
    public class FSObjectClass
    {
        private string _extension;

        public bool IsFile => Extension != FileSystemInfo.DRIVE_EXTENSION && Extension != FileSystemInfo.FOLDER_EXTENSION;
        public bool IsDrive => Extension == FileSystemInfo.DRIVE_EXTENSION;
        public bool IsFolder => Extension == FileSystemInfo.FOLDER_EXTENSION;
        public string Id { get; }
        public string Extension
        {
            get => _extension;
            private set
            {
                Guard.EnsureNotNull(value, typeof(ArgumentNullException));
                Guard.Ensure(FileSystemInfo.IsValidExtension(value), typeof(FormatException), "Extension was of invalid format");

                _extension = value;
            }
        }
        public string Editor { get; private set; }
        public Glyph ObjectGlyph { get; internal set; }

        public FSObjectClass(string objectClass, string extension) 
            : this(objectClass, extension, null, null) { }

        public FSObjectClass(string objectClass, string extension, Glyph objectGlyph)
            : this(objectClass, extension, null, objectGlyph) { }

        public FSObjectClass(string objectClass, string extension, string editor)
            : this(objectClass, extension, editor, null) { }

        public FSObjectClass(string objectClass, string extension, string editor, Glyph objectGlyph)
        {
            Guard.EnsureNotNull(objectClass, typeof(ArgumentNullException));
            
            Editor = editor;
            Extension = extension;
            Id = objectClass;
            ObjectGlyph = objectGlyph;
        }

        public override string ToString() => $"{Id}: {Extension}";

        internal FSObjectClass With(string extension = null, string editor = null, Glyph glyph = null)
        {
            var ext = extension ?? Extension;
            var edt = editor ?? Editor;
            var  glh = glyph ?? ObjectGlyph;

            return new FSObjectClass(Id, ext, edt, glh);
        }
    }
}