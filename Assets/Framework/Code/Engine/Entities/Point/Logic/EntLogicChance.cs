using System;
using UnityEngine;

namespace Jape
{
    [AddComponentMenu("")]
    public class EntLogicChance : EntLogic
    {
        protected override Texture Icon => GetIcon("IconRandom");

        [SerializeField] private Probability chance = 0;

        public override Enum Outputs() { return ChanceOutputsFlags.OnChance; }

        [Route]
        public void Roll() { if (Random.Chance(chance)) { Launch(ChanceOutputs.OnChance); }}

        [Route]
        public void SetChance(float chance) { this.chance = chance; }
    }
}