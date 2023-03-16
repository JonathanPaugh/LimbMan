using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    [AddComponentMenu("")]
    public class EntLogicAlternator : EntLogic
    {
        protected override Texture2D Icon => GetIcon("IconAlternator");

        public override Enum Outputs() { return BranchOutputsFlags.OnTrue |
                                                BranchOutputsFlags.OnFalse; }

        [SerializeField]
        [DisableIf(Game.GameIsRunning)]
        private bool value;

        [Route]
        public void Next()
        {
            value = !value;
            switch (value)
            {
                case true: Launch(BranchOutputsFlags.OnTrue); break;
                case false: Launch(BranchOutputsFlags.OnFalse); break;
            }
        }
    }
}