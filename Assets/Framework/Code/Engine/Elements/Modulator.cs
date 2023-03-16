using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    [DisallowMultipleComponent]
    public class Modulator : Element
    {
        [SerializeField] [Eject] private Transform position = default;
        [Space(8)]
        [SerializeField] [Eject] private Transform rotation = default;
        [Space(8)]
        [SerializeField] [Eject] private Transform scale = default;

        private Direction.Horizontal overflow;

        public enum Mode { None, Linear, Alternating, Continuous, Random };

        protected override void Enabled()
        {
            position.Set(this, () => transform.localPosition, p => transform.localPosition = p);
            rotation.Set(this, SendRotation, ReturnRotation);
            scale.Set(this, () => transform.localScale, s => transform.localScale = s);
        }

        protected override void Disabled()
        {
            position.Reset();
            rotation.Reset();
            scale.Reset();
        }

        private Vector3 SendRotation()
        {
            Vector3 angle = transform.localEulerAngles;
            switch (overflow)
            {
                case Direction.Horizontal.Left: return angle.z <= 180 ? new Vector3(angle.x, angle.y, angle.z + 360) : angle;
                case Direction.Horizontal.Right: return angle.z > 180 ? new Vector3(angle.x, angle.y, angle.z - 360) : angle;
                default: return angle;
            }
        }

        private void ReturnRotation(Vector3 angle)
        {
            if (angle.z > 180) { overflow = Direction.Horizontal.Left; }
            if (angle.z <= 180) { overflow = Direction.Horizontal.Right; }

            transform.localRotation = Quaternion.Euler(angle);
        }

        [Serializable]
        public class Transform
        {
            [SerializeField] 
            private Mode mode = Mode.None;

            [Space(8)]

            [SerializeField]
            [HideIf(nameof(mode), Mode.None)]
            private Vector3 start = Vector3.zero;

            [SerializeField]
            [HideIf(nameof(mode), Mode.None)]
            private Vector3 end = Vector3.zero;

            [Space(8)]

            [SerializeField] 
            [HideLabel]
            [HideIf(nameof(mode), Mode.None)]
            private Time.Interval interval = new(Time.Counter.Seconds, 0);

            private Vector3 @default;
            private Func<Vector3> current;
            
            private Action<Vector3> set;

            private Vector3 tempStart;
            private Vector3 tempEnd;

            private Timer timer;
            private Job job;

            public void Set(Modulator self, Func<Vector3> get, Action<Vector3> set)
            {
                @default = get();
                current = get;

                this.set = set;

                timer = self.CreateTimer();
                job = self.CreateJob().Set(Run()).ChangeMode(Job.Mode.Loop).Start();
            }

            public void Reset()
            {
                set(@default);
            }

            private float GetChange() { return Vector3.Distance(tempStart, tempEnd) / Vector3.Distance(start, end); }
            private float GetInterval() { return Mathf.Clamp(GetChange() * interval.Value(), 1, Mathf.Infinity); }

            private bool IsActive()
            {
                if (mode == Mode.None) { return false; }
                if (start == Vector3.zero && end == Vector3.zero) { return false; }
                if (Mathf.Approximately(interval.Value(), 0)) { return false; }
                return true;
            }

            private void Prepare()
            {
                switch (mode)
                {
                    case Mode.Linear:
                        tempStart = @default + start;
                        tempEnd = @default + end;
                        break;

                    case Mode.Alternating:
                        tempStart = @default + start;
                        tempEnd = @default + end;
                        break;

                    case Mode.Continuous:
                        tempStart = current();
                        tempEnd = current() + end;
                        break;

                    case Mode.Random:
                        tempStart = current();
                        tempEnd = Random.Vector(@default + start, @default + end);
                        break;

                    default:
                        tempStart = @default;
                        tempEnd = @default;
                        break;
                }
            }

            private IEnumerable Run()
            {
                while (IsActive())
                {
                    Prepare();

                    timer.Set(GetInterval(), interval.Counter).IntervalAction(Progress).Start();

                    yield return timer.WaitIdle();

                    if (mode != Mode.Alternating) { continue; }

                    timer.Set(GetInterval(), interval.Counter).IntervalAction(Inverse).Start();

                    yield return timer.WaitIdle();
                }

                yield return Wait.Frame();

                void Progress(Timer timer) { set(Vector3.Lerp(tempStart, tempEnd, timer.Progress())); }
                void Inverse(Timer timer) { set(Vector3.Lerp(tempStart, tempEnd, timer.Inverse())); }
            }
        }
    }
}