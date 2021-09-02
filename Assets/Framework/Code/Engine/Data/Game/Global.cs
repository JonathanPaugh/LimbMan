using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;

namespace Jape
{
    public class Global : GameData
    {
        private SuperArg arg;

        [SerializeField]
        [HideLabel, HideReferenceObjectPicker, HideInPlayMode]
        protected SuperArg Arg
        {
            get { return arg ?? (arg = new SuperArg()); }
            set { arg = value; }
        }

        [HideInEditorMode]
        [ShowInInspector]
        public Type Type => arg.Type;

        [Space(8)]

        [PropertyOrder(2)]
        [ShowInInspector]
        [HideInEditorMode]
        [HideReferenceObjectPicker]
        private object value;

        public bool IsSet() { return arg.IsSet(); }

        public object GetValue() { return value; }
        public void SetValue(object value) { this.value = value; }

        public static void InitAll()
        {
            foreach (Global global in FindAll<Global>())
            {
                global.value = global.arg.Value;
            }
        }
    }
}