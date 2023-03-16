using Jape;
using UnityEngine;

namespace JapeNet
{
    public class ServerSide : Element, IEntityComponent
    {
        [SerializeField] private Component[] serverComponents = null;

        internal override void Awake()
        {
            if (Game.IsRunning)
            {
                if (NetManager.GetMode().IsClientOnly)
                {
                    for (int i = serverComponents.Length - 1; i >= 0; i--)
                    {
                        DestroyImmediate(serverComponents[i]);
                    }
                }

                Destroy(this);
            }

            base.Awake();
        }
    }
}