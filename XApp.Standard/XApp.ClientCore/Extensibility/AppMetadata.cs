using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using Vardirsoft.XApp.FileSystem;
using Vardirsoft.XApp.IoC;

namespace Vardirsoft.XApp.Extensibility
{
    public abstract class AppMetadata
    {
        public string Description { get; set; }
        
        public static void Save<T>(T metadata, Stream stream)
            where T : AppMetadata
        {
            if (metadata is null)
                throw new ArgumentNullException(nameof(metadata));
            
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));
            
            Serialize(metadata, stream);
        }

        public static void Save<T>(T metadata, string filePath)
            where T : AppMetadata
        {
            var fileManager = IoCContainer.Resolve<FileManager>();
            var directory = fileManager.GetParentDirectory(filePath);

            if (fileManager.DirectoryExists(directory))
            {
                var bytes = SaveToByteStream(metadata);
                
                fileManager.Write(bytes, filePath);
            }
            else throw new DirectoryNotFoundException(directory);
        }

        public static byte[] SaveToByteStream<T>(T metadata)
            where T : AppMetadata
        {
            using var memory = new MemoryStream();
            
            Serialize(metadata, memory);
            
            return memory.ToArray();
        }

        public static T Load<T>(Stream stream)
            where T : AppMetadata
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            return Deserialize<T>(stream);
        }
        
        public static T Load<T>(string filePath)
            where T : AppMetadata
        {
            var bytes = IoCContainer.Resolve<FileManager>().TryReadBytes(filePath);
            
            if (bytes is null)
                throw new SerializationException("Couldn't read metadata from file");
            
            using var memory = new MemoryStream();
            using var writer = new BinaryWriter(memory);
            
            writer.Write(bytes);

            return Deserialize<T>(memory);
        }

        private static T Deserialize<T>(Stream stream)
            where T : AppMetadata
        {
            var formatter = new BinaryFormatter();

            return (T)formatter.Deserialize(stream);
        }
        
        private static void Serialize<T>(T metadata, Stream stream)
            where T : AppMetadata
        {
            var formatter = new BinaryFormatter();
            
            formatter.Serialize(stream, metadata);
        }
    }

    public abstract class ComponentMetadata : AppMetadata
    {
        public string Id { get; set; }
        
        public string Version { get; set; }   
    }
    
    [Serializable]
    public sealed class ModuleMetadata : ComponentMetadata
    {
        
    }

    [Serializable]
    public sealed class ExtensionMetadata : ComponentMetadata
    {
        public string Title { get; set; }
        
        public string Copyright { get; set; }
        
        public string Authors { get; set; }
        
        public string Owners { get; set; }

        public string LiceseUrl { get; set; }
        
        public string ProjectUrl { get; set; }
        
        public string[] Tags { get; set; }

        public Dependency[] Dependencies { get; set; }
    }

    [Serializable]
    public struct Dependency
    {
        public DependencyTarget Target { get; set; }
        
        public string Version { get; set; }
    }

    public enum DependencyTarget
    {
        Kernel,
        Extension,
        Platform
    }
}