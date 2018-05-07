namespace Serpent.VisualStudio.SolutionTools.Models
{
    using System.Collections.Generic;

    public class ProjectWithRelations
    {
        public ProjectWithRelations(Project project)
        {
            this.Project = project;
        }

        public Project Project { get; }

        public string Name => this.Project.Name;

        public IReadOnlyCollection<ProjectWithRelations> Referrals { get; set; }

        public IReadOnlyCollection<ProjectWithRelations> InternalReferences { get; set; }

        public override string ToString()
        {
            return this.Name + ", References: " + this.InternalReferences.Count + ", Referrals: " + this.Referrals.Count;
        }
    }
}