using Jape;
using UnityEngine;

namespace JapeNet
{
    public class ClientSide : Element, IEntityComponent
    {
        [SerializeField] private Component[] clientComponents = null;

        internal override void Awake()
        {
            if (Game.IsRunning)
            {
                if (NetManager.GetMode().IsServerOnly)
                {
                    for (int i = clientComponents.Length - 1; i >= 0; i--)
                    {
                        DestroyImmediate(clientComponents[i]);
                    }
                }

                Destroy(this);
            }

            base.Awake();
        }
    }
}