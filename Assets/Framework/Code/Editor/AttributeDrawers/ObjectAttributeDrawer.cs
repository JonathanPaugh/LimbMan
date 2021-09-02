using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Jape;
using Sirenix.OdinInspector.Editor.ValueResolvers;

using Object = UnityEngine.Object;

namespace JapeEditor
{
	public class ObjectAttributeDrawer : AttributeDrawer<ObjectAttribute>
	{
        private List<ValueResolver<bool>> filterResolvers = new List<ValueResolver<bool>>();
        private List<Func<Object, bool>> filters = new List<Func<Object, bool>>();

        protected override void Initialize()
        {
            foreach(string methodName in Attribute.MethodNames)
            {

                filterResolvers.Add(ValueResolver.Get<bool>(Property, methodName, new NamedValue("item", typeof(Object))));
            }
        }

        protected override void Draw(GUIContent label) 
		{
            foreach (ValueResolver<bool> resolver in filterResolvers.Where(resolver => resolver.HasError))
            {
                resolver.DrawError();
            }

		    GUILayout.BeginHorizontal();

            CallNextDrawer(label);

            if (!Attribute.HidePicker) { DrawPicker(); }

            GUILayout.EndHorizontal();
        }

        private void DrawPicker()
        {
            if (GUILayout.Button(new GUIContent(Database.GetAsset("IconPick").Load<Texture>()), 
                new GUIStyle(GUI.skin.button).Padding(new RectOffset(1, 1, 1, 1)), 
                GUILayout.Width(18), 
                GUILayout.Height(18)))
            {
                PickerWindow window = PickerWindow.Call(Property.ValueEntry.BaseValueType, SetValue, (PickerWindow.Mode)Attribute.PickerMode);

                foreach (ValueResolver<bool> resolver in filterResolvers.Where(r => !r.HasError))
                {
                    filters.Add(delegate(Object value)
                    {
                        resolver.Context.NamedValues.Set("item", value);
                        return resolver.GetValue();
                    });
                }

                foreach (Func<Object, bool> filter in filters)
                {
                    window.AddFilter(filter);
                }

                window.RefreshItems();
            }
        }

		public void SetValue(Object value)
        {
		    Property.ValueEntry.WeakSmartValue = value;
        }
	}
}