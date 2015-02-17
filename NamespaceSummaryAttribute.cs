using System;

namespace CSharpDocumentation
{
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
    public class NamespaceSummaryAttribute : Attribute
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        public NamespaceSummaryAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
