using System;

namespace CSharpDocumentation
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class SummaryAttribute : Attribute
    {
        public string Description { get; private set; }

        public SummaryAttribute(string description)
        {
            Description = description;
        }
    }
}
