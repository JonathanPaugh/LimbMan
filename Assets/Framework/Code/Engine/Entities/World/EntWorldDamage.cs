using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    public abstract class EntWorldDamage : WorldEntity
    {
        private Dictionary<GameObject, Job> targets;

        [Space(8)]

        [SerializeField] private bool retrigger = true;

        [Space(8)]

        [SerializeField] 
        [ShowIf(nameof(retrigger))] 
        [HideLabel] 
        private Time.Interval interval = new(Time.Counter.Seconds, 0);

        protected abstract Damage Damage { get; }

        protected override void Activated()
        {
            targets = new Dictionary<GameObject, Job>();
        }

        protected override void TouchAction(GameObject gameObject)
        {
            if (targets.ContainsKey(gameObject)) { return; }
            gameObject.Properties().OnDisabled += LeaveAction;
            if (retrigger)
            {
                targets.Add(gameObject, CreateJob().Set(DamageRoutine(gameObject)).ChangeMode(Job.Mode.Loop).Start());
            }
            else
            {
                targets.Add(gameObject, null);
                gameObject.Send(Damage);
            }
        }

        protected override void LeaveAction(GameObject gameObject)
        {
            if(!targets.TryGetValue(gameObject, out Job job)) { return; }
            gameObject.Properties().OnDisabled -= LeaveAction;
            targets.Remove(gameObject);
            DestroyJob(job);
        }

        private IEnumerable DamageRoutine(GameObject gameObject)
        {
            gameObject.Send(Damage);
            switch (interval.Counter)
            {
                case Time.Counter.Frames: yield return Wait.Frames((int)interval.Value()); break;
                case Time.Counter.Seconds: yield return Wait.Seconds(interval.Value()); break;
                case Time.Counter.Realtime: yield return Wait.Realtime(interval.Value()); break;
            }
        }
    }
}