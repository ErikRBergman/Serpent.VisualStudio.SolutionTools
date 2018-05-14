namespace Serpent.VisualStudio.SolutionTools
{
    using System.Collections.Generic;
    using System.Linq;

    using Serpent.VisualStudio.SolutionTools.Models;

    internal class InternalProjectWithRelations
    {
        public InternalProjectWithRelations(Project project)
        {
            this.Project = project;
        }

        public string Name => this.Project.Name;

        public Project Project { get; }

        public List<InternalProjectWithRelations> Referrals { get; } = new List<InternalProjectWithRelations>();

        public List<InternalProjectWithRelations> InternalReferences { get; } = new List<InternalProjectWithRelations>();

        public ProjectWithRelations ToProjectWithRelations(Dictionary<Project, ProjectWithRelations> context)
        {
            if (context.TryGetValue(this.Project, out var project))
            {
                return project;
            }

            project = new ProjectWithRelations(this.Project, this.InternalReferences.Select(r => r.ToProjectWithRelations(context)).ToArray(), this.Referrals.Select(r => r.ToProjectWithRelations(context)).ToArray());
            context.Add(project.Project, project);

            return project;
        }

        public override string ToString()
        {
            return this.Name + ", References: " + this.InternalReferences.Count + ", Referrals: " + this.Referrals.Count;
        }
    }
}