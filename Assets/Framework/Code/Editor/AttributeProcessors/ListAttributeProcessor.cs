using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;

namespace JapeEditor
{
	public class ListAttributeProcessor<T> : AttributeProcessor<List<T>>
	{
		public override void ProcessSelf(InspectorProperty property, List<Attribute> attributes)
        {
            if (attributes.Any(a => a is ListDrawerSettingsAttribute)) { return; }
            ListDrawerSettingsAttribute attribute = new()
            {
                AlwaysAddDefaultValue = true,
                Expanded = true,
                ShowPaging = false
            };
            attributes.Add(attribute);
        }
        public override void ProcessChildren(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes) {}
	}
}