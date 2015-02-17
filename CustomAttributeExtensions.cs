using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSharpDocumentation
{
    public static class CustomAttributeExtensions
    {
        #region Assembly
        public static T GetCustomAttribute<T>(this Assembly assembly) where T : Attribute
        {
            return GetCustomAttribute<T>(assembly, true);
        }

        public static T GetCustomAttribute<T>(this Assembly assembly, bool inherit) where T : Attribute
        {
            var attributes = GetCustomAttributes<T>(assembly, inherit);
            return attributes.FirstOrDefault();
        }

        public static IEnumerable<T> GetCustomAttributes<T>(this Assembly assembly) where T : Attribute
        {
            return GetCustomAttributes<T>(assembly, true);
        }

        public static IEnumerable<T> GetCustomAttributes<T>(this Assembly assembly, bool inherit) where T : Attribute
        {
            return (T[]) assembly.GetCustomAttributes(typeof(T), inherit);
        }
        #endregion

        #region MemberInfo
        public static T GetCustomAttribute<T>(this MemberInfo member) where T : Attribute
        {
            return GetCustomAttribute<T>(member, true);
        }

        public static T GetCustomAttribute<T>(this MemberInfo member, bool inherit) where T : Attribute
        {
            var attributes = GetCustomAttributes<T>(member, inherit);
            return attributes.FirstOrDefault();
        }

        public static IEnumerable<T> GetCustomAttributes<T>(this MemberInfo member) where T : Attribute
        {
            return GetCustomAttributes<T>(member, true);
        }

        public static IEnumerable<T> GetCustomAttributes<T>(this MemberInfo member, bool inherit) where T : Attribute
        {
            return (T[]) member.GetCustomAttributes(typeof(T), inherit);
        }
        #endregion
    }
}
