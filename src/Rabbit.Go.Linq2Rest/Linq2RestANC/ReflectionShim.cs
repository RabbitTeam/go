using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Linq2Rest
{
    internal static class ReflectionShim
    {
        public static bool IsGenericType(this Type t) => t.GetTypeInfo().IsGenericType;
        public static bool IsInterface(this Type t) => t.GetTypeInfo().IsInterface;
        public static bool IsEnum(this Type t) => t.GetTypeInfo().IsEnum;
        public static TypeAttributes Attributes(this Type t) => t.GetTypeInfo().Attributes;
        public static PropertyInfo GetProperty(this Type t, string name) => t.GetTypeInfo().GetProperty(name);
        public static PropertyInfo[] GetProperties(this Type t) => t.GetTypeInfo().GetProperties();
        public static MethodInfo GetMethod(this Type t, string name) => t.GetTypeInfo().GetMethod(name);
        public static MethodInfo[] GetMethods(this Type t) => t.GetTypeInfo().GetMethods();
        public static T GetCustomAttribute<T>(this Type t) where T : Attribute => t.GetTypeInfo().GetCustomAttribute<T>();
        public static IEnumerable<Attribute> GetCustomAttributes(this Type t, Type attributeType, bool inherit) => t.GetTypeInfo().GetCustomAttributes(attributeType, inherit).Cast<Attribute>();
        public static IEnumerable<Attribute> GetCustomAttributes(this Type t, bool inherit) => t.GetCustomAttributes(inherit).Cast<Attribute>();
        public static Type[] FindInterfaces(this Type t, TypeFilter filter, object filterCriteria) => t.GetTypeInfo().FindInterfaces(filter, filterCriteria);
        public static Type[] GetGenericArguments(this Type t) => t.GetTypeInfo().GetGenericArguments();
        public static IList<CustomAttributeData> GetCustomAttributesData(this Type t) => CustomAttributeData.GetCustomAttributes(t.GetTypeInfo());
        public static IList<CustomAttributeData> GetCustomAttributesData(this MemberInfo mi) => CustomAttributeData.GetCustomAttributes(mi);
        public static bool IsAssignableFrom(this Type t, Type from) => t.GetTypeInfo().IsAssignableFrom(from);
    }
}
