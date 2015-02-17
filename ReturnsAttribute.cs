using System;

namespace CSharpDocumentation
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Delegate, Inherited = false, AllowMultiple = false)]
    public class ReturnsAttribute : Attribute
    {
        public string Description { get; private set; }

        public ReturnsAttribute(string description)
        {
            Description = description;
        }
    }
}
