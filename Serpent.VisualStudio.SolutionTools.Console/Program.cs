namespace Serpent.VisualStudio.SolutionTools.Console
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Serpent.VisualStudio.SolutionTools.Applications;
    using Serpent.VisualStudio.SolutionTools.File;

    internal class Program
    {
        private static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            var projects = (await ProjectFileLoader.LoadAllProjectsInDirectoriesAsync(
                                "c:\\projects\\insclear\\heartbeat\\src",
                                d => Directory.EnumerateFiles(d.ProjectPath, "*publish*.ps1").Any() ? d.ProjectFiles : Array.Empty<string>())).ToArray();

            var dependencies = ProjectRelationsService.GetProjectsWithRelationsTree(projects);

            var projectsInBuildOrder = BuildOrderService.GetProjectsInBuildOrder(dependencies);
        }
    }
}