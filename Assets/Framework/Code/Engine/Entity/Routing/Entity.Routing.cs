using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Jape
{
	public partial class Entity
    {
        [Serializable]
        public partial class Routing
        {
            internal enum Field { Output = 0, Target = 1, Action = 2, Parameters = 3, Delay = 4 }

            public enum TargetMode { Global, Local, Self }

            private Routing() {}

            [SerializeField, HideInInspector] 
            internal Entity entity;

            [SerializeField] 
            public string output;

            [SerializeField]
            private TargetMode mode;
            public TargetMode Mode
            {
                get { return mode; }
                set
                {
                    switch (mode)
                    {
                        case TargetMode.Local:
                        case TargetMode.Global:
                        {
                            if (value == TargetMode.Self) { target = "Self"; }
                            break;
                        }
                        case TargetMode.Self:
                        {
                            if (value == TargetMode.Local || value == TargetMode.Global) { target = string.Empty; }
                            break;
                        }
                    }
                    mode = value;
                }
            }

            [SerializeField]
            public string target;

            [SerializeField] 
            public string action;

            [SerializeField] 
            public string[] parameters;

            [SerializeField, HideInInspector] public string[] signatureNames;
            [SerializeField, HideInInspector] public string[] signatureTypes;

            [SerializeField] public float delay;

            internal GameObject LocalTarget()
            {
                Transform parent = entity.transform.parent;
                if (parent != null) { return parent.gameObject; }
                return entity.gameObject;
            }

            internal IEnumerable<Entity> GetTargets()
            {
                switch (mode)
                {
                    case TargetMode.Global: return Game.Find<Entity>().Where(IsMatch);
                    case TargetMode.Local: return LocalTarget().GetComponentsInChildren<Entity>(true).Where(IsMatch); 
                    case TargetMode.Self: return new [] { entity };
                    default: return default;
                }
                bool IsMatch(Entity entity) { return entity.gameObject.name == target; }
            }
            
            internal IEnumerable<MethodInfo> GetMethods(IEnumerable<Entity> entities)
            {
                List<MethodInfo> methods = new();
                foreach (Entity entity in entities) { methods.AddRange(GetMethods(entity)); }
                return methods;
            }

            private IEnumerable<MethodInfo> GetMethods(Entity entity) { return entity.Actions().Where(m => m.Name == action); }

            internal Dictionary<string, string> GetSignatures(MethodInfo method)
            {
                return method == null ?
                       new Dictionary<string, string>() : 
                       method.GetParameters().ToDictionary(parameter => parameter.Name, parameter => parameter.ParameterType.Name);
            }

            internal void SetSignatures(Dictionary<string, string> signatures)
            {
                signatureNames = signatures.Keys.ToArray();
                signatureTypes = signatures.Values.ToArray();
            }

            internal static Routing Create(Entity entity, string output = default, TargetMode targetMode = default, string target = null, string action = null, string[] parameters = null, float delay = 0)
            {
                Routing routing = new()
                {
                    entity = entity,
                    output = output,
                    mode = targetMode,
                    target = target,
                    action = action,
                    parameters = parameters,
                    delay = delay
                };

                return routing;
            }

            internal bool IsIdentical(Routing routing)
            {
                if (entity != routing.entity) { return false; }
                if (output != routing.output) { return false; }
                if (target != routing.target) { return false; }
                if (action != routing.action) { return false; }
                if (parameters == null && routing.parameters != null) { return false; }
                if (parameters != null && routing.parameters == null) { return false; }
                if (!parameters.SequenceEqual(routing.parameters)) { return false; }
                if (!Mathf.Approximately(delay, routing.delay)) { return false; }

                return true;
            }

            public Routing Clone() { return Create(entity, output, mode, target, action, parameters.ToArray(), delay); }
        }
    }
}