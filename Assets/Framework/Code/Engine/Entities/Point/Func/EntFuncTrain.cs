using System.Collections;
using UnityEngine;

namespace Jape
{
    [AddComponentMenu("")]
    public class EntFuncTrain : EntFunc
    {
        protected override Texture2D Icon => GetIcon("IconTrain");

        [SerializeField] private EntInfoTrack track = null;

        [SerializeField] private bool autoStart = true;

        [SerializeField] private float speed;

        private EntInfoTrack current;
        private EntInfoTrack target;

        private Job job;
        private Timer timer;

        private bool stopped;

        private float GetSpeed() { return !Mathf.Approximately(current.speed, -1) ? current.speed : speed; }

        protected override void Activated()
        {
            job = CreateJob().Set(MoveRoutine());
            timer = CreateTimer();
        }

        protected override void Init()
        {
            InitTarget();
            if (autoStart) { job.Start(); }
        }

        [Route]
        public void TrainStart()
        {
            if (!job.IsProcessing()) { job.Start(); }
            else
            {
                timer.Resume();
                stopped = false;
            }
        }

        [Route]
        public void TrainStop()
        {
            stopped = true;
            timer.Pause();
        }

        [Route]
        public void ChangeSpeed(float speed) { this.speed = speed; }

        protected override void FrameEditor()
        {
            if (track != null)
            {
                transform.position = track.transform.position;
            }
        }

        private void InitTarget()
        {
            current = track;
            target = track.target;
        }

        private void NextTarget()
        {
            current = target;
            target = current.target;
        }

        private IEnumerable MoveRoutine()
        {
            while (target != null)
            {
                while (stopped)
                {
                    yield return Wait.Frame();
                }

                if (current.delay > 0)
                {
                    timer.Set(current.delay).ClearActions().Start();
                    while (timer.IsProcessing()) { yield return timer.WaitIdle(); }
                }

                if (Mathf.Approximately(GetSpeed(), 0)) { yield return Wait.Until(() => !Mathf.Approximately(GetSpeed(), 0)); }

                float speed = GetSpeed();
                float distance = Vector3.Distance(current.transform.position, target.transform.position);

                if (distance > 0)
                {
                    float time = distance / speed;
                    timer.Set(time).IntervalAction(Move).Start();
                    while (timer.IsProcessing()) { yield return timer.WaitIdle(); }
                }

                NextTarget();
                current.Pass();
            }

            void Move() { transform.position = Vector3.Lerp(current.transform.position, target.transform.position, timer.Progress()); }
        }
    }
}