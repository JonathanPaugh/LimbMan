using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jape
{
    public abstract partial class Element : Mono
    {
        public virtual bool Saved => false;

        public virtual string Key => !string.IsNullOrEmpty(gameObject.Id()) ? 
                                     $"{GetType().FullName}_{gameObject.Id()}" : 
                                     $"{GetType().FullName}_{gameObject.Alias()}";

        internal List<Job> jobs = new List<Job>();
        internal List<Activity> activities = new List<Activity>();
        internal List<ModifierInstance> modifiers = new List<ModifierInstance>();

        internal void AddModifier(ModifierInstance modifier) { modifiers.Add(modifier); }
        internal void RemoveModifier(ModifierInstance modifier) { modifiers.Remove(modifier); }
        public bool HasModifier(ModifierInstance modifier) { return modifiers.Contains(modifier); }

        protected virtual Status CreateStatus() => new Status { Key = Key };

        private bool CanSave()
        {
            if (string.IsNullOrEmpty(gameObject.Id()) && string.IsNullOrEmpty(gameObject.Alias())) { return false; }
            return Saved && gameObject.Properties().CanSaveElement(this);
        }
        
        /// <summary>
        /// Called before status is saved
        /// Used to set status values from element
        /// </summary>
        protected virtual void StatusSave(Status status) {}

        /// <summary>
        /// Called after status is loaded, called after Init() when first initialized
        /// Used to set element values from status
        /// </summary>
        protected virtual void StatusLoad(Status status) {}

        /// <summary>
        /// Called before status is loaded, called before StatusLoad()
        /// </summary>
        protected virtual void StatusPreload() {}

        /// <summary>
        /// Called when status is streaming
        /// </summary>
        protected virtual void StatusStream(DataStream stream) {}

        public void Save()
        {
            if (!CanSave()) { return; }

            Status status = CreateStatus();

            SaveAttributes(status);
            SaveStream(status);
            StatusSave(status);

            Jape.Status.Save(status);
        }

        public void Load()
        {
            if (!CanSave()) { return; }

            Status status = Jape.Status.Load<Status>(Key);

            if (status == null) { return; }

            StatusPreload();
            LoadAttributes(status);
            LoadStream(status);
            StatusLoad(status);
        }

        private void SaveStream(Status status)
        {
            status.StreamWrite(StatusStream);
        }

        private void LoadStream(Status status)
        {
            status.StreamRead(StatusStream);
        }

        private IEnumerable<FieldInfo> GetSavedMembers()
        {
            return GetType().GetFields().Where(f => Attribute.IsDefined(f, typeof(SaveAttribute)));
        }

        private void SaveAttributes(Status status)
        {
            foreach (FieldInfo field in GetSavedMembers())
            {
                SaveAttribute attribute = field.GetCustomAttribute<SaveAttribute>();
                string key = attribute.Key ?? field.Name;
                if (status.AttributeData.ContainsKey(key))
                {
                    status.AttributeData[key] = field.GetValue(this);
                } 
                else { status.AttributeData.Add(key, field.GetValue(this)); }
            }
        }

        private void LoadAttributes(Status status)
        {
            foreach (FieldInfo field in GetSavedMembers())
            {
                SaveAttribute attribute = field.GetCustomAttribute<SaveAttribute>();
                string key = attribute.Key ?? field.Name;
                if (status.AttributeData.ContainsKey(key)) { field.SetValue(this, status.AttributeData[key]); } 
            }
        }

        internal override void Awake()
        {
            base.Awake(); // First //

            if (Game.IsRunning)
            {
                EngineManager.Instance.runtimeElements.Add(this);
                SaveManager.Instance.OnSaveRequest += Save;
                SaveManager.Instance.OnLoadResponse += Load;

                Load();
            }
        }

        internal override void OnDestroy()
        {
            base.OnDestroy(); // First //

            if (Game.IsRunning)
            {
                if (EngineManager.Instance != null) { EngineManager.Instance.runtimeElements.Remove(this); }
                if (SaveManager.Instance != null)
                {
                    SaveManager.Instance.OnSaveRequest -= Save;
                    SaveManager.Instance.OnLoadResponse -= Load;
                }

                DestroyJobs();
                DestroyActivities();
                DestroyModifiers();
            }
        }

        internal void Send(IReceivable receivable)
        {
            if (receivable == null) { return; }
            foreach (FieldInfo field in GetType().GetFields(Member.Bindings).Where(f => f.FieldType.IsGenericSubclassOf(typeof(Receiver<>))))
            {
                Type typeArgument = field.FieldType.GetGenericArguments().First();
                if (receivable.GetType().IsBaseOrSubclassOf(typeArgument))
                {
                    typeof(Receiver<>).MakeGenericType(typeArgument).GetMethod("Receive").Invoke(field.GetValue(this), new object[] { this, receivable });
                }
            }
        }

        private void DestroyModifiers()
        {
            for (int i = modifiers.Count - 1; i >= 0; i--)
            {
                ModifierInstance modifier = modifiers[i];
                modifier.Destroy();
            }
        }

        protected Job RunJob(IEnumerable routine)
        {
            Job job = CreateJob().Set(routine).Start();
            JobManager.QueueAction(job, QueueDestroy);
            return job;

            void QueueDestroy() { DestroyJob(job); }
        }

        protected Job RunJob(Action routine)
        {
            Job job = CreateJob().Set(routine).Start();
            JobManager.QueueAction(job, QueueDestroy);
            return job;

            void QueueDestroy() { DestroyJob(job); }
        }

        protected Job CreateJob()
        {
            Job job = new ElementJob(this);
            jobs.Add(job);
            return job;
        }

        protected void DestroyJob(Job job) 
        {
            job?.Destroy();
            jobs.Remove(job);
        }

        private void DestroyJobs()
        {
            for (int i = jobs.Count - 1; i >= 0; i--)
            {
                DestroyJob(jobs[i]);
            }
        }

        protected Activity CreateActivity()
        {
            Activity activity = new ElementActivity(this);
            activities.Add(activity);
            return activity;
        }

        protected void DestroyActivity(Activity activity) 
        {
            activity?.Destroy();
            activities.Remove(activity);
        }

        private void DestroyActivities()
        {
            for (int i = activities.Count - 1; i >= 0; i--)
            {
                DestroyActivity(activities[i]);
            }
        }

        protected JobQueue CreateJobQueue() { return CreateDrivenModule<JobQueue>(); }

        protected Timer CreateTimer() { return CreateDrivenModule<Timer>(); }
        protected void Delay(float time, Time.Counter counter, Action action) { Timer.Delay(CreateTimer(), time, counter, action); }

        protected Condition CreateCondition() { return CreateDrivenModule<Condition>(); }
        protected Flash CreateFlash() { return CreateDrivenModule<Flash>(); }

        public T CreateDrivenModule<T>() where T : JobDriven<T>
        {
            JobDriven<T>.LogOff();
            T module = (T)Activator.CreateInstance(typeof(T), true);
            module.SetJob(CreateJob());
            JobDriven<T>.LogOn();
            return module;
        }

        protected static Receiver<T> Receive<T>(Action<Element, T> action) where T : IReceivable { return new Receiver<T>(action); }

        public static IEnumerable<Type> Subclass(bool includeEntities = false, bool includeManagers = false)
        {
            IEnumerable<Type> classes = typeof(Element).GetSubclass();

            if (!includeEntities) { classes = classes.Where(t => !typeof(Entity).IsAssignableFrom(t)); }
            if (!includeManagers) { classes = classes.Where(t => !t.IsGenericSubclassOf(typeof(Manager<>))); }

            return classes;
        }

        [Pure] public new static IEnumerable<T> FindAll<T>() where T : Element { return FindAll(typeof(T)).Cast<T>(); }
        [Pure] public new static IEnumerable<Element> FindAll(Type type) { return FindAll().Where(e => e.GetType().IsBaseOrSubclassOf(type)); }
        [Pure] public new static IEnumerable<Element> FindAll() { return EngineManager.Instance.runtimeElements.Where(e => e.gameObject.activeInHierarchy); }
    }
}