using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    [Serializable]
	public class Arg
    {
        protected virtual bool showPrimitives => true;

        protected virtual object defaultValue { get; set; }
        protected virtual Type defaultType { get; set; }

        public Type Type => defaultType;

        [SerializeField]
        [ShowIf(nameof(showPrimitives))]
        private Primitive type;

        [SerializeField]
        [LabelText("Value")]
        [ShowIf(nameof(IsString))]
        protected string stringValue;

        [SerializeField]
        [LabelText("Value")]
        [ShowIf(nameof(IsBool))]
        protected bool boolValue;

        [SerializeField]
        [LabelText("Value")]
        [ShowIf(nameof(IsFloat))]
        protected float floatValue;

        [SerializeField]
        [LabelText("Value")]
        [ShowIf(nameof(IsInt))]
        protected int intValue;

        public object Value
        {
            get
            {
                if (IsString()) { return stringValue; }
                if (IsBool()) { return boolValue; }
                if (IsFloat()) { return floatValue; }
                if (IsInt()) { return intValue; }
                return defaultValue;
            }
            set
            {
                stringValue = default;
                boolValue = default;
                floatValue = default;
                stringValue = default;
                defaultValue = default;

                defaultType = value.GetType();

                if (IsString())
                {
                    type = Primitive.String;
                    stringValue = (string)value; return;
                }

                if (IsBool())
                {
                    type = Primitive.Bool;
                    boolValue = (bool)value; return;
                }

                if (IsFloat())
                {
                    type = Primitive.Float;
                    floatValue = (float)value; return;
                }

                if (IsInt())
                {
                    type = Primitive.Int;
                    intValue = (int)value; return;
                }

                defaultValue = value;
            }
        }

        public Arg() {}
        public Arg(object @default) { Value = @default; }

        public bool IsSet() { return defaultType != null; }

        protected virtual bool IsString() { return type == Primitive.String; }
        protected virtual bool IsBool() { return type == Primitive.Bool; }
        protected virtual bool IsFloat() { return type == Primitive.Float; }
        protected virtual bool IsInt() { return type == Primitive.Int; }
    }
}