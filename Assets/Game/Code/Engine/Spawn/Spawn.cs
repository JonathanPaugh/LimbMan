using Game;
using Jape;
using UnityEngine;

namespace Game
{
    public class Spawn : Element
    {
        public bool primary;

        protected override void Activated()
        {
            if (gameObject.HasTag(Tag.Find("GameStart"))) { return; }
            if (Game.Settings<GameSettings>().Difficulty != Difficulty.Hardcore) { return; }

            Destroy(gameObject);
        }

        protected override void Touch(GameObject gameObject)
        {
            if (!gameObject.HasTag(Tag.Find("Player"))) { return; }
            UpdateSpawn();
        }

        private void UpdateSpawn()
        {
            Jape.Status status = new Jape.Status
            {
                Key = "Spawn"
            };

            status.Write("Id", gameObject.Identifier());
            status.Write("Scene", gameObject.scene.path);

            Status.Save(status);

            Jape.Game.Save();
        }

        public Vector3 SpawnPosition()
        {
            return transform.position + new Vector3(0, 2, 0);
        }
    }
}