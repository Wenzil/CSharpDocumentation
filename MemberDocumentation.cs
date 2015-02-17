using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CSharpDocumentation
{
    public class MemberDocumentation
    {
        private static StringBuilder warningsBuilder = new StringBuilder();
        private static ParameterOrderComparer parameterOrderComparer = new ParameterOrderComparer();

        public readonly SummaryAttribute Summary;
        public readonly IEnumerable<TypeParameterAttribute> TypeParameters;
        public readonly IEnumerable<ParameterAttribute> Parameters;
        public readonly ReturnsAttribute Returns;
        public readonly IEnumerable<ExceptionAttribute> Exceptions;
        public readonly IEnumerable<RemarksAttribute> Remarks;
        public readonly IEnumerable<CodeExampleAttribute> Examples;
        public readonly IEnumerable<SeeAlsoAttribute> SeeAlsos;
        public readonly string Warnings;

        public bool HasSummary { get { return Summary != null && !string.IsNullOrEmpty(Summary.Description); } }
        public bool HasTypeParameters { get { return TypeParameters.Any(); } }
        public bool HasParameters { get { return Parameters.Any(); } }
        public bool HasReturns { get { return Returns != null && !string.IsNullOrEmpty(Returns.Description); } }
        public bool HasExceptions { get { return Exceptions.Any(); } }
        public bool HasRemarks { get { return Remarks.Any(); } }
        public bool HasExamples { get { return Examples.Any(); } }
        public bool HasSeeAlsos { get { return SeeAlsos.Any(); } }
        public bool IsDocumented { get { return HasSummary || HasTypeParameters || HasParameters || HasReturns || HasExceptions || HasRemarks || HasExamples || HasSeeAlsos; } }

        private class ParameterOrderComparer : IComparer<string>
        {
            public string[] orderedParameterNames { get; set; }
            
            public int Compare(string a, string b)
            {
                return Array.IndexOf(orderedParameterNames, a).CompareTo(Array.IndexOf(orderedParameterNames, b));
            }
        }

        public static MemberDocumentation FetchMemberDocumentation(Type type)
        {
            var summary = type.GetCustomAttribute<SummaryAttribute>();
            var typeParameters = GetTypeParameterAttributes(type);
            var parameters = GetParameterAttributes(type); // for delegates
            var returns = type.GetCustomAttribute<ReturnsAttribute>(); // for delegates
            var exceptions = Enumerable.Empty<ExceptionAttribute>();
            var remarks = type.GetCustomAttributes<RemarksAttribute>().Where(r => !string.IsNullOrEmpty(r.Description));
            var examples = type.GetCustomAttributes<CodeExampleAttribute>().Where(ex => !string.IsNullOrEmpty(ex.Code));
            var seeAlsos = type.GetCustomAttributes<SeeAlsoAttribute>().Where(sa => !string.IsNullOrEmpty(sa.Reference));
            var warnings = warningsBuilder.ToString();
            warningsBuilder.Length = 0;

            return new MemberDocumentation(summary, typeParameters, parameters, returns, exceptions, remarks, examples, seeAlsos, warnings);
        }

        public static MemberDocumentation FetchMemberDocumentation(MethodInfo method)
        {
            var summary = method.GetCustomAttribute<SummaryAttribute>();
            var typeParameters = GetTypeParameterAttributes(method);
            var parameters = GetParameterAttributes(method);
            var returns = method.GetCustomAttribute<ReturnsAttribute>();
            var exceptions = method.GetCustomAttributes<ExceptionAttribute>()
                .Where(e => e.ExceptionType != null && !string.IsNullOrEmpty(e.Description))
                .OrderBy(e => e.ExceptionType.FullName);
            var remarks = method.GetCustomAttributes<RemarksAttribute>().Where(r => !string.IsNullOrEmpty(r.Description));
            var examples = method.GetCustomAttributes<CodeExampleAttribute>().Where(ex => !string.IsNullOrEmpty(ex.Code));
            var seeAlsos = method.GetCustomAttributes<SeeAlsoAttribute>().Where(sa => !string.IsNullOrEmpty(sa.Reference));
            var warnings = warningsBuilder.ToString();
            warningsBuilder.Length = 0;

            return new MemberDocumentation(summary, typeParameters, parameters, returns, exceptions, remarks, examples, seeAlsos, warnings);
        }

        public static MemberDocumentation FetchMemberDocumentation(ConstructorInfo constructor)
        {
            var summary = constructor.GetCustomAttribute<SummaryAttribute>();
            var typeParameters = Enumerable.Empty<TypeParameterAttribute>();
            var parameters = GetParameterAttributes(constructor);
            var returns = null as ReturnsAttribute;
            var exceptions = constructor.GetCustomAttributes<ExceptionAttribute>()
                .Where(e => e.ExceptionType != null && !string.IsNullOrEmpty(e.Description))
                .OrderBy(e => e.ExceptionType.FullName);
            var remarks = constructor.GetCustomAttributes<RemarksAttribute>().Where(r => !string.IsNullOrEmpty(r.Description));
            var examples = constructor.GetCustomAttributes<CodeExampleAttribute>().Where(ex => !string.IsNullOrEmpty(ex.Code));
            var seeAlsos = constructor.GetCustomAttributes<SeeAlsoAttribute>().Where(sa => !string.IsNullOrEmpty(sa.Reference));
            var warnings = warningsBuilder.ToString();
            warningsBuilder.Length = 0;

            return new MemberDocumentation(summary, typeParameters, parameters, returns, exceptions, remarks, examples, seeAlsos, warnings);
        }

        public static MemberDocumentation FetchMemberDocumentation(MemberInfo member)
        {
            var summary = member.GetCustomAttribute<SummaryAttribute>();
            var typeParameters = Enumerable.Empty<TypeParameterAttribute>();
            var parameters = Enumerable.Empty<ParameterAttribute>(); ;
            var returns = null as ReturnsAttribute;
            var exceptions = member.GetCustomAttributes<ExceptionAttribute>()
                .Where(e => e.ExceptionType != null && !string.IsNullOrEmpty(e.Description))
                .OrderBy(e => e.ExceptionType.FullName); // for properties
            var remarks = member.GetCustomAttributes<RemarksAttribute>().Where(r => !string.IsNullOrEmpty(r.Description));
            var examples = member.GetCustomAttributes<CodeExampleAttribute>().Where(ex => !string.IsNullOrEmpty(ex.Code));
            var seeAlsos = member.GetCustomAttributes<SeeAlsoAttribute>().Where(sa => !string.IsNullOrEmpty(sa.Reference));

            return new MemberDocumentation(summary, typeParameters, parameters, returns, exceptions, remarks, examples, seeAlsos, "");
        }

        private static IEnumerable<TypeParameterAttribute> GetTypeParameterAttributes(Type type)
        {
            var correctlyOrderedTypeParameterNames = type.GetGenericArguments().Select(tp => tp.Name).ToArray();
            var typeParameters = type.GetCustomAttributes<TypeParameterAttribute>();
            parameterOrderComparer.orderedParameterNames = correctlyOrderedTypeParameterNames;

            // validate that each type parameter attribute refers to an actual type parameter of the documented type
            foreach (var tp in typeParameters)
            {
                if (!correctlyOrderedTypeParameterNames.Contains(tp.Name))
                    warningsBuilder.Append(string.Format("The type parameter '{0}' documented as part of type {1} was excluded because {1} defines no such type parameter.\n", tp.Name, type.Name));
            }

            return typeParameters
                .Where(tp => !string.IsNullOrEmpty(tp.Name) && !string.IsNullOrEmpty(tp.Description))
                .Where(tp => correctlyOrderedTypeParameterNames.Contains(tp.Name))
                .OrderBy(tp => tp.Name, parameterOrderComparer);
        }

        private static IEnumerable<TypeParameterAttribute> GetTypeParameterAttributes(MethodInfo method)
        {
            var correctlyOrderedTypeParameterNames = method.GetGenericArguments().Select(tp => tp.Name).ToArray();
            var typeParameters = method.GetCustomAttributes<TypeParameterAttribute>();
            parameterOrderComparer.orderedParameterNames = correctlyOrderedTypeParameterNames;

            // validate that each type parameter attribute refers to an actual type parameter of the documented method
            foreach (var tp in typeParameters)
            {
                if (!correctlyOrderedTypeParameterNames.Contains(tp.Name))
                    warningsBuilder.Append(string.Format("The type parameter '{0}' documented as part of method {1}.{2}() was excluded because {1}.{2}() defines no such type parameter.\n", tp.Name, method.DeclaringType.Name, method.Name));
            }

            return typeParameters
                .Where(tp => !string.IsNullOrEmpty(tp.Name) && !string.IsNullOrEmpty(tp.Description))
                .Where(tp => correctlyOrderedTypeParameterNames.Contains(tp.Name))
                .OrderBy(tp => tp.Name, parameterOrderComparer);
        }

        private static IEnumerable<ParameterAttribute> GetParameterAttributes(MethodBase method)
        {
            var correctlyOrderedParameterNames = method.GetParameters().Select(p => p.Name).ToArray();
            var parameters = method.GetCustomAttributes<ParameterAttribute>();
            parameterOrderComparer.orderedParameterNames = correctlyOrderedParameterNames;

            // validate that each parameter attribute refers to an actual parameter of the documented method or constructor
            foreach (var p in parameters)
            {
                if (!correctlyOrderedParameterNames.Contains(p.Name))
                    warningsBuilder.Append(string.Format("The parameter '{0}' documented as part of method {1}.{2}() was exluded because {1}.{2}() accepts no such parameter.\n", p.Name, method.DeclaringType.Name, method.Name));
            }

            return parameters
                .Where(p => !string.IsNullOrEmpty(p.Name) && !string.IsNullOrEmpty(p.Description))
                .Where(p => correctlyOrderedParameterNames.Contains(p.Name))
                .OrderBy(p => p.Name, parameterOrderComparer);
        }

        private static IEnumerable<ParameterAttribute> GetParameterAttributes(Type type)
        {
            var isDelegate = typeof(MulticastDelegate).IsAssignableFrom(type.BaseType);
            if (!isDelegate)
                return Enumerable.Empty<ParameterAttribute>();
            
            var delegateMethod = type.GetMethod("Invoke");
            var correctlyOrderedParameterNames = delegateMethod.GetParameters().Select(p => p.Name).ToArray();
            var parameters = type.GetCustomAttributes<ParameterAttribute>();
            parameterOrderComparer.orderedParameterNames = correctlyOrderedParameterNames;

            // validate that each parameter attribute refers to an actual parameter of the documented delegate
            foreach (var p in parameters)
            {
                if (!correctlyOrderedParameterNames.Contains(p.Name))
                    warningsBuilder.Append(string.Format("The parameter '{0}' documented as part of delegate {1} was exluded because {1} defines no such parameter.\n", p.Name, type.Name));
            }

            return parameters
                .Where(p => !string.IsNullOrEmpty(p.Name) && !string.IsNullOrEmpty(p.Description))
                .Where(p => correctlyOrderedParameterNames.Contains(p.Name))
                .OrderBy(p => p.Name, parameterOrderComparer);
        }

        private MemberDocumentation(SummaryAttribute summary,
            IEnumerable<TypeParameterAttribute> typeParameters,
            IEnumerable<ParameterAttribute> parameters,
            ReturnsAttribute returns,
            IEnumerable<ExceptionAttribute> exceptions,
            IEnumerable<RemarksAttribute> remarks,
            IEnumerable<CodeExampleAttribute> examples,
            IEnumerable<SeeAlsoAttribute> seeAlsos,
            string warnings)
        {
            Summary = summary;
            TypeParameters = typeParameters;
            Parameters = parameters;
            Returns = returns;
            Exceptions = exceptions;
            Remarks = remarks;
            Examples = examples;
            SeeAlsos = seeAlsos;
            Warnings = warnings;
        }
    }
}