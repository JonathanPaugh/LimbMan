using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    [AddComponentMenu("")]
    public abstract class EntMath : PointEntity
    {
        protected override Texture Icon => GetIcon("IconMath");

        [SerializeField, HideInInspector] protected float floatValue;
        [SerializeField, HideInInspector] protected int intValue;

        [PropertyOrder(-1)]
        [ShowInInspector]
        [HideReferenceObjectPicker]
        protected abstract object Value { get; set; }

        public override Enum BaseOutputs() { return BaseOutputsFlags.None | 
                                                    BaseOutputsFlags.OnValue |
                                                    BaseOutputsFlags.OnGet |
                                                    BaseOutputsFlags.OnTrigger | 
                                                    BaseOutputsFlags.OnDestroy |
                                                    BaseOutputsFlags.OnLaunch1 |
                                                    BaseOutputsFlags.OnLaunch2 |
                                                    BaseOutputsFlags.OnLaunch3 |
                                                    BaseOutputsFlags.OnLaunch4; }

        public override IEnumerable<Send> Sends() 
        { 
            return new []
            {
                new Send(Jape.BaseOutputs.OnValue, typeof(object), "Value"),
                new Send(Jape.BaseOutputs.OnGet, typeof(object), "Value"),
            };
        }

        [Route]
        public void Get() { Launch(Jape.BaseOutputs.OnGet, Value); }

        protected void LaunchValue(object value)
        {
            Value = value; 
            Launch(Jape.BaseOutputs.OnValue, Value);
        }
    }
}