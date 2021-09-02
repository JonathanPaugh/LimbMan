using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
	public abstract partial class Entity : Element
    {
        public enum Class { Point, World }

        protected virtual Texture Icon => GetIcon("IconEntity");
        protected virtual int IconSize => 32;

        [PropertySpace(8, 8)]

        [PropertyOrder(-100)]
        [SerializeField]
        [DisableInPrefabAssets]
        [Button(ButtonSizes.Large)]
        [LabelText("Router")]
        private void RouterButton() { RouterWindow.Call(this); }

        [PropertySpace(0, 8)]

        [PropertyOrder(-100)]
        [SerializeField]
        [Button(ButtonSizes.Large)]
        [LabelText("Trigger")]
        [ShowIf(Game.GameIsRunning)]
        private void TriggerButton() { Trigger(); }

        [PropertySpace(16)]

        [PropertyOrder(100)]
        [SerializeField]
        [HideIf(nameof(Always))]
        [HideLabel]
        private Router router;
        public Router GetRouter() { return router ?? (router = new Router()); }

        protected Texture GetIcon(string name) { return Database.GetAsset<Texture>(name).Load<Texture>(); }

        protected virtual Type[] Components()
        {
            return new[]
            {
                typeof(IEntityComponent)
            };
        }

        public virtual Enum BaseOutputs() { return BaseOutputsFlags.None | 
                                                   BaseOutputsFlags.OnTrigger | 
                                                   BaseOutputsFlags.OnEnable | 
                                                   BaseOutputsFlags.OnDisable | 
                                                   BaseOutputsFlags.OnDestroy; }

        public virtual Enum Outputs() { return null; }

        public virtual IEnumerable<Send> Sends() { return new Send[0]; }

        internal IEnumerable<string> GetOutputs()
        {
            List<string> outputs = new List<string>();
            if (Outputs() != null) { outputs.AddRange(Outputs().FlagValues()); }
            if (BaseOutputs() != null) { outputs.AddRange(BaseOutputs().FlagValues()); }
            return outputs;
        }

        internal IEnumerable<MethodInfo> Actions() { return TypeActions().Concat(BaseActions()); }

        internal IEnumerable<MethodInfo> TypeActions()
        {
            return GetType().
                   GetMethods(Member.Bindings).
                   Where(m => Attribute.IsDefined(m, typeof(RouteAttribute)));
        }

        internal static IEnumerable<MethodInfo> BaseActions()
        {
            return typeof(Entity).
                   GetMethods(Member.Bindings).
                   Where(m => Attribute.IsDefined(m, typeof(BaseRouteAttribute)));
        }

        protected void Launch(Enum output, params object[] sends) { GetRouter().Launch(this, output, sends); }

        private void SetActive(bool value)
        {
            if (gameObject.activeSelf == value) { return; }
            
            gameObject.SetActive(value);

            switch (value)
            {
                case true: Launch(Jape.BaseOutputs.OnEnable); return;
                case false: Launch(Jape.BaseOutputs.OnDisable); return;
            }
        }

        [BaseRoute]
        public void Trigger() { Launch(Jape.BaseOutputs.OnTrigger); }

        [BaseRoute]
        public void Kill() { Destroy(this); }

        [BaseRoute]
        public void Enable() { SetActive(true); }

        [BaseRoute]
        public void Disable() { SetActive(false); }

        [BaseRoute]
        public void Toggle() { SetActive(!gameObject.activeSelf); }

        [BaseRoute]
        public void Write(object input) { this.Log().Response(input); }

        [BaseRoute]
        public virtual void Launch1() { throw new NotImplementedException(); }

        [BaseRoute]
        public virtual void Launch2() { throw new NotImplementedException(); }

        [BaseRoute]
        public virtual void Launch3() { throw new NotImplementedException(); }

        [BaseRoute]
        public virtual void Launch4() { throw new NotImplementedException(); }

        #if UNITY_EDITOR

        internal override void OnEnable()
        {
            base.OnEnable();
            if (Game.IsRunning) { return; }
            UnityEditor.EditorApplication.hierarchyWindowItemOnGUI += SetHierarchyIcon;
        }

        internal override void OnDisable()
        {
            base.OnDisable();
            if (Game.IsRunning) { return; }
            UnityEditor.EditorApplication.hierarchyWindowItemOnGUI -= SetHierarchyIcon;
        }

        internal override void Update()
        {
            base.Update();
            if (Game.IsRunning) { return; }
            Singularize();
            Order();
            SetInspectorIcon(Icon);
        }

        protected virtual void Singularize()
        {
            Component[] components = GetComponents<Component>().
                                     Where(c => c != this).
                                     Where(c => c.GetType() != typeof(Transform) && c.GetType() != typeof(Properties)).
                                     Where(c => Components().All(t => c.GetType().GetInterfaces().All(i => i != t))).
                                     Where(c => Components().All(t => !c.GetType().IsBaseOrSubclassOf(t))).
                                     Reverse().
                                     ToArray();

            foreach (Component component in components)
            {
                this.Log().Response($"Entities cannot contain additional components: {component.GetType()}");
                DestroyImmediate(component);
            }
        }

        private void Order()
        {
            List<Component> components = gameObject.GetComponents<Component>().ToList();
            int index = components.FindIndex(c => c == this);
            Enumeration.Repeat(index - 3, () => UnityEditorInternal.ComponentUtility.MoveComponentUp(this));
        }

        private void SetHierarchyIcon(int instanceID, Rect rect)
        {
            if (Icon == null) { return; }

            if (gameObject.GetInstanceID() != instanceID) { return; }

            rect.width = 16;
            rect.x = 32;

            GUI.Label(rect, Icon); 
        }

        private void SetInspectorIcon(Texture icon)
        {
            if (icon == null) { return; }
            MethodInfo setIcon = typeof(UnityEditor.EditorGUIUtility).GetMethod("SetIconForObject", BindingFlags.Static | BindingFlags.NonPublic, null, new [] { typeof(UnityEngine.Object), typeof(Texture2D) }, null);
            setIcon?.Invoke(null, new object[] { gameObject, icon });
        }

        #endif

        public static IEnumerable<Type> Subclass(Class entityClass)
        {
            IEnumerable<Type> classes = typeof(Entity).GetSubclass();

            switch (entityClass)
            {
                case Class.Point:
                    classes = classes.Where(t => t.IsBaseOrSubclassOf(typeof(PointEntity)));
                    break;

                case Class.World:
                    classes = classes.Where(t => t.IsBaseOrSubclassOf(typeof(WorldEntity)));
                    break;
            }

            return classes;
        }

        private bool Always() { return true; }

        [AttributeUsage(AttributeTargets.Method)]
        public class RouteAttribute : Attribute {}

        [AttributeUsage(AttributeTargets.Method)]
        internal class BaseRouteAttribute : Attribute {}
    }
}