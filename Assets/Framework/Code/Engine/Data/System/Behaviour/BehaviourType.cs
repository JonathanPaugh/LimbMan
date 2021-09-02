using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Jape
{
    public class BehaviourType : Script
    {
        protected override Mode GetMode() => Mode.Behaviour;

        protected new static string Path => "System/Resources/BehaviourTypes";

        protected override string TemplatePath => "System/Resources/Templates/Behaviours";

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
        protected Behaviour.Template script = new Behaviour.Template();

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
        public Behaviour Create()
        {
            // ReSharper disable JoinDeclarationAndInitializer
            Behaviour behaviour = null;
            // ReSharper restore JoinDeclarationAndInitializer

            #if UNITY_EDITOR

            UnityEditor.Selection.activeObject = this;

            string path = $"{GetPath(ScriptType, CurrentSector())}/{name}";

            behaviour = (Behaviour)CreateData(ScriptType, path, behaviourName);

            behaviourName = string.Empty;

            behaviour.behaviourType = this;

            behaviour.Enable();

            UnityEditor.Selection.activeObject = behaviour;
            
            #endif

            return behaviour;
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