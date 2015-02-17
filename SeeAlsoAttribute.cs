using System;

namespace CSharpDocumentation
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class SeeAlsoAttribute : Attribute
    {
        public string Reference { get; private set; }

        public SeeAlsoAttribute(string reference)
        {
            Reference = reference;
        }
    }
}
