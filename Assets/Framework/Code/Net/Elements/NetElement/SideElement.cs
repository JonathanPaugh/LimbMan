using System;
using System.Linq;
using Jape;
using UnityEngine;

namespace JapeNet
{
    public abstract class SideElement : NetElement
    {
        protected override Communication CommuncationMode => Communication.All;
        protected virtual NetMode Side => NetMode.Offline;
        protected virtual Type[] PairComponents => null;

        internal override void Awake()
        {
            if (Game.IsRunning)
            {
                if (Mode.IsOnline)
                {
                    if (!Mode.HasFlag(Side))
                    {
                        DestroyImmediate(this);
                        DestroyPairComponents();
                        return;
                    }
                }
            } 

            base.Awake();
        }

        protected void DestroyPairComponents()
        {
            if (PairComponents == null) { return; }
            foreach (Component component in PairComponents.Select(GetComponent).Where(c => c != null))
            {
                DestroyImmediate(component);
            }
        }
    }
}
