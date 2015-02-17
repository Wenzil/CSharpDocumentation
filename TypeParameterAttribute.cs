using System;

namespace CSharpDocumentation
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Delegate | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class TypeParameterAttribute : Attribute
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        public TypeParameterAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
