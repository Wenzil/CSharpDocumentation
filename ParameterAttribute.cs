using System;

namespace CSharpDocumentation
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Delegate, Inherited = false, AllowMultiple = true)]
    public class ParameterAttribute : Attribute
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        public ParameterAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
