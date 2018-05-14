namespace Serpent.VisualStudio.SolutionTools.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;

    public static class XmlElementExtensions
    {
        public static IEnumerable<XmlAttribute> Attributes(this XmlElement element, string name)
        {
            return element.Attributes.Cast<XmlAttribute>().Where(cn => cn.Name == name);
        }

        public static XmlAttribute Attribute(this XmlElement element, string name)
        {
            return element.Attributes(name).FirstOrDefault();
        }
    }
}