using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector.Editor;
using Jape;
using Sirenix.OdinInspector;

using Object = UnityEngine.Object;

namespace JapeEditor
{
	public class ObjectAttributeProcessor<T> : AttributeProcessor<T> where T : Object
	{
		public override void ProcessSelf(InspectorProperty property, List<Attribute> attributes)
        {
            ObjectAttribute objectAttribute = new ObjectAttribute();

            if (attributes.Any(a => a is AssetsOnlyAttribute))
            {
                objectAttribute.PickerMode = 1;
            }

            if (attributes.Any(a => a is SceneObjectsOnlyAttribute))
            {
                objectAttribute.PickerMode = 2;
            }

            if (attributes.Any(a => a is HidePickerAttribute))
            {
                objectAttribute.HidePicker = true;
            }

            foreach(PickFilterAttribute attribute in attributes.Where(a => a.GetType() == typeof(PickFilterAttribute)).Cast<PickFilterAttribute>())
            {
                objectAttribute.MethodNames.Add(attribute.MethodName);
            }

            attributes.Add(objectAttribute);
        }

        public override void ProcessChildren(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes) {}
	}
}