using System;
using System.Collections.Generic;
using Jape;

namespace JapeNet
{
	public abstract class NetShell : GameShell
    {
        protected sealed override void OnBuildInit() => base.OnGameInit();
        protected sealed override void OnGameInit() => base.OnGameInit();

        protected sealed override void OnGameSave() => base.OnGameSave();
        protected sealed override void OnGameLoad() => base.OnGameLoad();

        public abstract Dictionary<string, Action<object[]>> Delegates { get; }
    }
}