namespace Serpent.VisualStudio.SolutionTools.Models
{
    using System.Collections.Generic;

    public class ProjectWithRelations
    {
        public ProjectWithRelations(
            Project project, 
            IReadOnlyCollection<ProjectWithRelations> internalReferences,
            IReadOnlyCollection<ProjectWithRelations> referrals)
        {
            this.Project = project;
            this.InternalReferences = internalReferences;
            this.Referrals = referrals;
        }

        public Project Project { get; }

        public string Name => this.Project.Name;

        public IReadOnlyCollection<ProjectWithRelations> Referrals { get; }

        public IReadOnlyCollection<ProjectWithRelations> InternalReferences { get; }

        public override string ToString()
        {
            return this.Name + ", References: " + this.InternalReferences.Count + ", Referrals: " + this.Referrals.Count;
        }
    }
}