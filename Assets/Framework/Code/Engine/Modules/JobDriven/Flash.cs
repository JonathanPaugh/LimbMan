using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jape
{
	public class Flash : JobDriven<Flash>
    {
        private Color defaultColor;
        private Color currentColor;

        private Action<Color> returnAction;

        private List<Instance> instances = new List<Instance>();
        
        internal Flash() { Init(this); }

        public Flash Set(Color defaultColor)
        {
            this.defaultColor = defaultColor;
            return this;
        }

        public Color GetCurrentColor() { return currentColor; }

        private void SetCurrentColor(Color color)
        {
            if (currentColor != color) { returnAction?.Invoke(color); }
            currentColor = color;
        }

        public void CreateFlash(Color color, float time)
        {
            instances.Add(new Instance(defaultColor, color, time));
            Launch();
        }

        public void CreateFade(Color color, float time)
        {
            instances.Add(new Instance(color, defaultColor, time));
            Launch();
        }

        private void Launch()
        {
            if (Job.IsProcessing()) { return; }

            onStart.Trigger(this, EventArgs.Empty);

            processing = true;
            paused = false;
            complete = false;

            StartAction();
        }

        public override Flash Start() { throw new Exception("Cannot use Start() or ForceStart() on Flash, use CreateFlash()"); }
        public override Flash ForceStart() { throw new Exception("Cannot use Start() or ForceStart() on Flash, use CreateFlash()"); }

        protected override void StopAction()
        {
            base.StopAction();
            SetCurrentColor(defaultColor);
        }

        protected override void DestroyAction()
        {
            for (int i = instances.Count - 1; i >= 0; i--) { instances[i].Destroy(); }
            base.DestroyAction();
        }

        public Flash ReturnAction(Action<Color> action)
        {
            returnAction = action;
            return this;
        }

        protected override IEnumerable Run()
        {
            while (instances.Count > 0) 
            {
                Color temp = defaultColor;
                for (int i = instances.Count - 1; i >= 0; i--)
                {
                    if (instances[i].IsProcessed()) { instances.Remove(instances[i]); }
                }
                foreach (Instance instance in instances)
                {
                    temp += instance.GetCurrentColor();
                }
                temp /= instances.Count;
                SetCurrentColor(temp);
                yield return Wait.Frame();
            }

            SetCurrentColor(defaultColor);

            Iteration(); 
            Complete(); 
            Processed();
        }

        public class Instance
        {
            private float time;

            private Color endColor;
            private Color startColor;

            private Timer timer = Timer.CreateGlobal();

            public Instance(Color endColor, Color startColor, float time)
            {
                this.endColor = endColor;
                this.startColor = startColor;
                this.time = time;
                timer.Set(time).Start();
            }

            public bool IsProcessed() { return !timer.IsProcessing(); }

            public Color GetCurrentColor() { return Color.Lerp(startColor, endColor, timer.Progress()); }

            internal void Destroy()
            {
                ModuleManager.Instance?.RemoveGlobal(timer);
                timer.Destroy();
            }
        }
    }
}