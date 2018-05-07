using System;

namespace Serpent.VisualStudio.SolutionTools.Console
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Serpent.Common.Async;

    using Console = System.Console;

    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            var projectFiles = new List<string>();

            foreach (var directory in Directory.EnumerateDirectories(args[0], "*.*", SearchOption.AllDirectories))
            {
                if (Directory.EnumerateFiles(directory, "*publish*.ps1").Any())
                {
                    projectFiles.AddRange(Directory.EnumerateFiles(directory, "*.csproj"));
                }
            }

            var projects = (await projectFiles.ForEachAsync(ProjectLoader.LoadProjectAsync, 16)).OrderBy(p => p.Name);

            var dependencies = DependencyGenerator.GetProjectsWithReferrals(projects);

            var x = projects;
        }
    }
}
