using System;
using System.Collections;
using System.Collections.Generic;

namespace Jape
{
    public sealed class JobManager : Manager<JobManager>
    {
        private Job job = Job.CreateGlobal().ChangeMode(Job.Mode.Loop);

        private Dictionary<Job, Action> jobs = new();
        private Queue<Job> inactiveJobs = new();

        protected override void Init() { job.Set(Routine()).Start(); }
        protected override void Destroyed() { job.Destroy(); }

        private IEnumerable Routine()
        {
            foreach (var item in jobs)
            {
                if (item.Key == null) { inactiveJobs.Enqueue(item.Key); continue; }
                if (item.Key.IsProcessing()) { continue; }
                inactiveJobs.Enqueue(item.Key);
                item.Value?.Invoke();
            }

            while (inactiveJobs.Count > 0) { jobs.Remove(inactiveJobs.Dequeue()); }

            yield return Wait.Frame();
        }

        public static void QueueAction(Job job, Action action) { Instance.jobs.Add(job, action); }
    }
}