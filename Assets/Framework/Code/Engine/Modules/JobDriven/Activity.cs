using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Jape
{
	public class Activity : Job 
    {
        protected override bool ProcessOnStop => false;

        private List<JobQueue.Task> routineTasks = new List<JobQueue.Task>();

        private JobQueue.Task setupTask;
        private JobQueue.Task successTask;
        private JobQueue.Task cleanupTask;

        private Restrictor<Type> restrictor = new Restrictor<Type>();

        private JobQueue queue;
        protected JobQueue Queue
        {
            get
            {
                if (queue != null) { return queue; }
                if (ModuleManager.Instance.GetModules().Contains(this))
                {
                    SetQueue(JobQueue.Create());
                    return queue;
                }
                if (ModuleManager.Instance.GetModulesGlobal().Contains(this))
                {
                    SetQueue(JobQueue.CreateGlobal());
                    return queue;
                }
                this.Log().Warning("Activity clone created without module manager");
                SetQueue(new JobQueue());
                return queue;
            }
        }

        internal Activity()
        {
            Init(this);
            routine = new Routine(Run());
        }
        
        internal void SetQueue(JobQueue jobQueue) { queue = jobQueue; }

        /// <summary>
        /// Clone activity to new activity in starting state
        /// </summary>
        public override object Clone()
        {
            Activity clone;

            if (ModuleManager.Instance.GetModules().Contains(this)) { clone = Module<Activity>.Create(); }
            else if (ModuleManager.Instance.GetModulesGlobal().Contains(this)) { clone = Module<Activity>.CreateGlobal(); }
            else
            {
                clone = new Activity();
                this.Log().Warning("Activity clone created without module manager");
            }

            clone.setupTask = setupTask.CloneConvert();
            clone.successTask = successTask.CloneConvert();
            clone.cleanupTask = cleanupTask.CloneConvert();
            clone.routineTasks = routineTasks.Select(t => t.CloneConvert()).ToList();

            return (Activity)CloneFill(clone);
        }

        protected virtual Job CreateJob(IEnumerable routine)
        {
            if (ModuleManager.Instance.GetModules().Contains(this)) { return Create().Set(routine); }
            if (ModuleManager.Instance.GetModulesGlobal().Contains(this)) { return CreateGlobal().Set(routine); }
            this.Log().Warning("Activity job created without module manager");
            return new Job().Set(routine);
        }

        protected virtual Job CreateJob(Action routine)
        {
            if (ModuleManager.Instance.GetModules().Contains(this)) { return Create().Set(routine); }
            if (ModuleManager.Instance.GetModulesGlobal().Contains(this)) { return CreateGlobal().Set(routine); }
            this.Log().Warning("Activity job created without module manager");
            return new Job().Set(routine);
        }

        protected override void StartAction()
        {
            if (IsRestricted())
            {
                this.Log().Diagnostic("Unable to start restricted activity");
                return;
            }

            routine = IsInstant() ? new Routine(Execute) : new Routine(Run());
            routine.Launch(Dispatch, Action);
        }
        protected override void StopAction() { Queue.Stop(); }
        protected override void PauseAction() { Queue.Pause(); }
        protected override void ResumeAction() { Queue.Resume(); }
        protected override void DestroyAction() { Kill(); }

        public override Job Stop()
        {
            if (!processing) { return instance; }

            onStop.Trigger(this, EventArgs.Empty);

            StopAction();

            return this;
        }

        public Job Kill()
        {
            Queue.Kill();
            return this;
        }

        public JobQueue.Task Action(IEnumerable routine)
        {
            JobQueue.Task task = JobQueue.CreateTask(CreateJob(routine));
            routineTasks.Add(task);
            return task;
        }

        public JobQueue.Task Action(Action routine)
        {
            JobQueue.Task task = JobQueue.CreateTask(CreateJob(routine));
            routineTasks.Add(task);
            return task;
        }

        public JobQueue.Task Action(Job job)
        {
            JobQueue.Task task = JobQueue.CreateTask(job);
            routineTasks.Add(task);
            return task;
        }

        public JobQueue.Task Setup(IEnumerable routine) { setupTask = JobQueue.CreateTask(CreateJob(routine)).Persist(); return setupTask; }
        public JobQueue.Task Setup(Action routine) { setupTask = JobQueue.CreateTask(CreateJob(routine)).Persist(); return setupTask; }
        public JobQueue.Task Setup(Job job) { setupTask = JobQueue.CreateTask(job).Persist(); return setupTask; }

        public JobQueue.Task Success(IEnumerable routine) { successTask = JobQueue.CreateTask(CreateJob(routine)); return successTask; }
        public JobQueue.Task Success(Action routine) { successTask = JobQueue.CreateTask(CreateJob(routine)); return successTask; }
        public JobQueue.Task Success(Job job) { successTask = JobQueue.CreateTask(job); return successTask; }

        public JobQueue.Task Cleanup(IEnumerable routine) { cleanupTask = JobQueue.CreateTask(CreateJob(routine)).Persist(); return cleanupTask; }
        public JobQueue.Task Cleanup(Action routine) { cleanupTask = JobQueue.CreateTask(CreateJob(routine)).Persist(); return cleanupTask; }
        public JobQueue.Task Cleanup(Job job) { cleanupTask = JobQueue.CreateTask(job).Persist(); return cleanupTask; }

        public bool IsRestricted() { return restrictor.IsRestricted(); }
        private bool IsInstant()
        {
            JobQueue.Task[] tasks = routineTasks.Append(setupTask).Append(successTask).Append(cleanupTask).ToArray();
            return tasks.Where(t => t != null).All(t => !t.Wait());
        }

        public void Execute()
        {
            Queue.Clear();

            if (setupTask != null) { Queue.Queue(setupTask); }
            foreach (JobQueue.Task job in routineTasks) { Queue.Queue(job); }
            if (successTask != null) { Queue.Queue(successTask); }
            if (cleanupTask != null) { Queue.Queue(cleanupTask); }

            Queue.Start();
        }

        public IEnumerable Run()
        {
            Execute();
            yield return Queue.onProcessed.Wait();
        }

        public Activity Restrict(Type restrictor)
        {
            this.restrictor.Restrict(restrictor);
            return this;
        }

        public Activity Unrestrict(Type restrictor)
        {
            this.restrictor.Unrestrict(restrictor);
            return this;
        }
    }
}