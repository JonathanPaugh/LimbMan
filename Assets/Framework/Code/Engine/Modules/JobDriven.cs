using System.Collections;
using System.Linq;
using UnityEngine;

namespace Jape
{
	public abstract class JobDriven<T> : Module<T> where T : Module
    {
        private Job job;
        protected Job Job
        {
            get
            {
                if (job != null) { return job; }
                if (ModuleManager.Instance.GetModules().Contains(this))
                {
                    SetJob(Job.Create());
                    return job;
                }
                if (ModuleManager.Instance.GetModulesGlobal().Contains(this))
                {
                    SetJob(Job.CreateGlobal());
                    return job;
                }
                Warning("JobDriven job created without module manager");
                SetJob(new Job());
                return job;
            }
        }

        internal void SetJob(Job job) { this.job = job.Set(Run()); }

        protected override void StartAction() { Job.Start(); }
        protected override void StopAction() { Job.Stop(); }
        protected override void PauseAction() { Job.Pause(); }
        protected override void ResumeAction() { Job.Resume(); }

        protected void ChangeMode(Job.Mode mode) { Job.ChangeMode(mode); }

        protected abstract IEnumerable Run();

        private static bool log = true;

        public static void LogOn() { log = true; }
        public static void LogOff() { log = false; }

        private object Warning(object line) { return !log ? null : this.Log().Warning(line); }
    }
}