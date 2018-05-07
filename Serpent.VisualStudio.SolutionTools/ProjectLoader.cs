namespace Serpent.VisualStudio.SolutionTools
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using Serpent.VisualStudio.SolutionTools.Helpers;
    using Serpent.VisualStudio.SolutionTools.Models;

    public class ProjectLoader
    {
        public static Project Parse(string projectText)
        {
            var doc = XDocument.Parse(projectText);

            if (doc.Root == null)
            {
                throw new Exception("Unparsable XML");
            }

            if (doc.Root.Name != "Project" && doc.Root.Attribute("Sdk").Value != "Microsoft.NET.Sdk")
            {
                throw new Exception("The project is not a valid Visual Studio 2017 project file");
            }

            var xmlProject = doc.Root;

            var projectVersions = GetProjectVersions(xmlProject);
            var references = GetProjectReferences(xmlProject);

            return new Project
            {
                Versions = projectVersions,
                References = references.ToArray()
            };
        }

        private static ProjectVersions GetProjectVersions(XElement xmlProject)
        {
            var projectVersions = new ProjectVersions();

            foreach (var propertyGroup in xmlProject.Elements("PropertyGroup"))
            {
                var version = propertyGroup.Element("Version");
                if (version != null)
                {
                    projectVersions.Version = Version.Parse(version.Value);
                }

                var fileVersion = propertyGroup.Element("FileVersion");
                if (fileVersion != null)
                {
                    projectVersions.FileVersion = Version.Parse(fileVersion.Value);
                }

                var assemblyVersion = propertyGroup.Element("AssemblyVersion");
                if (assemblyVersion != null)
                {
                    projectVersions.AssemblyVersion = Version.Parse(assemblyVersion.Value);
                }
            }

            return projectVersions;
        }

        public static IEnumerable<Reference> GetProjectReferences(XElement xmlProject)
        {
            var references = new List<Reference>();

            foreach (var propertyGroup in xmlProject.Elements("ItemGroup"))
            {
                references.AddRange(GetItemGroupPackageReferences(propertyGroup));
                references.AddRange(GetItemGroupProjectReferences(propertyGroup));
            }

            return references;
        }


        public static IEnumerable<Reference> GetItemGroupPackageReferences(XElement xmlProject)
        {
            var references = new List<Reference>();

            foreach (var packageReference in xmlProject.Elements("PackageReference"))
            {
                var referenceName = packageReference.Attribute("Include");
                var version = packageReference.Attribute("Version");

                references.Add(new Reference(ReferenceType.Package)
                {
                    ReferenceName = referenceName.Value,
                    Version = Version.Parse(version.Value)
                });
            }

            return references;
        }

        public static IEnumerable<Reference> GetItemGroupProjectReferences(XElement xmlProject)
        {
            var references = new List<Reference>();

            foreach (var packageReference in xmlProject.Elements("ProjectReference"))
            {
                var referenceName = packageReference.Attribute("Include");

                references.Add(new Reference(ReferenceType.Project)
                {
                    ReferenceName = Path.GetFileNameWithoutExtension(referenceName.Value),
                    ReferenceFullName = referenceName.Value,
                });
            }

            return references;
        }

        public static async Task<Project> LoadProjectAsync(string filename)
        {
            var projectText = await FileHelper.ReadAllText(filename);
            var project = Parse(projectText);
            project.Filename = filename;
            project.Name = Path.GetFileNameWithoutExtension(filename);
            return project;
        }
    }
}