using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Jape
{
    public class Member
    {
        public const BindingFlags Bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        public MemberInfo Target { get; private set; }
        public object Instance { get; private set; }

        private Type instanceType;
        public Type InstanceType
        {
            get
            {
                if (Instance == null) { return instanceType; }
                return Instance.GetType();
            }
        }

        private Member() {} 

        public Member(object instance, string target)
        {
            Instance = instance;
            Target = FindTarget(target);
        }

        public Member(object instance, string target, Func<MemberInfo[], MemberInfo> solver)
        {
            Instance = instance;
            Target = FindTarget(target, false, solver);
        }

        public object Get(params object[] args)
        {
            if (Target == null) { Response($"Unable to find target: {Target}"); return null; }

            switch (Target.MemberType)
            {
                case MemberTypes.Field: 
                    FieldInfo field = (FieldInfo)Target;
                    return field.GetValue(Instance);

                case MemberTypes.Property:
                    PropertyInfo property = (PropertyInfo)Target;
                    return property.GetValue(Instance);

                case MemberTypes.Method: 
                    MethodInfo method = (MethodInfo)Target;
                    return method.Invoke(Instance, args);

                default: 
                    Response($"Invalid member type: {Target.MemberType}"); 
                    return null;
            }
        }

        public void Set(object value)
        {   
            if (Target == null) { Response($"Unable to find target: {Target}"); return; }

            switch (Target.MemberType)
            {
                case MemberTypes.Field: 
                    FieldInfo field = (FieldInfo)Target;
                    field.SetValue(Instance, value);
                    return;

                case MemberTypes.Property:
                    PropertyInfo property = (PropertyInfo)Target;
                    property.SetValue(Instance, value);
                    return;

                case MemberTypes.Method: 
                    Response("Setting a method is not possible");
                    return;

                default: 
                    Response($"Invalid member type: {Target.MemberType}");
                    return;
            }
        }

        public Type Type()
        {
            if (Target == null) { Response($"Unable to find target: {Target}"); return null; }

            switch (Target.MemberType)
            {
                case MemberTypes.Field: 
                    FieldInfo field = (FieldInfo)Target;
                    return field.FieldType;

                case MemberTypes.Property:
                    PropertyInfo property = (PropertyInfo)Target;
                    return property.PropertyType;

                case MemberTypes.Method: 
                    MethodInfo method = (MethodInfo)Target;
                    return method.ReturnType;

                default: 
                    Response($"Invalid member type: {Target.MemberType}");
                    return null;
            }
        }

        public static object Get(object instance, string target, Func<MemberInfo[], MemberInfo> solver = null, params object[] args)
        {
            return new Member(instance, target, solver).Get(args);
        }

        public static void Set(object instance, string target, object value, Func<MemberInfo[], MemberInfo> solver = null)
        {
            new Member(instance, target, solver).Set(value);
        }

        private MemberInfo FindTarget(string target, bool trueInstance = false, Func<MemberInfo[], MemberInfo> solver = null)
        {
            if (string.IsNullOrEmpty(target)) { Response("Member target is empty"); return null; }

            MemberInfo[] members = null;

            instanceType = null;
            Target = null;

            string[] instanceTargets = target.Split('.');

            foreach (string instanceTarget in instanceTargets)
            {
                if (Target != null)
                {
                    trueInstance = false;
                    Instance = Get();
                }

                Type instance = trueInstance ? (Type)Instance : Instance.GetType();

                members = instance.GetMember(instanceTarget, MemberTypes.Field | MemberTypes.Property | MemberTypes.Method, Bindings);

                if (members.Length == 0) { Response($"Unable to find member {instanceTarget} in {Instance}"); return null; }
                if (members.Length > 1) 
                { 
                    if (solver == null || solver(members) == null)
                    {
                        Response($"Found ambiguous matches for member {instanceTarget} in {Instance}"); 
                        Log.Write(instanceTargets); 
                        return null;
                    }
                    members = new [] { solver(members) };
                }

                Target = members.First();
            }

            return members.First();
        }

        public static Member Static(Assembly assembly, string instanceName, string target, Func<Type[], Type> classSolver = null, Func<MemberInfo[], MemberInfo> targetSolver = null)
        {
            return Static(Class(assembly, instanceName, classSolver), target, targetSolver);
        }

        public static Member Static(Type type, string target, Func<MemberInfo[], MemberInfo> targetSolver = null)
        {
            Member member = new()
            {
                Instance = type
            };

            member.Target = member.FindTarget(target, true, targetSolver);
            member.instanceType = member.Instance.GetType();
            member.Instance = null;
            
            return member;
        }

        public static Member StaticDeep(Assembly assembly, string instanceName, string target, Func<Type[], Type> classSolver = null, Func<MemberInfo[], MemberInfo> targetSolver = null)
        {
            return StaticDeep(Class(assembly, instanceName, classSolver), target, targetSolver);
        }

        public static Member StaticDeep(Type type, string target, Func<MemberInfo[], MemberInfo> targetSolver = null)
        {
            LogOff();
            Member member = new(null, null);
            Type instance = type;
            while (member.Target == null)
            {
                member = Static(instance, target, targetSolver);
                if (instance.BaseType != null) { instance = instance.BaseType; }
                else { return member; }
            }
            LogOn();
            return member;
        }

        public static Type Class(Assembly assembly, string instanceName, Func<Type[], Type> solver = null)
        {
            Type[] instances = assembly.GetTypes().Where(t => t.Name == instanceName).ToArray();

            if (instances.Length == 0)
            {
                if (!log) { return null; } 
                Log.Write($"Unable to find {instanceName} in {assembly}"); 
                return null;
            }
            if (instances.Length > 1)
            {
                if (solver == null || solver(instances) == null)
                {
                    if (!log) { return null; }
                    Log.Write($"Found ambiguous matches for {instanceName} in {assembly}"); 
                    Log.Write(instances); 
                    return null;
                }
                instances = new[] { solver(instances) };
            }
            return instances.First();
        }

        public static IEnumerable<MemberInfo> AllMembers(Type type) { return type.GetFields(Bindings).Cast<MemberInfo>().Concat(type.GetProperties(Bindings)); }

        private static bool log = true;

        public static void LogOn() { log = true; }
        public static void LogOff() { log = false; }

        private object Response(object line) { return !log ? null : this.Log().Response(line); }
    }
}