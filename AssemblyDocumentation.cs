using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSharpDocumentation
{
    public class AssemblyDocumentation
    {
        public static AssemblyDocumentation FetchAssemblyDocumentation(Assembly assembly)
        {
            return new AssemblyDocumentation(assembly);
        }

        private Dictionary<string, string> namespaceSummaries;
        public IEnumerable<string> DocumentedNamespaces { get { return namespaceSummaries.Keys; } }

        private AssemblyDocumentation(Assembly assembly)
        {
            namespaceSummaries = assembly.GetCustomAttributes<NamespaceSummaryAttribute>()
                .Where(ns => !string.IsNullOrEmpty(ns.Name) && !string.IsNullOrEmpty(ns.Description))
                .ToDictionary(ns => ns.Name, ns => ns.Description);
        }

        public string GetNamespaceSummary(string targetNamespace)
        {
            return HasNamespaceSummary(targetNamespace) ? namespaceSummaries[targetNamespace] : null;
        }

        public bool HasNamespaceSummary(string targetNamespace)
        {
            return namespaceSummaries.ContainsKey(targetNamespace);
        }
    }
}
