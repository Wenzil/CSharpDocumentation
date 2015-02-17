using System;

namespace CSharpDocumentation
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public class ExceptionAttribute : Attribute
    {
        public Type ExceptionType { get; private set; }
        public string Description { get; private set; }

        public ExceptionAttribute(Type exceptionType, string description)
        {
            ExceptionType = exceptionType;
            Description = description;
        }
    }
}
