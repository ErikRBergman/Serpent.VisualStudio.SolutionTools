namespace Serpent.VisualStudio.SolutionTools.Models
{
    using System;
    using System.Collections.Generic;

    public class Project
    {
        public string Name { get; set; }

        public string Filename { get; set; }

        public ProjectVersions Versions { get; set; }
        
        public IReadOnlyCollection<Reference> References { get; set; }

        public override string ToString()
        {
            return $"{this.Name}";
        }
    }

    public class ProjectVersions
    {
        public Version Version { get; set; }

        public Version FileVersion { get; set; }

        public Version AssemblyVersion { get; set; }
    }
}