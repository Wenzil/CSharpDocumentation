using System;

namespace CSharpDocumentation
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class RemarksAttribute : Attribute
    {
        public string Description { get; private set; }

        public RemarksAttribute(string description)
        {
            Description = description;
        }
    }
}
