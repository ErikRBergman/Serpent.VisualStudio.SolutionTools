namespace Serpent.VisualStudio.SolutionTools
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;

    using Serpent.VisualStudio.SolutionTools.Extensions;
    using Serpent.VisualStudio.SolutionTools.Helpers;
    using Serpent.VisualStudio.SolutionTools.Models;

    public class ProjectLoader
    {
        public static IEnumerable<Reference> GetItemGroupPackageReferences(XmlElement xmlProject)
        {
            var references = new List<Reference>();

            foreach (var packageReference in xmlProject.Elements("PackageReference"))
            {
                var referenceName = packageReference.Attribute("Include");
                var version = packageReference.Attribute("Version");

                references.Add(
                    new Reference(ReferenceType.Package)
                        {
                            ReferenceName = referenceName.Value,
                            Version = Version.Parse(version.Value)
                        });
            }

            return references;
        }

        public static IEnumerable<Reference> GetItemGroupProjectReferences(XmlElement xmlProject)
        {
            var references = new List<Reference>();

            foreach (var packageReference in xmlProject.Elements("ProjectReference"))
            {
                var referenceName = packageReference.Attribute("Include");

                references.Add(
                    new Reference(ReferenceType.Project)
                        {
                            ReferenceName = Path.GetFileNameWithoutExtension(referenceName.Value),
                            ReferenceFullName = referenceName.Value
                        });
            }

            return references;
        }

        public static IEnumerable<Reference> GetProjectReferences(XmlElement xmlProject)
        {
            var references = new List<Reference>();

            foreach (var propertyGroup in xmlProject.Elements("ItemGroup"))
            {
                references.AddRange(GetItemGroupPackageReferences(propertyGroup));
                references.AddRange(GetItemGroupProjectReferences(propertyGroup));
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

        public static Project Parse(string projectText)
        {
            var projectDocument = new XmlDocument();
            projectDocument.LoadXml(projectText);

            ProjectValidationService.ThrowIfInvalidProject(projectDocument);

            XmlElement xmlProject = (XmlElement)projectDocument.FirstChild;

            var projectVersions = GetProjectVersions(xmlProject);
            var references = GetProjectReferences(xmlProject);

            return new Project
                       {
                           Versions = projectVersions,
                           References = references.ToArray()
                       };
        }

        private static ProjectVersions GetProjectVersions(XmlElement xmlProject)
        {
            var projectVersions = new ProjectVersions();

            foreach (XmlElement propertyGroup in xmlProject.Elements("PropertyGroup"))
            {
                var version = propertyGroup.Element("Version");
                if (version != null)
                {
                    projectVersions.Version = Version.Parse(version.InnerText);
                }

                var fileVersion = propertyGroup.Element("FileVersion");
                if (fileVersion != null)
                {
                    projectVersions.FileVersion = Version.Parse(fileVersion.InnerText);
                }

                var assemblyVersion = propertyGroup.Element("AssemblyVersion");
                if (assemblyVersion != null)
                {
                    projectVersions.AssemblyVersion = Version.Parse(assemblyVersion.InnerText);
                }
            }

            return projectVersions;
        }
    }
}