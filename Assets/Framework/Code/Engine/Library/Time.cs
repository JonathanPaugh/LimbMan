using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
	public static class Time
    {
        private static TimeSettings settings;
        public static TimeSettings Settings => settings ?? (settings = Game.Settings<TimeSettings>());

        public static int tickCount;

        public static int FrameRate() { return Settings.FrameRate; }
        public static int FrameCount() { return UnityEngine.Time.frameCount; }
        public static float FrameInterval() { return UnityEngine.Time.deltaTime; }

        public static int TickRate() { return Settings.TickRate; }
        public static int TickCount() { return tickCount; }
        public static float TickInterval() { return UnityEngine.Time.fixedDeltaTime; }

        public static float RealtimeCount() { return UnityEngine.Time.realtimeSinceStartup; }

        public static int Unix() { return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds; }

        public static float ConvertRate(int rate) { return 1f / rate; }
        public static int ConvertInterval(float interval) { return ((int)(1f / interval)).RoundSpecial(); }

        public enum Counter { Frames, Seconds, Realtime }

        [Serializable]
        public class Interval
        {
            [SerializeField, HideInInspector] private Counter counter = Counter.Seconds;

            [PropertyOrder(0)] [ShowInInspector] public Counter Counter
            {
                get { return counter; }
                set
                {
                    floatValue = 0;
                    intValue = 0;
                    counter = value;
                }
            }

            [SerializeField] [PropertyOrder(1)] [LabelText("Interval")] [ShowIf(nameof(UseFloat))] private float floatValue = -1;
            [SerializeField] [PropertyOrder(1)] [LabelText("Interval")] [ShowIf(nameof(UseInt))] private int intValue = -1;

            public Interval() {}
            public Interval(Counter counter, float value)
            {
                Counter = counter;
                if (UseFloat()) { floatValue = value; }
                if (UseInt()) { intValue = (int)value; }
            }

            public float Value()
            {
                if (UseFloat()) { return floatValue; }
                if (UseInt()) { return intValue; }
                throw new Exception();

            }

            private bool UseFloat() { return counter != Counter.Frames; }
            private bool UseInt() { return counter == Counter.Frames; }
        }
    }
}