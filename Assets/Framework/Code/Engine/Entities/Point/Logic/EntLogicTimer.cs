using System;
using System.Collections;
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

        private Timer timer;
        private Job job;

        public override Enum Outputs() { return TimerOutputsFlags.OnTimer; }

        protected override void Activated()
        {
            timer = CreateTimer().Set(settings.timeTotal, settings.interval.Counter, settings.interval.Value()).ChangeMode(mode);
            job = CreateJob().Set(TimerRoutine()).ChangeMode(Job.Mode.Loop).Start();
        }

        protected override void Init() { if (autoStart) { TimerStart(); }}

        [Route]
        public void TimerStart() { timer.Start(); }

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

        private IEnumerable TimerRoutine()
        {
            yield return timer.onIteration.Wait();
            Launch(TimerOutputs.OnTimer);
        }
    }
}