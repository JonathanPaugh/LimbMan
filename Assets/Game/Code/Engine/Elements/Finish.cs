using UnityEngine;
using Jape;

namespace Game
{
    public class Finish : Element
    {
        protected override void Touch(GameObject gameObject)
        {
            if (!gameObject.HasComponent<Player>(false)) { return; }
            this.gameObject.SetActive(false);
            GameManager.Finish();
        }
    }
}