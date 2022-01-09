using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Jape;

namespace Game
{
    public class DifficultyFilter : Element
    {
        private enum Mode { Include, Exclude }

        [SerializeField]
        private List<Difficulty> difficulties = new List<Difficulty>();

        [SerializeField]
        private Mode mode = Mode.Include;

        protected override void Activated()
        {
            if (!Game.IsRunning) { return; } 
            
            GameSettings gameSettings = Game.Settings<GameSettings>();

            Difficulty difficultySetting = gameSettings.Difficulty;
            Change(difficultySetting);

            gameSettings.onDifficultyChange += Change;
        }

        protected override void Destroyed()
        {
            GameSettings gameSettings = Game.Settings<GameSettings>();

            gameSettings.onDifficultyChange -= Change;
        }

        private void Change(Difficulty difficulty)
        {
            switch (mode)
            {
                case Mode.Include: gameObject.SetActive(difficulties.Any(d => d == difficulty)); break;
                case Mode.Exclude: gameObject.SetActive(difficulties.All(d => d != difficulty)); break;
            }
        }
    }
}