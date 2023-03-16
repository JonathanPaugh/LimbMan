using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    public class Global : GameData
    {
        [SerializeField]
        [Eject]
        private Arg arg = null;

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