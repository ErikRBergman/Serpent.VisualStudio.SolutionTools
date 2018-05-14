namespace Serpent.VisualStudio.SolutionTools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Serpent.Common.BaseTypeExtensions.Collections;
    using Serpent.VisualStudio.SolutionTools.Models;

    public class ProjectRelationsService
    {
        public static IEnumerable<ProjectWithRelations> GetProjectsWithRelationsTree(IEnumerable<Project> projects)
        {
            var duplicates = projects.Duplicates(p => p.Name).ToArray();
            if (duplicates.Any())
            {
                throw new Exception("Duplicate projects found in definition. Duplicate projects are: " + string.Join(", ", duplicates.Select(d => d.Name)));
            }

            var relations = projects.Select(p => new InternalProjectWithRelations(p)).ToArray();

            var projectRelations = relations.ToDictionary(p => p.Project.Name);

            foreach (var project in relations)
            {
                foreach (var reference in project.Project.References)
                {
                    if (projectRelations.TryGetValue(reference.ReferenceName, out var referencedProject))
                    {
                        referencedProject.Referrals.Add(project);
                        project.InternalReferences.Add(referencedProject);
                    }
                }
            }

            var context = new Dictionary<Project, ProjectWithRelations>();
            return projectRelations.Values.Where(p => p.InternalReferences.Count == 0).Select(p => p.ToProjectWithRelations(context)).ToArray();
        }
    }
}