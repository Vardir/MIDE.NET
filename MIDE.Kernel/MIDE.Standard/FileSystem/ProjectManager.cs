using System;
using System.Drawing;
using Newtonsoft.Json;
using MIDE.FileSystem;
using MIDE.API.Visuals;
using MIDE.API.Services;
using MIDE.API.DataModels;
using MIDE.API.Components;
using System.Collections.Generic;
using MIDE.Schemes.JSON.ProjectSystem;

namespace MIDE.Application
{
    public class ProjectManager
    {
        private static ProjectManager assetManager;
        public static ProjectManager Instance => (assetManager ?? (assetManager = new ProjectManager()));

        private readonly UIManager uiManager;
        private readonly FileManager fileManager;
        private readonly Dictionary<string, ProjectObjectClass> pjObjectClasses;

        private ProjectManager()
        {
            uiManager = AppKernel.Instance.UIManager;
            fileManager = AppKernel.Instance.FileManager;
            pjObjectClasses = new Dictionary<string, ProjectObjectClass>()
            {
                ["folder"] = new ProjectObjectClass("folder", FileSystemInfo.FOLDER_EXTENSION, new Glyph("\uf07b") { AlternateColor = Color.Orange }),
                ["file"]   = new ProjectObjectClass("file", FileSystemInfo.ANY_FILE_EXTENSION, new Glyph("\uf15b") { AlternateColor = Color.Silver })
            };

            Initialize();
        }

        public bool IsRegistered(string key) => pjObjectClasses.ContainsKey(key);
        public bool IsRegistered(ProjectObjectClass pjClass) => pjObjectClasses.ContainsKey(pjClass.Id);
        public bool RegisterClass(ProjectObjectClass pjClass)
        {
            if (pjClass == null)
                throw new ArgumentNullException(nameof(pjClass));
            if (FileSystemInfo.IsSpecialExtension(pjClass.Extension))
                throw new ArgumentException("Can not add duplicate special file system class", nameof(pjClass));

            if (pjObjectClasses.ContainsKey(pjClass.Id))
                return false;
            pjObjectClasses.Add(pjClass.Id, pjClass);
            return true;
        }
        public ProjectObjectClass Find(string id)
        {
            if (pjObjectClasses.TryGetValue(id, out ProjectObjectClass fsoClass))
                return fsoClass;
            return null;
        }
        public ProjectObjectClass FindBy(string extension)
        {
            if (extension == FileSystemInfo.ANY_FILE_EXTENSION)
                return pjObjectClasses["file"];
            if (extension == FileSystemInfo.FOLDER_EXTENSION)
                return pjObjectClasses["folder"];

            foreach (var kvp in pjObjectClasses)
            {
                if (kvp.Key == extension)
                    return kvp.Value;
            }
            return pjObjectClasses["file"];
        }
        public IEnumerable<ProjectObjectClass> Select(Func<ProjectObjectClass, bool> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));
            foreach (var kvp in pjObjectClasses)
            {
                if (match(kvp.Value))
                    yield return kvp.Value;
            }
        }

        public Project CreateProject()
        {
            var dialogBox = new CreateProjectDialogBox("Create project");
            var (dialogResult, creationArgs) = uiManager.OpenDialog(dialogBox);

            if (dialogResult == DialogResult.Cancel)
                return null;
            
            string path = fileManager.Combine(creationArgs.path, creationArgs.name);
            fileManager.MakeFolder(path);
            Project project = new Project(creationArgs.projectScheme);
            //TODO: initialize project

            return project;
        }
        public Project LoadProject(string path) => null;

        private void Initialize()
        {
            string fileData = fileManager.ReadOrCreate(fileManager.GetFilePath(FileManager.ASSETS, "project-items.json"), 
                                                       "{ \"icons\": null, \"extensions\": null, \"templates\": null }");
            ProjectItemParameters parameters = JsonConvert.DeserializeObject<ProjectItemParameters>(fileData);
            if (parameters.FileExtensions != null)
                LoadFileExtensions(parameters);
            if (parameters.Icons != null)
                LoadItemIcons(parameters);
            if (parameters.FileTemplates != null)
                LoadFileTemplates(parameters);
        }
        private void LoadItemIcons(ProjectItemParameters parameters)
        {
            foreach (var kvp in parameters.Icons)
            {
                Glyph glyph = null;
                string value = kvp.Value;
                if (value != null && value.Length > 4 && value.StartsWith("@fa-"))
                    glyph = new Glyph(value.Substring(4));
                 
                Update(kvp.Key, null, null, null, glyph ?? new Glyph('X'));
            }
        }
        private void LoadFileExtensions(ProjectItemParameters parameters)
        {
            foreach (var kvp in parameters.FileExtensions)
            {
                Update(kvp.Key, kvp.Value, null, null, null);
            }
        }
        private void LoadFileTemplates(ProjectItemParameters parameters)
        {
            foreach (var kvp in parameters.FileTemplates)
            {
                Update(kvp.Key, null, null, kvp.Value, null);
            }
        }
        private void Update(string key, string extension, string editor, string template, Glyph glyph)
        {
            if (pjObjectClasses.TryGetValue(key, out ProjectObjectClass objectClass))
                pjObjectClasses[key] = objectClass.With(extension, editor, template, glyph);
            else
                pjObjectClasses.Add(key, new ProjectObjectClass(key, extension, editor, template, glyph));
        }
    }
}