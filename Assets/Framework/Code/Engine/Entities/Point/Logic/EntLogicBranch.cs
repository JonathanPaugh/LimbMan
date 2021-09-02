using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    [AddComponentMenu("")]
    public class EntLogicBranch : EntLogic
    {
        protected override Texture Icon => GetIcon("IconBranch");

        public override Enum Outputs() { return BranchOutputsFlags.OnTrue |
                                                BranchOutputsFlags.OnFalse; }

        [SerializeField]
        [LabelText("Default")]
        private bool defaultValue = false;

        [Route]
        public void Evaluate(float input)
        {
            if (Mathf.Approximately(input, 0)) { Launch(BranchOutputs.OnFalse); return; }
            if (Mathf.Approximately(input, 1)) { Launch(BranchOutputs.OnTrue); return; }
            switch (defaultValue)
            {
                case false: Launch(BranchOutputs.OnFalse); break;
                case true: Launch(BranchOutputs.OnTrue); break;
            }
        }
    }
}