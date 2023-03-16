using System;
using UnityEngine;

namespace Jape
{
    [AddComponentMenu("")]
    public class EntLogicAuto : EntLogic
    {
        protected override Texture2D Icon => GetIcon("IconAuto");

        public override Enum Outputs() { return AutoOutputsFlags.OnInit |
                                                AutoOutputsFlags.OnFirst |
                                                AutoOutputsFlags.OnFrame |
                                                AutoOutputsFlags.OnTick; }

        protected override void Init() { Launch(AutoOutputs.OnInit); }
        protected override void First() { Launch(AutoOutputs.OnFirst); }
        protected override void Frame() { Launch(AutoOutputs.OnFrame); }
        protected override void Tick() { Launch(AutoOutputs.OnTick); }
    }
}