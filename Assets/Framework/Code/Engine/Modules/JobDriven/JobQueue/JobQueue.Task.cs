using System;

namespace Jape
{
    public partial class JobQueue
    {
        public class Task : ICloneable
        {
            private bool wait = true;
            private bool persistent;

            private Job job;

            public Task(Job job) { this.job = job; }

            public object Clone()
            {
                Task clone = new Task(job.CloneConvert())
                {
                    wait = wait,
                    persistent = persistent
                };

                return clone;
            }

            public Job Job() { return job; }

            public Task DontWait() { wait = false; return this; }
            public Task Persist() { persistent = true; return this; }

            public bool Wait() { return !job.IsAction() && wait; }
            public bool Persistent() { return persistent; }
        }
    }
}