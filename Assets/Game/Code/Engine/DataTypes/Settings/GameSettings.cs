using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public class GameSettings : Jape.GameSettings
    {
        public Level startingLevel;

        [HideInInspector, NonSerialized]
        public Action<Difficulty> onDifficultyChange = delegate {};

        [SerializeField, HideInInspector]
        private Difficulty difficulty = Difficulty.Normal;

        [ShowInInspector]
        public Difficulty Difficulty
        {
            get => difficulty;
            set
            {
                if (difficulty == value) { return; }
                difficulty = value;
                onDifficultyChange(difficulty);
            }
        }
    }
}