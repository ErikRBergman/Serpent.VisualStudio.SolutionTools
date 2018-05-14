namespace Serpent.VisualStudio.SolutionTools.File
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Serpent.Common.Async;
    using Serpent.VisualStudio.SolutionTools.Models;

    public static class ProjectFileLoader
    {
        public static Task<IEnumerable<Project>> LoadAllProjectsInDirectoriesAsync(string baseDirectory, Func<DirectoryAndProjectFiles, IEnumerable<string>> filter = null)
        {
            filter = filter ?? (files => files.ProjectFiles);
            return LoadAllProjectsInDirectoriesInternal(baseDirectory, d => Task.FromResult(filter(d)));
        }

        public static Task<IEnumerable<Project>> LoadAllProjectsInDirectoriesAsync(string baseDirectory, Func<DirectoryAndProjectFiles, Task<IEnumerable<string>>> filter = null)
        {
            // Set default filter
            filter = filter ?? (files => Task.FromResult<IEnumerable<string>>(files.ProjectFiles));
            return LoadAllProjectsInDirectoriesInternal(baseDirectory, filter);
        }

        private static async Task<IEnumerable<Project>> LoadAllProjectsInDirectoriesInternal(
            string baseDirectory,
            Func<DirectoryAndProjectFiles, Task<IEnumerable<string>>> filter = null)
        {
            var projectFiles = new List<string>(50);

            foreach (var directory in Directory.EnumerateDirectories(baseDirectory, "*.*", SearchOption.AllDirectories))
            {
                var files = Directory.EnumerateFiles(directory, "*.csproj").ToArray();

                if (filter != null)
                {
                    files = (await filter(new DirectoryAndProjectFiles(directory, files))).ToArray();
                }

                projectFiles.AddRange(files);
            }

            return (await projectFiles.ForEachAsync(ProjectLoader.LoadProjectAsync, 16)).OrderBy(p => p.Name);
        }

        public struct DirectoryAndProjectFiles
        {
            public DirectoryAndProjectFiles(string projectPath, IReadOnlyCollection<string> projectFiles)
            {
                this.ProjectPath = projectPath;
                this.ProjectFiles = projectFiles;
            }

            public IReadOnlyCollection<string> ProjectFiles { get; }

            public string ProjectPath { get; private set; }
        }
    }
}