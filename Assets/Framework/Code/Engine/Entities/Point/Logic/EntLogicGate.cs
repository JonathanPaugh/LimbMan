using System;
using UnityEngine;

namespace Jape
{
    [AddComponentMenu("")]
    public class EntLogicGate : EntLogic
    {
        protected override Texture2D Icon => GetIcon("IconGate");

        public override Enum Outputs() { return GateOutputsFlags.OnSuccess |
                                                GateOutputsFlags.OnFail; }
        
        [SerializeField] private float value;
        [SerializeField] private Condition condition;

        public enum Condition { Equal, NotEqual, Less, More, LessOrEqual, MoreOrEqual }

        [Route]
        public void Evaluate(float input)
        {
            switch (condition)
            {
                case Condition.Equal:
                    if (Mathf.Approximately(input, value)) { Launch(GateOutputs.OnSuccess); return; }
                    Launch(GateOutputs.OnFail); 
                    return;

                case Condition.NotEqual:
                    if (!Mathf.Approximately(input, value)) { Launch(GateOutputs.OnSuccess); return; }
                    Launch(GateOutputs.OnFail); 
                    return;

                case Condition.Less:
                    if (input < value) { Launch(GateOutputs.OnSuccess); return; }
                    Launch(GateOutputs.OnFail); 
                    return;

                case Condition.More:
                    if (input > value) { Launch(GateOutputs.OnSuccess); return; }
                    Launch(GateOutputs.OnFail); 
                    return;

                case Condition.LessOrEqual:
                    if (input <= value) { Launch(GateOutputs.OnSuccess); return; }
                    Launch(GateOutputs.OnFail); 
                    return;

                case Condition.MoreOrEqual:
                    if (input >= value) { Launch(GateOutputs.OnSuccess); return; }
                    Launch(GateOutputs.OnFail); 
                    return;
            }
        }

        [Route]
        public void SetValue(float input) { value = input; }

        [Route]
        public void SetCondition(string condition) { this.condition = (Condition)Enum.Parse(typeof(Condition), condition, true); }
    }
}