namespace Serpent.VisualStudio.SolutionTools.Models
{
    using System;

    public class Reference
    {
        public Reference(ReferenceType referenceType)
        {
            this.ReferenceType = referenceType;
        }

        /// <summary>
        /// Gets or sets the reference type
        /// </summary>
        public ReferenceType ReferenceType { get; set; }

        /// <summary>
        /// Gets or sets the version referenced
        /// </summary>
        public Version Version { get; set; }

        /// <summary>
        /// Gets or sets the reference name
        /// </summary>
        public string ReferenceName { get; set; }

        /// <summary>
        /// Gets or sets the reference full name
        /// </summary>
        public string ReferenceFullName { get; set; }

        /// <summary>
        /// Gets or sets the referenced project, NOT the project referencing
        /// </summary>
        public Project Project { get; set; }

        public override string ToString()
        {
            return $"{this.ReferenceName} ({this.ReferenceType}){(this.Version != null ? this.Version.ToString() : "")}";
        }
    }
}