namespace Serpent.VisualStudio.SolutionTools.Tests
{
    using System;
    using System.Linq;

    using Serpent.VisualStudio.SolutionTools.Models;

    using Xunit;

    public class ProjectLoaderTests
    {
        [Fact]
        public void ProjectLoader_Parse_Test()
        {
            var projectText =
                "<Project Sdk=\"Microsoft.NET.Sdk\">  <PropertyGroup>    <Version>0.0.0.1</Version>    <FileVersion>0.0.0.2</FileVersion>    <AssemblyVersion>0.0.0.3</AssemblyVersion>  </PropertyGroup>"
                + "  <ItemGroup>    <PackageReference Include=\"packageReference1\" Version=\"1.0.0.0\" />    <PackageReference Include=\"packageReference2\" Version=\"1.0.0.1\" />  </ItemGroup>"
                + "  <ItemGroup>    <ProjectReference Include=\"..\\projectReference\\projectReference.csproj\" />  </ItemGroup></Project>";

            var project = ProjectLoader.Parse(projectText);

            Assert.Equal(Version.Parse("0.0.0.1"), project.Versions.Version);
            Assert.Equal(Version.Parse("0.0.0.2"), project.Versions.FileVersion);
            Assert.Equal(Version.Parse("0.0.0.3"), project.Versions.AssemblyVersion);

            Assert.Equal(3, project.References.Count);

            var packageReference1 = project.References.FirstOrDefault(p => p.ReferenceName == "packageReference1");
            Assert.NotNull(packageReference1);
            Assert.Equal(Version.Parse("1.0.0.0"), packageReference1.Version);
            Assert.Equal("packageReference1", packageReference1.ReferenceName);
            Assert.Equal(ReferenceType.Package, packageReference1.ReferenceType);

            var packageReference2 = project.References.FirstOrDefault(p => p.ReferenceName == "packageReference2");
            Assert.NotNull(packageReference2);
            Assert.Equal("packageReference2", packageReference2.ReferenceName);
            Assert.Equal(Version.Parse("1.0.0.1"), packageReference2.Version);
            Assert.Equal(ReferenceType.Package, packageReference2.ReferenceType);

            var projectReference = project.References.FirstOrDefault(p => p.ReferenceName == "projectReference");
            Assert.NotNull(projectReference);
            Assert.Equal(ReferenceType.Project, projectReference.ReferenceType);
            Assert.Equal("projectReference", projectReference.ReferenceName);
            Assert.Equal("..\\projectReference\\projectReference.csproj", projectReference.ReferenceFullName);
        }
    }
}