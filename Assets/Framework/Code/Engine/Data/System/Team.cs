using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    public class Team : SystemData
    {
        protected new static string Path => IO.JoinPath(SystemPath, "Teams");

        [SerializeField]
        [ListDrawerSettings(CustomAddFunction = nameof(ClickAddPlayer))]
        private HashSet<int> players = new();

        private int ClickAddPlayer()
        {
            int player = 0;
            while (players.Contains(player))
            {
                player++;
            }
            return player;
        }

        public void AddPlayer(int player) => players.Add(player);
        public void RemovePlayer(int player) => players.Remove(player);
        public bool HasPlayer(int player) => players.Contains(player);

        public static Team Find(string name) { return Find<Team>(name); }
        public static Team[] FindPlayer(int player) { return FindAll<Team>().Where(t => t.HasPlayer(player)).ToArray(); }
    }
}