using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace Jape
{
    public static class DrawerEditor
    {
        #if UNITY_EDITOR

        private static Dictionary<Type, PropertyDrawer> drawers = new();
 
        public static PropertyDrawer Find(SerializedProperty property)
        {
            Type type = GetPropertyType(property);
            if (!drawers.ContainsKey(type)) { drawers.Add(type, Find(type)); }
            return drawers[type];
        }
        
        public static PropertyDrawer Find(Type propertyType)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type candidate in assembly.GetTypes())
                {
                    FieldInfo typeField = typeof(CustomPropertyDrawer).GetField("m_Type", BindingFlags.NonPublic | BindingFlags.Instance);
                    FieldInfo childField = typeof(CustomPropertyDrawer).GetField("m_UseForChildren", BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (Attribute attribute in candidate.GetCustomAttributes(typeof(CustomPropertyDrawer)))
                    {
                        if (attribute.GetType().IsSubclassOf(typeof(CustomPropertyDrawer)) || attribute is CustomPropertyDrawer)
                        {
                            CustomPropertyDrawer drawerAttribute = (CustomPropertyDrawer)attribute;
                            Type drawerType =  (Type) typeField.GetValue(drawerAttribute);
                            if (drawerType == propertyType ||
                               (bool)childField.GetValue(drawerAttribute) && propertyType.IsSubclassOf(drawerType) ||
                               (bool)childField.GetValue(drawerAttribute) && propertyType.IsGenericSubclassOf(drawerType))
                            {
                                if (candidate.IsSubclassOf(typeof(PropertyDrawer))) { return (PropertyDrawer)Activator.CreateInstance(candidate); }
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static Type GetPropertyType(SerializedProperty property)
        {
            Type type = property.serializedObject.targetObject.GetType();
            string[] path = property.propertyPath.Split('.');
            FieldInfo field = type.GetField(path[0]);
            for (int i = 1; i < path.Length; i++) { field = field.FieldType.GetField(path[i]); }
            return field.FieldType;
        }

        #endif
    }
}