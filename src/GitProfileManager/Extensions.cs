using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GitProfileManager
{
    public static class Extensions
    {
        public static string ToConfig(this string s) {
            return string.Join(Environment.NewLine, s.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries));
        }

        public static IEnumerable<string> ToConfig(this IEnumerable<string> ss) {
            return ss
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Where(l => !l.StartsWith("#"))
                .Select(l => l.Trim())
                .Select(l => {
                    if (l.StartsWith("git ")) {
                        // Console.WriteLine("removing git");
                        l = l.Replace("git", string.Empty).Trim();
                    }
                    return l;
                })
                .Select(l => {
                    if (l.StartsWith("config ")) {
                        //Console.WriteLine("removing config");
                        l = l.Replace("config ", string.Empty).Trim();
                    }
                    return l;
                })
                .Select(l => l.Trim());
        }

        public static bool IsAssignableTo(this Type type, Type otherType)
        {
            var typeInfo = type.GetTypeInfo();
            var otherTypeInfo = otherType.GetTypeInfo();

            if (otherTypeInfo.IsGenericTypeDefinition)
            {
                if (typeInfo.IsGenericTypeDefinition)
                {
                    return typeInfo.Equals(otherTypeInfo);
                }

                return typeInfo.IsAssignableToGenericTypeDefinition(otherTypeInfo);
            }

            return otherTypeInfo.IsAssignableFrom(typeInfo);
        }

        public static bool IsAssignableTo<TTarget>(this object o) {
            var otherTypeInfo = typeof(TTarget);
            var typeInfo = o.GetType();
            return typeInfo.IsAssignableTo(otherTypeInfo);
        }

        private static bool IsAssignableToGenericTypeDefinition(this TypeInfo typeInfo, TypeInfo genericTypeInfo)
        {
            var interfaceTypes = typeInfo.ImplementedInterfaces.Select(t => t.GetTypeInfo());

            foreach (var interfaceType in interfaceTypes)
            {
                if (interfaceType.IsGenericType)
                {
                    var typeDefinitionTypeInfo = interfaceType
                        .GetGenericTypeDefinition()
                        .GetTypeInfo();

                    if (typeDefinitionTypeInfo.Equals(genericTypeInfo))
                    {
                        return true;
                    }
                }
            }

            if (typeInfo.IsGenericType)
            {
                var typeDefinitionTypeInfo = typeInfo
                    .GetGenericTypeDefinition()
                    .GetTypeInfo();

                if (typeDefinitionTypeInfo.Equals(genericTypeInfo))
                {
                    return true;
                }
            }

            var baseTypeInfo = typeInfo.BaseType?.GetTypeInfo();

            if (baseTypeInfo == null)
            {
                return false;
            }

            return baseTypeInfo.IsAssignableToGenericTypeDefinition(genericTypeInfo);
        }
    }
}