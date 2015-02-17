using System;

namespace CSharpDocumentation
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class CodeExampleAttribute : Attribute
    {
        public string Code { get; private set; }

        public CodeExampleAttribute(string code)
        {
            Code = code;
        }
    }
}
