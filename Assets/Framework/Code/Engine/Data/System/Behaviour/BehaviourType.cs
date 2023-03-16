using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Jape
{
    public class BehaviourType : Script
    {
        protected override Mode GetMode() => Mode.Behaviour;

        protected new static string Path => IO.JoinPath(SystemPath, "BehaviourTypes");
        protected override string TemplatePath => IO.JoinPath(SystemPath, "Templates", "Behaviours");

        protected virtual string SingularName => $"{ScriptType.CleanName()}";
        protected virtual string PluralName => $"{SingularName}s";

        protected override string PathSuffix => PluralName;

        protected virtual string ButtonName => $"Create {name} {SingularName}";
        protected virtual string ListName => $"{name} {PluralName}";

        protected virtual string DefaultTemplate => "DefaultBehaviour";

        protected virtual Type InstanceType => typeof(BehaviourInstance);
        protected virtual Type ScriptType => typeof(Behaviour);

        public Type Type => InstanceType.GetSubclass(Assemblies.GetJape(), false, true).FirstOrDefault(t => t.Name == script.template.name);

        [SerializeField]
        [ShowIf(nameof(GetMode), Mode.Behaviour)]
        [LabelText("Code Region")]
        [EnumToggleButtons]
        protected CodeRegion behaviourRegion = CodeRegion.Engine;

        [PropertySpace(16)]

        [SerializeField]
        [HideLabel, HideReferenceObjectPicker]
        protected Behaviour.Template script = new();

        [PropertySpace(32)]

        [ShowInInspector]
        [HideLabel]
        public string BehaviourName
        {
            get { return behaviourName; }
            set { behaviourName = value.Replace(" ", string.Empty); }
        }

        [SerializeField, HideInInspector]
        private string behaviourName;

        [PropertyOrder(1)]
        [EnableIf(nameof(IsNameSet))]
        [LabelText("$" + nameof(ButtonName))]
        [Button(ButtonSizes.Large, DrawResult = false)]
        public Behaviour CreateBehaviourEditor()
        {
            Behaviour behaviour = CreateBehaviourEditor(behaviourName, CurrentSector());
            behaviourName = string.Empty;
            return behaviour;
        }

        public Behaviour CreateBehaviourEditor(string name, Sector sector)
        {
            #if UNITY_EDITOR

            string path = IO.JoinPath(GetPath(ScriptType, sector), this.name);

            Behaviour behaviour = (Behaviour)CreateData(ScriptType, path, name, true);

            behaviour.behaviourType = this;

            behaviour.Enable();
            
            return behaviour;

            #else
            return null;
            #endif
        }

        [PropertyOrder(2)]
        [ShowInInspector]
        [LabelText("$" + nameof(ListName))]
        [ReadOnly]
        public List<Behaviour> Behaviours => FindAll(ScriptType).Cast<Behaviour>().Where(b => b.behaviourType != null && b.behaviourType == this).ToList();

        public CodeRegion GetRegion() { return behaviourRegion; }

        private bool IsNameSet() { return !string.IsNullOrEmpty(behaviourName); }

        protected override void Created() { template.template = Database.GetAsset<TextAsset>(DefaultTemplate).Load<TextAsset>(); }

        protected override void EnabledEditor()
        {
            #if UNITY_EDITOR

            base.EnabledEditor();
            
            script.Script = Find<Script>(GetType().CleanName());
            script.Sector = CurrentSector;
            script.Suffix = SingularName;
            script.SetName(name);
            
            path = name;

            #endif
        }
    }
}