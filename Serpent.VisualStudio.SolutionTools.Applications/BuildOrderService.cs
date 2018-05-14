using System;

namespace Serpent.VisualStudio.SolutionTools.Applications
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Serpent.Common.BaseTypeExtensions.Collections;
    using Serpent.VisualStudio.SolutionTools.Models;

    public static class BuildOrderService
    {
        public static IReadOnlyCollection<ProjectWithRelations> GetProjectsInBuildOrder(IEnumerable<ProjectWithRelations> projectsWithRelations)
        {
            var projectsInBuildOrder = new List<ProjectWithRelations>(projectsWithRelations.Where(p => p.InternalReferences.Count == 0));
            var projectsAdded = projectsInBuildOrder.ToHashSet();

            // the caller may not have sent the projects in a perfect tree, so we process all projects sent in, not just the ones in the root
            var projectsToProcess = projectsWithRelations.ToHashSet();

            var projectsToAddToProcess = new HashSet<ProjectWithRelations>();
            var projectsToRemoveFromProcess = new HashSet<ProjectWithRelations>();

            while (projectsToProcess.Count > 0)
            {
                var wasModified = false;

                foreach (var resolvedProject in projectsToProcess.Where(p => p.InternalReferences.All(projectsAdded.Contains)))
                {
                    wasModified = true;

                    if (projectsAdded.Add(resolvedProject))
                    {
                        projectsInBuildOrder.Add(resolvedProject);
                    }

                    projectsToAddToProcess.UnionWith(resolvedProject.Referrals.Where(p => projectsAdded.Contains(p) == false));
                    projectsToRemoveFromProcess.Add(resolvedProject);
                }


                if (!wasModified)
                {
                    throw new Exception("Stuck trying to resolve references for the current projects: " + string.Join(", ", projectsToProcess.Select(p => p.Name)));
                }

                projectsToProcess.UnionWith(projectsToAddToProcess);
                projectsToAddToProcess.Clear();

                projectsToProcess.ExceptWith(projectsToRemoveFromProcess);
                projectsToRemoveFromProcess.Clear();
            }

            return projectsInBuildOrder;
        }
    }
}
