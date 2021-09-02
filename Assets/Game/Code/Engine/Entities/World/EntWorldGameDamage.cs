using System;
using UnityEngine;
using Jape;

namespace Game
{
    [AddComponentMenu("")]
    public class EntWorldGameDamage : EntWorldDamage
    {
        public override Enum Outputs() { return null; }

        [Space(8)]
        
        [SerializeField]
        [Eject]
        private Damage damage = new Damage();

        protected override Jape.Damage Damage => damage;
    }
}