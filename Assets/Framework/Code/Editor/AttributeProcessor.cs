using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector.Editor;

namespace JapeEditor
{
	public class AttributeProcessor<T> : OdinAttributeProcessor<T>
    {
        public virtual void ProcessSelf(InspectorProperty property, List<Attribute> attributes) {}
        public virtual void ProcessChildren(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes) {}

        public sealed override void ProcessSelfAttributes(InspectorProperty property, List<Attribute> attributes)
        {
            ProcessSelf(property, attributes);
        }

        public sealed override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            ProcessChildren(parentProperty, member, attributes);
        }
    }
}