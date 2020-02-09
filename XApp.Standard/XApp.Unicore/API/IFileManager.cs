using System;
using System.Collections.Generic;
using System.IO;

namespace Vardirsoft.XApp.API
{
    public interface IFileManager
    {
        /// <summary>
        /// Recursively deletes a directory from a specified path
        /// </summary>
        /// <param name="path"></param>
        void DeleteDirectory(string path);
        
        /// <summary>
        /// Writes the given string to the given file
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        void Write(string data, string path);
        
        /// <summary>
        /// Writes the given lines of text to the given file
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        void Write(string[] data, string path);
        void Write(IEnumerable<string> data, string path);
        
        /// <summary>
        /// Copies all the files and subdirectories from the source directory to the given destination
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        void Copy(string source, string destination);
        
        /// <summary>
        /// Tries to serialize the given object in file, if it is impossible writes log
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        void Serialize(object data, string path);
        
        /// <summary>
        /// Cleans all the contents of the given directory
        /// </summary>
        /// <param name="path"></param>
        void CleanDirectory(string path);

        string GetAbsolutePath(string path);
        string GetParentDirectory(string path);

        /// <summary>
        /// Verifies if the given path exists, it can be either file or directory
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool Exists(string path);

        /// <summary>
        /// Verifies if the given path is existing file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool FileExists(string path);

        /// <summary>
        /// Verifies if the given path is existing directory
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool DirectoryExists(string path);

        /// <summary>
        /// Verifies if the given path is file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool IsFile(string path);

        /// <summary>
        /// Tries to load bytes from the given file and returns null if file does not exist
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        byte[] TryReadBytes(string path);

        TResult OpenBinaryFile<TResult>(string path, Func<BinaryReader, TResult> readingAction);
        
        string GetFileParent(string file);

        /// <summary>
        /// Tries to load data from the given file and returns null if file does not exist
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        string TryRead(string filePath);

        /// <summary>
        /// Tries to load data from the given file and returns null if file does not exist
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        string[] TryReadLines(string filePath);

        /// <summary>
        /// Extracts the exact file or directory name from the given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string ExtractName(string path);

        /// <summary>
        /// Deletes file or directory by the given path, returns warning if it is not possible
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string Delete(string path);

        /// <summary>
        /// Creates a directory by the given path, returns warning if it is not possible
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string MakeFolder(string path);

        /// <summary>
        /// Creates a file by the given path, if template path given copies all it's data to new file, returns warning if it is not possible
        /// </summary>
        /// <param name="path"></param>
        /// <param name="templatePath"></param>
        /// <returns></returns>
        string MakeFile(string path, string templatePath);

        /// <summary>
        /// Reads file by the given path, if file not exists creates and fills with default content
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="defaultContent"></param>
        /// <returns></returns>
        string ReadOrCreate(string filePath, string defaultContent = "");

        /// <summary>
        /// Combines a set of paths into one solid sequence
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        string Combine(params string[] paths);

        /// <summary>
        /// Enumerates all filtered files in the given directory
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        IEnumerable<string> EnumerateFiles(string directory, string filter = null);

        /// <summary>
        /// Tries to deserialize an object from the given file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        T Deserialize<T>(string path) where T: class;
    }
}