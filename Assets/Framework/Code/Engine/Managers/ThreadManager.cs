using System;
using System.Collections.Generic;

namespace Jape
{
    public sealed class ThreadManager : Manager<ThreadManager> 
    {
        private new static bool InitOnLoad => true;

        private static readonly List<Action> frameReceiver = new();
        private static readonly List<Action> frameSender = new();

        private static readonly List<Action> tickReceiver = new();
        private static readonly List<Action> tickSender = new();

        private static bool processingFrame;
        private static bool processingTick;

        protected override void Frame() { ProcessFrame(); } 
        protected override void Tick() { ProcessTick(); } 

        public static void QueueFrame(Action action)
        {
            if (action == null) { return; }

            lock (frameReceiver)
            {
                frameReceiver.Add(action);
                processingFrame = true;
            }
        }

        public static void QueueTick(Action action)
        {
            if (action == null) { return; }

            lock (tickReceiver)
            {
                tickReceiver.Add(action);
                processingTick = true;
            }
        }

        private static void ProcessFrame()
        {
            if (processingFrame)
            {
                frameSender.Clear();
                lock (frameReceiver)
                {
                    frameSender.AddRange(frameReceiver);
                    frameReceiver.Clear();
                    processingFrame = false;
                }

                foreach (Action action in frameSender)
                {
                    action();
                }
            }
        }

        private static void ProcessTick()
        {
            if (processingTick)
            {
                tickSender.Clear();
                lock (tickReceiver)
                {
                    tickSender.AddRange(tickReceiver);
                    tickReceiver.Clear();
                    processingTick = false;
                }

                foreach (Action action in tickSender)
                {
                    action();
                }
            }
        }
    }
}