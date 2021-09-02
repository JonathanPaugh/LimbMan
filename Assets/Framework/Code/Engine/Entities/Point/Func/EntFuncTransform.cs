using System;
using UnityEngine;

namespace Jape
{
    [AddComponentMenu("")]
    public class EntFuncTransform : EntFunc
    {
        protected override Texture Icon => GetIcon("IconTransform");

        private Timer moveTimer;
        private Timer rotateTimer;
        private Timer resizeTimer;

        protected override void Activated()
        {
            moveTimer = CreateTimer();
            rotateTimer = CreateTimer();
            resizeTimer = CreateTimer();
        }

        [Route]
        public void SetPosition(float x, float y, float z) { PositionLocal = new Vector3(x, y, z); }

        [Route]
        public void SetRotation(float x, float y, float z) { RotationLocal = Quaternion.Euler(x, y, z); }

        [Route]
        public void SetScale(float x, float y, float z) { ScaleLocal = new Vector3(x, y, z); }

        [Route]
        public void Move(float seconds, float x, float y, float z)
        {
            if (moveTimer.IsProcessing()) { this.Log().Response("Cannot move when already moving"); }
            Vector3 position = transform.localPosition;
            moveTimer.Set(seconds).IntervalAction(Transform).Start();
            void Transform(Timer timer) { PositionLocal = Vector3.Lerp(position, position + new Vector3(x, y, z), timer.Progress()); }
        }

        [Route]
        public void Rotate(float seconds, float x, float y, float z)
        {
            if (rotateTimer.IsProcessing()) { this.Log().Response("Cannot rotate when already rotating"); }
            Quaternion rotation = transform.localRotation;
            rotateTimer.Set(seconds).IntervalAction(Transform).Start();
            void Transform(Timer timer) { RotationLocal = Quaternion.Lerp(rotation, rotation * Quaternion.Euler(x, y, z), timer.Progress()); }
        }

        [Route]
        public new void Scale(float seconds, float x, float y, float z)
        {
            if (resizeTimer.IsProcessing()) { this.Log().Response("Cannot scale when already scaling"); }
            Vector3 scale = transform.localScale;
            resizeTimer.Set(seconds).IntervalAction(Transform).Start();
            void Transform(Timer timer) { ScaleLocal = Vector3.Lerp(scale, scale + new Vector3(x, y, z), timer.Progress()); }
        }
    }
}