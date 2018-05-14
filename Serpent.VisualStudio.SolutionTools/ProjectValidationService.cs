namespace Serpent.VisualStudio.SolutionTools
{
    using System;
    using System.Xml;

    public static class ProjectValidationService
    {
        public static void ThrowIfInvalidProject(XmlDocument projectDocument)
        {
            XmlElement project = (XmlElement)projectDocument.FirstChild;

            if (project == null)
            {
                throw new Exception("Can't find a root element, this is not a Visual Studio 2017 project document");
            }

            if (project.Name != "Project")
            {
                throw new Exception("Root element is not \"project\" ergo, this is not a Visual Studio 2017 project document");
            }

            var sdkAttribute = project.Attributes["Sdk"];

            if (sdkAttribute == null || sdkAttribute.Value != "Microsoft.NET.Sdk")
            {
                throw new Exception("Root element is \"project\" but it does not have the Sdk attribute set correctly, this is not a Visual Studio 2017 project document");
            }
        }
    }
}