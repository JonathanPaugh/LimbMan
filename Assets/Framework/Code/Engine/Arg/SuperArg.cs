using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Jape
{
	public class SuperArg : Arg
    {
        [Flags]
        public enum Mode { Primitives = 1, Game = 2, Framework = 4, Unity = 16 }

        protected override object defaultValue { get => objectValue; }
        protected override Type defaultType { get => type; }

        protected override bool showPrimitives => false;

        [PropertyOrder(-1)]
        [SerializeField]
        [HideLabel]
        [EnumToggleButtons]
        [HideInInlineEditors]
        internal Mode mode = Mode.Primitives | Mode.Game | Mode.Framework;

        [PropertySpace(8)]

        [PropertyOrder(-1)]
        [NonSerialized, OdinSerialize, ShowInInspector]
        [ValueDropdown(nameof(Types))]
        [HideInInlineEditors]
        private Type type;

        [PropertySpace(16)]

        [NonSerialized, OdinSerialize, ShowInInspector]
        [HideIf(nameof(IsPrimitive))]
        [LabelText("Value")]
        [TypeFilter(nameof(ValueFilter))]
        private object objectValue;

        public SuperArg() {}
        public SuperArg(object @default) { Value = @default; }

        private IEnumerable<Type> ValueFilter() { return new [] { type }; }
        private IEnumerable<Type> Types()
        {
            List<Type> classes = new List<Type>();

            if (mode.HasFlag(Mode.Primitives)) { classes.AddRange(new [] { typeof(string), typeof(bool), typeof(float), typeof(int) }); }
            if (mode.HasFlag(Mode.Game)) { classes.AddRange(Filter(Assemblies.GameEngine)); }
            if (mode.HasFlag(Mode.Framework)) { classes.AddRange(Filter(Assemblies.FrameworkEngine)); }
            if (mode.HasFlag(Mode.Unity)) { classes.AddRange(Filter(Assemblies.UnityCore)); }

            return classes;

            IEnumerable<Type> Filter(Assembly assembly)
            {
                return assembly.
                    GetExportedTypes().
                    Where(t => !t.IsEnum).
                    Where(t => !t.IsAbstract).
                    Where(t => !t.IsGenericTypeDefinition).
                    Where(t => !t.IsSubclassOf(typeof(UnityEngine.Object)));
            }
        }

        private bool IsPrimitive()
        {
            if (IsString()) { return true; }
            if (IsBool()) { return true; }
            if (IsFloat()) { return true; }
            if (IsInt()) { return true; }
            return false;
        }

        protected override bool IsString() { return type == typeof(string); }
        protected override bool IsBool() { return type == typeof(bool); }
        protected override bool IsFloat() { return type == typeof(float); }
        protected override bool IsInt() { return type == typeof(int); }
    }
}