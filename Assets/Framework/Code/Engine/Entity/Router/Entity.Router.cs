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
        public partial class Router
        {
            [SerializeField]
            private List<Routing> routings = new();

            [SerializeField, HideInInspector] 
            private Sorting sorting = new();

            public List<Routing> GetRoutings() { return routings; }

            internal Sorting GetSorting() { return sorting; }

            internal void SortAscend(Routing.Field field)
            {
                switch (field)
                {
                    case Routing.Field.Output:
                        routings = routings.OrderBy(r => r.output).Reverse().ToList();
                        break;

                    case Routing.Field.Target:
                        routings = routings.OrderBy(r => r.target).Reverse().ToList();
                        break;

                    case Routing.Field.Action:
                        routings = routings.OrderBy(r => r.action).Reverse().ToList();
                        break;

                    case Routing.Field.Parameters:
                        routings = routings.OrderBy(r => string.Join(string.Empty, r.parameters)).Reverse().ToList();
                        break;

                    case Routing.Field.Delay:
                        routings = routings.OrderBy(r => r.delay).Reverse().ToList();
                        break;
                }
            }

            internal void SortDescend(Routing.Field field)
            {
                switch (field)
                {
                    case Routing.Field.Output:
                        routings = routings.OrderBy(r => r.output).ToList();
                        break;

                    case Routing.Field.Target:
                        routings = routings.OrderBy(r => r.target).ToList();
                        break;

                    case Routing.Field.Action:
                        routings = routings.OrderBy(r => r.action).ToList();
                        break;

                    case Routing.Field.Parameters:
                        routings = routings.OrderBy(r => string.Join(string.Empty, r.parameters)).ToList();
                        break;

                    case Routing.Field.Delay:
                        routings = routings.OrderBy(r => r.delay).ToList();
                        break;
                }
            }

            public Router Add(Routing routing)
            {
                routings.Add(routing);
                return this;
            }

            public Router Remove(Routing routing)
            {
                routings.Remove(routing);
                return this;
            }

            public Router Clear()
            {
                routings.Clear();
                return this;
            }

            internal void Launch(Entity caller, Enum output, params object[] sends)
            {
                Routing[] outputRoutings = routings.Where(r => r.output == output.ToString()).ToArray();
                foreach (Routing routing in outputRoutings)
                {
                    if (Mathf.Approximately(routing.delay, 0)) { Action(routing); }
                    else { Timer.Create().Set(routing.delay).CompletedAction(() => Action(routing)).Start(); }
                }

                void Action(Routing routing)
                {
                    Entity[] targets = routing.GetTargets().ToArray();

                    if (targets.Length == 0) { this.Log().Warning("Unable to find any targets"); return; }

                    ParameterInfo[] signatures = routing.GetMethods(targets).First().GetParameters().ToArray();

                    foreach (Entity target in targets)
                    {
                        MethodInfo method = target.Actions().First(a => a.Name == routing.action);
                        List<object> args = new();

                        for (int i = 0; i < signatures.Length; i++)
                        {
                            ParameterInfo signature = signatures[i];

                            object value = routing.parameters[i];

                            if (Routing.Selector.ParameterIndicated(routing.parameters[i])) { value = Routing.Selector.ParameterValue(routing, caller, routing.parameters[i], sends); }

                            if (signature.ParameterType == typeof(object))
                            {
                                args.Add(value);
                                continue;
                            }

                            if (signature.ParameterType == typeof(string))
                            {
                                args.Add(value.ToString());
                                continue;
                            }

                            try { args.Add(Convert.ChangeType(value, signature.ParameterType)); }
                            catch { this.Log().Warning($"{routing.entity.name} {output}: Unable to convert parameter \"{signature.Name.SerializationName()}\" {value.GetType()} to {signature.ParameterType}"); return; }
                        }

                        method.Invoke(target, args.ToArray());
                    }
                }
            }
        }
    }
}