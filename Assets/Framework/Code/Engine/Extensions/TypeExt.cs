using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Jape
{
	public static class TypeExt
    {
        public static object GetDefaultValue(this Type type)
        {
            return type.IsValueType ? 
                   Activator.CreateInstance(type) : 
                   null;
        }

        public static IEnumerable<Type> GetSubclass(this Type type, bool includeBase = false, bool includeAbstract = false, bool includeGeneric = true)
        {
            return GetSubclass(type, Assemblies.GetJapeRuntime(), includeBase, includeAbstract, includeGeneric);
        }

        public static IEnumerable<Type> GetSubclass(this Type type, IEnumerable<Assembly> assemblies, bool includeBase = false, bool includeAbstract = false, bool includeGeneric = true)
        {
            List<Type> classes = new();

            foreach (Assembly assembly in assemblies)
            {
                classes.AddRange(assembly.GetTypes().Where(t => t.IsBaseOrSubclassOf(type) || t.IsGenericSubclassOf(type)));
            }

            if (!includeBase) { classes = classes.Where(t => t != type).ToList(); }
            if (!includeAbstract) { classes = classes.Where(t => !t.IsAbstract).ToList(); }
            if (!includeGeneric) { classes = classes.Where(t => !t.IsGenericTypeDefinition).ToList(); }

            return classes;
        }

        public static bool IsBaseOrSubclassOf(this Type child, Type parent)
        {
            if (parent == null || child == null) { return false; }
            if (parent == child) { return true; }
            return child.IsSubclassOf(parent);
        }

        public static bool IsGenericSubclassOf(this Type child, Type parent) 
        {
            if (parent == null || child == null) { return false; }
            if (parent == child) { return false; }
            if (!parent.IsGenericType) { return false; }

            while (child != null && child != typeof(object)) 
            {
                var cur = child.IsGenericType ? child.GetGenericTypeDefinition() : child;

                if (parent == cur) { return true; }

                child = child.BaseType;
            }

            return false;
        }

        public static string CleanName(this Type type) { return type.Name.RemoveNamespace().RemoveSuffix().RemoveBrackets(); }
    }
}