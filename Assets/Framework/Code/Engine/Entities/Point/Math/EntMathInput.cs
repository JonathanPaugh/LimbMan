using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Jape
{
    [AddComponentMenu("")]
    public class EntMathInput : EntMath
    {
        protected override Texture Icon => GetIcon("IconInput");

        protected override object Value { get; set; }

        [PropertySpace(8)]

        [SerializeField]
        [InlineProperty]
        [HideLabel]
        private Input.Action action = new Input.Action(null);

        [SerializeField, HideInInspector]
        private bool active;

        public override Enum BaseOutputs() { return BaseOutputsFlags.None | 
                                                    BaseOutputsFlags.OnGet |
                                                    BaseOutputsFlags.OnTrigger | 
                                                    BaseOutputsFlags.OnDestroy |
                                                    BaseOutputsFlags.OnLaunch1 |
                                                    BaseOutputsFlags.OnLaunch2 |
                                                    BaseOutputsFlags.OnLaunch3 |
                                                    BaseOutputsFlags.OnLaunch4; }

        public override Enum Outputs()
        {
            return InputOutputsFlags.OnPress | 
                   InputOutputsFlags.OnRelease |
                   InputOutputsFlags.OnHoldFrame |
                   InputOutputsFlags.OnHoldTick;
        }

        public override IEnumerable<Send> Sends()
        {
            return new []
            {
                new Send(InputOutputs.OnPress, typeof(object), "Value"),
                new Send(InputOutputs.OnHoldFrame, typeof(object), "Value"),
                new Send(InputOutputs.OnHoldTick, typeof(object), "Value")
            };
        }

        private object Stream()
        {
            object value = action.Stream();
            switch (value)
            {
                case null: return null;
                case bool _: return Convert.ToInt32(value);
                default: return value;
            }
        }

        protected override void Init() { action.Input.started += OnInput; }
        protected override void Destroyed() { action.Input.started -= OnInput; }

        protected override void Enabled()
        {
            action.Enable();
        }

        protected override void Disabled()
        {
            LaunchValue(null);
            OnRelease();
            action.Disable();
        }

        protected override void Frame()
        {
            Value = Stream();
            OnHold(InputOutputs.OnHoldFrame);
            OnRelease();
        }

        protected override void Tick()
        {
            OnHold(InputOutputs.OnHoldTick);
        }

        private void OnInput(InputAction.CallbackContext input) { OnPress(); }

        private void OnPress()
        {
            if (!active)
            {
                active = true;
                LaunchValue(Value);
                Press();
            } 
        }

        private void OnRelease()
        {
            if (active)
            {
                if (Value == null)
                {
                    active = false;
                    LaunchValue(Value);
                    Release();
                }
            }
        }

        private void OnHold(Enum output)
        {
            if (active)
            {
                if (Value != null)
                {
                    LaunchValue(Value);
                    Launch(output, Value);
                }
            }
        }

        /// <summary>
        /// Called when first binding is pressed
        /// </summary>
        private void Press() { Launch(InputOutputs.OnPress, Value); }

        /// <summary>
        /// Called when all bindings are released
        /// </summary>
        private void Release() { Launch(InputOutputs.OnRelease); }
    }
}