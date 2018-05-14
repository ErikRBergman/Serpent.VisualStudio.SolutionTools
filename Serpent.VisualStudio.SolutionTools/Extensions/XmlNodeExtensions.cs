using System;
using System.Collections.Generic;
using System.Text;

namespace Serpent.VisualStudio.SolutionTools.Extensions
{
    using System.Linq;
    using System.Xml;

    public static class XmlNodeExtensions
    {
        public static IEnumerable<XmlElement> Elements(this XmlNode parentNode, string name)
        {
            return parentNode.ChildNodes.OfType<XmlElement>().Where(cn => cn.Name == name);
        }

        public static XmlElement Element(this XmlNode parentNode, string name)
        {
            return parentNode.Elements(name).FirstOrDefault();
        }
    }
}
