using System.Collections.Generic;
using UnityEngine;

namespace Jape
{
    public sealed class ModuleManager : Manager<ModuleManager>
    {
        private new static bool InitOnLoad => true;

        private List<Module> modules = new List<Module>();
        private List<Module> globalModules = new List<Module>();

        internal IEnumerable<Module> GetModules() { return modules; }

        internal void Add(Module module) { modules.Add(module); }
        internal void Remove(Module module) { modules.Remove(module); }

        public void DestroyAll()
        {
            Module[] modules = this.modules.ToArray();
            for (int i = modules.Length - 1; i >= 0; i--)
            {
                this.modules.Remove(modules[i]);
                modules[i].Destroy();
            }
        }

        internal IEnumerable<Module> GetModulesGlobal() { return globalModules; }

        internal void AddGlobal(Module module) { globalModules.Add(module); }
        internal void RemoveGlobal(Module module) { globalModules.Remove(module); }

        internal void DestroyGlobal(Module module)
        {
            if (!globalModules.Contains(module)) { this.Log().Response("Cannot find global module to destroy"); }
            globalModules.Remove(module);
            module.Destroy();
        }

        public void DestroyAllGlobal()
        {
            Module[] modules = globalModules.ToArray();
            for (int i = modules.Length - 1; i >= 0; i--)
            {
                globalModules.Remove(modules[i]);
                modules[i].Destroy();
            }
        }

        protected override void Destroyed()
        {
            DestroyAll();
            DestroyAllGlobal();
        }
    }
}