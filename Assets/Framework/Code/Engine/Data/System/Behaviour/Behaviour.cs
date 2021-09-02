using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    public partial class Behaviour : SystemData
    {
        protected new static string Path => "System/Resources/Behaviours";

        protected override bool Hidden => true;

        public Type Type => behaviourType.Type.GetSubclass(Assemblies.GetJape()).FirstOrDefault(t => t.Name == script.template.name);

        [PropertySpace(SpaceAfter = 24)]

        [ReadOnly]
        [HideInInlineEditors]
        public BehaviourType behaviourType;

        [PropertySpace(-8)]

        [SerializeField]
        [HideLabel, HideReferenceObjectPicker]
        protected Template script = new Template();

        [PropertySpace(16)]

        [HideInInlineEditors]
        [Button(ButtonSizes.Large)]
        private void DeleteSelf()
        {
            script.Delete();
            DestroyAsset();
        }

        protected virtual string NamePrefix() { return $"_{behaviourType.name}_"; }

        private string Inject(int index) { return index == 0 ? behaviourType.Type.CleanName() : null; }

        public bool IsActive() { return script.IsSet() && Type != null; }

        internal BehaviourInstance CreateInstance()
        {
            if (!IsActive()) { return null; }
            return (BehaviourInstance)Activator.CreateInstance(Type);
        }

        protected override void EnabledEditor()
        {
            if (behaviourType == null) { return; }
            Enable();
        }

        public void Enable()
        {
            script.Script = behaviourType;
            script.Sector = CurrentSector;
            script.Region = behaviourType.GetRegion;
            script.Prefix = NamePrefix();
            script.SetName(name);
            script.SetInject(Inject);
        }

        public static Behaviour Find<T>() where T : BehaviourInstance { return FindAll<Behaviour>().FirstOrDefault(b => b.Type == typeof(T)); }
    }
}