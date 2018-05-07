using System;
using System.Collections.Generic;
using System.Text;

namespace Serpent.VisualStudio.SolutionTools.Helpers
{
    using System.Threading.Tasks;

    internal static class FileHelper
    {
        public static async Task<string> ReadAllText(string filename)
        {
            using (var fileStream = System.IO.File.OpenText(filename))
            {
                return await fileStream.ReadToEndAsync();
            }
        }
    }
}
