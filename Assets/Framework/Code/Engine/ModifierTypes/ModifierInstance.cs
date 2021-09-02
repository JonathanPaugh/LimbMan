using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Jape
{
	public abstract class ModifierInstance : BehaviourInstance
    {
        public abstract void Destroy();

        protected abstract void OnInflicted();
        protected abstract void OnDestroyed();
    }
}