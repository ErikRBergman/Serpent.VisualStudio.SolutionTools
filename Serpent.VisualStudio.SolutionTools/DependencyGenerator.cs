namespace Serpent.VisualStudio.SolutionTools
{
    using System.Collections.Generic;
    using System.Linq;

    using Serpent.VisualStudio.SolutionTools.Models;

    public class DependencyGenerator
    {
        public static IEnumerable<ProjectWithRelations> GetProjectsWithReferrals(IEnumerable<Project> projects)
        {
            var relations = projects.Select(p => new InternalProjectWithRelations(p)).ToArray();
            var projectRelations = relations.ToDictionary(p => p.Project.Name);

            foreach (var project in relations)
            {
                foreach (var reference in project.Project.References)
                {
                    var x = reference.ReferenceName;

                    if (projectRelations.TryGetValue(reference.ReferenceName, out var referencedProject))
                    {
                        referencedProject.Referrals.Add(project);
                        project.InternalReferences.Add(referencedProject);
                    }
                }
            }

            var context = new Dictionary<Project, ProjectWithRelations>();
            return projectRelations.Values.Select(p => p.ToProjectWithRelations(context)).Where(p => p.InternalReferences.Count == 0).ToArray();
        }

        private class InternalProjectWithRelations
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

                project = new ProjectWithRelations(this.Project);
                context.Add(project.Project, project);

                project.Referrals = this.Referrals.Select(r => r.ToProjectWithRelations(context)).ToArray();
                project.InternalReferences = this.InternalReferences.Select(r => r.ToProjectWithRelations(context)).ToArray();

                return project;
            }

            public override string ToString()
            {
                return this.Name + ", References: " + this.InternalReferences.Count + ", Referrals: " + this.Referrals.Count;
            }
        }
    }
}