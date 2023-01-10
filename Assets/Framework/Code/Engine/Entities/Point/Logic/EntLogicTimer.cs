using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    [AddComponentMenu("")]
    public class EntLogicTimer : EntLogic
    {
        protected override Texture Icon => GetIcon("IconTimer");

        [SerializeField] public bool autoStart = false;

        [Space(8)]

        [SerializeField] 
        protected Timer.Mode mode;

        [Space(16)]

        [SerializeField]
        [Eject]
        protected Timer.Settings settings;

        [ShowInInspector]
        [ShowIf(nameof(IsSet))]
        [ShowIf(Game.GameIsRunning)]
        private float Remaining => IsSet() ? timer.TimeRemaining : 0;

        private Timer timer;

        public override Enum Outputs() { return TimerOutputsFlags.OnTimer; }

        private bool IsSet() => timer != null;

        protected override void Activated()
        {
            timer = CreateTimer();
            timer.CompletedAction(() =>
            {
                Launch(TimerOutputs.OnTimer);
                if (mode == Timer.Mode.Loop) { TimerStart(); }
            });
        }

        protected override void Init() { if (autoStart) { TimerStart(); }}

        [Route]
        public void TimerStart() { timer.Set(settings.timeTotal, settings.interval.Counter, settings.interval.Value()).Start(); }

        [Route]
        public void TimerStop() { timer.Stop(); }

        [Route]
        public void TimerPause() { timer.Pause(); }

        [Route]
        public void TimerResume() { timer.Resume(); }

        [Route]
        public void TimerSingle() { timer.ChangeMode(Timer.Mode.Single); }

        [Route]
        public void TimerLoop() { timer.ChangeMode(Timer.Mode.Loop); }

        [Route]
        public void TimerSetMax(float max) { settings.timeTotal = max; }

        [Route]
        public void TimerSetRemaining(float remaining) { timer.SetTimeRemaining(remaining); }
    }
}