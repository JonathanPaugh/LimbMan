using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    [Serializable]
    public class Timer : JobDriven<Timer>
    {
        public enum Mode { Single, Loop };
        [SerializeField] private Mode mode = Mode.Single;

        [Space(16)]

        [SerializeField] 
        [HideLabel] 
        private Settings settings = new(0, Time.Counter.Seconds, -1);

        [Space(16)]

        [SerializeField] 
        [ReadOnly] 
        private float timeRemaining;
        public float TimeRemaining
        {
            get => timeRemaining;
            set
            {
                if (value < 0) { timeRemaining = 0; return; }
                timeRemaining = value;
            }
        }

        public float TimeTotal => settings.timeTotal;

        private Action<Timer> intervalAction;
        private Action<Timer> iterationAction;
        private Action<Timer> completeAction;
        private Action<Timer> processedAction;

        internal Timer() { Init(this); }

        protected override void Setup()
        {
            Job.onProcessed.Subscribe(Processed);
            Job.onComplete.Subscribe(Complete);
        }

        public float Progress() { return 1 - (timeRemaining / settings.timeTotal); }
        public float Inverse() { return timeRemaining / settings.timeTotal; }

        internal static void Delay(Timer timer, float time, Time.Counter counter, Action action)
        {
            timer.Set(time, counter, time).CompletedAction(action).Start();
        }

        public static void Delay(float time, Time.Counter counter, Action action)
        {
            Delay(Create(), time, counter, action);
        }

        public static void DelayGlobal(float time, Time.Counter counter, Action action)
        {
            Delay(CreateGlobal(), time, counter, action);
        }

        public override Timer ForceStart()
        {
            if (settings == null) { this.Log().Response("Cant start because timer is not set"); return this; }

            return base.ForceStart();
        }

        public Timer ForceStart(float time, Time.Counter counter = Time.Counter.Seconds, float interval = -1)
        {
            if (IsProcessing())
            {
                Stop();
            }

            settings = new Settings(time, counter, interval);
            
            return ForceStart();
        }

        public Timer Set(float time, Time.Counter counter = Time.Counter.Seconds, float interval = -1)
        {
            if (IsProcessing()) { this.Log().Response("Cant set timer while it is processing"); return this; }

            settings = new Settings(time, counter, interval);

            return this;
        }

        public Timer Restart()
        {
            if (IsProcessing())
            {
                return SetTimeRemaining(TimeTotal);
            }
            else
            {
                return Start();
            }
        }

        public Timer Finish()
        {
            if (!processing) { return this; }

            base.Stop();

            timeRemaining = 0;

            Processed();
            Complete();

            return this;
        }

        public Timer SetTimeRemaining(float time)
        {
            if (!IsProcessing()) { TimeRemainingWarning(); return this; }
            timeRemaining = time; 
            return this;
        }

        public Timer AddTimeRemaining(float time)
        {
            if (!IsProcessing()) { TimeRemainingWarning(); return this; }
            timeRemaining += time; return this;
        }

        public Timer SubtractTimeRemaining(float time)
        {
            if (!IsProcessing()) { TimeRemainingWarning(); return this; }
            timeRemaining -= time; return this;
        }

        private void TimeRemainingWarning() { this.Log().Response("Cant modify time remaining if timer is not processing"); }

        public Timer ChangeMode(Mode mode) { this.mode = mode; return this; }

        public Timer IntervalAction(Action<Timer> action) { intervalAction = action; return this; }
        public Timer IntervalAction(Action action) { return IntervalAction(_ => action()); }

        public Timer IterationAction(Action<Timer> action) { iterationAction = action; return this; }
        public Timer IterationAction(Action action) { return IterationAction(_ => action()); }

        public Timer CompletedAction(Action<Timer> action) { completeAction = action; return this; }
        public Timer CompletedAction(Action action) { return CompletedAction(_ => action()); }

        public Timer ProcessedAction(Action<Timer> action) { processedAction = action; return this; }
        public Timer ProcessedAction(Action action) { return ProcessedAction(_ => action()); }

        public Timer ClearActions()
        {
            intervalAction = null;
            iterationAction = null;
            completeAction = null;
            processedAction = null;
            return this;
        }

        protected sealed override IEnumerable Run()
        {
            do 
            {
                timeRemaining = settings.timeTotal;
                while (timeRemaining > 0) 
                {
                    if (AutoInterval())
                    {
                        yield return Wait.Frame();
                        switch (settings.interval.Counter)
                        {
                            case Time.Counter.Frames: timeRemaining -= 1; break;
                            case Time.Counter.Seconds: timeRemaining -= Time.FrameInterval(); break;
                            case Time.Counter.Realtime: throw new NotImplementedException();
                        }
                    }
                    else
                    {
                        switch (settings.interval.Counter)
                        {
                            case Time.Counter.Frames: yield return Wait.Frames((int)settings.interval.Value()); break;
                            case Time.Counter.Seconds: yield return Wait.Seconds(settings.interval.Value()); break;
                            case Time.Counter.Realtime: yield return Wait.Realtime(settings.interval.Value()); break;
                        }
                        timeRemaining -= settings.interval.Value();
                    }
                    intervalAction?.Invoke(this);
                } 

                timeRemaining = 0;

                if (!processing) { yield break; }

                Iteration(); 

            } while (mode == Mode.Loop);

            bool AutoInterval() { return Mathf.Approximately(settings.interval.Value(), -1); }
        }

        protected override void Iteration()
        {
            base.Iteration();
            iterationAction?.Invoke(this);
        }

        protected override void Complete()
        {
            if (complete) { return; }

            base.Complete();
            completeAction?.Invoke(this);
        }

        protected override void Processed()
        {
            if (!processing) { return; }

            base.Processed();
            processedAction?.Invoke(this);
        }

        [Serializable]
        public class Settings
        {
            public float timeTotal;

            [Space(8)]

            [HideLabel] public Time.Interval interval;

            public Settings(float time, Time.Counter counter, float interval)
            {
                timeTotal = time;
                this.interval = new Time.Interval(counter, interval);
            }
        }
    }
}