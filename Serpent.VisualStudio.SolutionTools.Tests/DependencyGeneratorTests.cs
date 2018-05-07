namespace Serpent.VisualStudio.SolutionTools.Tests
{
    using System.Linq;

    using Serpent.VisualStudio.SolutionTools.Models;

    using Xunit;

    public class DependencyGeneratorTests
    {
        [Fact]
        public void GetProjectsWithReferrals_Test()
        {
            //// Input (references):
            //// A -> B -> C
            //// D -> E -> C
            //// 
            //// Output
            //// C (2 referrals, 0 references),
            //// .B (1 referral (A), 1 reference (C))
            //// .E (1 referral (D), 1 reference (C))
            //// ..A (0 referrals, 1 reference (B))
            //// ..D (0 referrals, 1 reference (E))
            var projects = new[]
                               {
                                   new Project
                                       {
                                           Name = "A",
                                           References = new[]
                                                            {
                                                                new Reference(ReferenceType.Project)
                                                                    {
                                                                        ReferenceName = "B"
                                                                    }
                                                            }
                                       },
                                   new Project
                                       {
                                           Name = "B",
                                           References = new[]
                                                            {
                                                                new Reference(ReferenceType.Project)
                                                                    {
                                                                        ReferenceName = "C"
                                                                    }
                                                            }
                                       },
                                   new Project
                                       {
                                           Name = "C",
                                           References = new Reference[0]
                                       },
                                   new Project
                                       {
                                           Name = "D",
                                           References = new[]
                                                            {
                                                                new Reference(ReferenceType.Project)
                                                                    {
                                                                        ReferenceName = "E"
                                                                    }
                                                            }
                                       },
                                   new Project
                                       {
                                           Name = "E",
                                           References = new[]
                                                            {
                                                                new Reference(ReferenceType.Project)
                                                                    {
                                                                        ReferenceName = "C"
                                                                    }
                                                            }
                                       }
                               };

            var relations = DependencyGenerator.GetProjectsWithReferrals(projects);

            Assert.Single(relations);

            var c = relations.Single();

            Assert.Equal(2, c.Referrals.Count);

            var b = c.Referrals.Single(p => p.Name == "B");
            var e = c.Referrals.Single(p => p.Name == "E");

            var a = b.Referrals.Single(p => p.Name == "A");
            var d = e.Referrals.Single(p => p.Name == "D");
        }
    }
}