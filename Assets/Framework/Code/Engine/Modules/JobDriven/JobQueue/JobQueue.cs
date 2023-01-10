using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Jape
{
    public partial class JobQueue : JobDriven<JobQueue>
    {
        private bool finishing;

        public enum Mode { Single, Constant, Loop };
        private Mode mode = Mode.Single;

        private LinkedList<Task> queue = new LinkedList<Task>();
        private List<Task> activeTasks = new List<Task>();

        public Event<EventArgs> onKill = new Event<EventArgs>();

        internal JobQueue() { Init(this); }

        protected sealed override void StartAction() { Job.ForceStart(); }
        protected sealed override void StopAction() { foreach (Task task in activeTasks.Where(q => !q.Persistent())) { task.Job().Stop(); }}
        protected sealed override void PauseAction() { Job.Pause(); foreach (Task task in activeTasks) { task.Job().Pause(); }}
        protected sealed override void ResumeAction() { Job.Resume(); foreach (Task task in activeTasks) { task.Job().Resume(); }}
        protected sealed override void DestroyAction() { Kill(); }

        /// <summary>
        /// Start the job queue
        /// </summary>
        public override JobQueue Start()
        {
            if (IsActive()) { this.Log().Response("Cant start because there is active jobs or it is processing, use ForceStart() to override"); return this; }

            ForceStart();

            return instance;
        }

        /// <summary>
        /// Start the job queue even if processing, kill all active jobs
        /// </summary>
        public override JobQueue ForceStart()
        {
            if (IsActive()) { Kill(); }

            onStart.Trigger(this, EventArgs.Empty);

            processing = true;
            paused = false;
            finishing = false;
            complete = false;

            StartAction();

            return this;
        }

        /// <summary>
        /// Stop all non persistent jobs next yield statement, queue will run until persistent jobs are processed
        /// </summary>
        public override JobQueue Stop()
        {
            if (!IsActive()) { return this; }
            if (finishing) { return this; }
            if (paused) { throw new NotImplementedException(); }

            onStop.Trigger(this, EventArgs.Empty);

            finishing = true;

            StopAction();

            return this;
        }

        /// <summary>
        /// Pause the queue and active jobs next yield statement.
        /// </summary>
        public override JobQueue Pause()
        {
            if (paused || !IsActive()) { return instance; }

            onPause.Trigger(this, EventArgs.Empty);

            paused = true;

            PauseAction();

            return instance;
        }

        /// <summary>
        /// Resume the queue and paused jobs instantly.
        /// </summary>
        public override JobQueue Resume()
        {
            if (!paused || !IsActive()) { return instance; }

            onResume.Trigger(this, EventArgs.Empty);

            paused = false;

            ResumeAction();

            return instance;
        }

        /// <summary>
        /// Stop the queue and all active jobs next yield statement
        /// </summary>
        public JobQueue Kill()
        {
            if (!IsActive()) { return this; }

            onKill.Trigger(this, EventArgs.Empty);

            KillAction();

            Processed();

            return this;

            void KillAction()
            {
                Job.Stop();
                foreach (Task task in activeTasks) { task.Job().Stop(); }
            }
        }

        public JobQueue ChangeMode(Mode mode)
        {
            this.mode = mode; 
            return this;
        }

        /// <summary>
        /// Clear the queue, preventing any upcoming jobs from being run.
        /// </summary>
        public JobQueue Clear()
        {
            queue.Clear();
            return this;
        }

        public static Task CreateTask(Job job) { return new Task(job); }

        public Task Queue(Job job) { return Queue(CreateTask(job)); }
        public Task Queue(Task task)
        {
            queue.AddLast(task);
            return task;
        }

        public Task Budge(Job job) { return Budge(CreateTask(job)); }
        public Task Budge(Task task)
        {
            queue.AddFirst(task);
            return task;
        }

        public Task Remove(Job job) { return Remove(queue.FirstOrDefault(q => q.Job() == job)); }
        public Task Remove(Task task)
        {
            queue.Remove(task);
            return task;
        }

        private Task Peek() { return queue.First(); }
        private Task PeekLast() { return queue.Last(); }

        private Task Pop()
        {
            Task task = Peek();
            queue.RemoveFirst();

            return task;
        }

        private Task PopLast()
        {
            Task task = PeekLast();
            queue.RemoveLast();

            return task;
        }

        private LinkedListNode<Task> Seek(Task task) { return queue.Find(task); }
        
        public bool IsActive() { return processing || TasksActive(); }
        public bool TasksActive() { return activeTasks.Count > 0; }

        public bool Contains(Task task) { return queue.Contains(task); }

        protected override IEnumerable Run()
        {
            LinkedList<Task> tempTasks = new LinkedList<Task>();

            while (!complete)
            {
                if (queue.Count > 0)
                {
                    Task current = Pop();

                    if (current == null) { continue; }
                    if (!current.Persistent() && finishing) { continue; }

                    activeTasks.Add(current);
                    tempTasks.AddLast(current.CloneConvert());

                    current.Job().Start();
                        
                    if (current.Job().IsProcessing())
                    {
                        JobManager.QueueAction(current.Job(), QueueRemove);
                        if (current.Wait()) { yield return current.Job().WaitIdle(); }
                        void QueueRemove() { activeTasks.Remove(current); }
                    }
                    else
                    {
                        activeTasks.Remove(current);
                    }
                }
                else
                {
                    if (finishing) { Processed(); break; }
                    switch (mode)
                    {
                        case Mode.Single: Iteration(); Processed(); Complete(); break;
                        case Mode.Constant: Iteration(); yield return Wait.Tick(); break;
                        case Mode.Loop: Iteration(); queue = new LinkedList<Task>(tempTasks); tempTasks.Clear(); break;
                    }
                }
            }
        }
    }
}