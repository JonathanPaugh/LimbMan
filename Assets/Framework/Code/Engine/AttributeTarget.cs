using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Jape
{
    public abstract class AttributeTarget<TTarget, TAttribute> where TAttribute : Attribute
    {
        public TTarget Target { get; }
        public TAttribute Attribute { get; }

        internal AttributeTarget(TTarget target, TAttribute attribute)
        {
            Target = target;
            Attribute = attribute;
        }

        public void Deconstruct(out TTarget target, out TAttribute attribute)
        {
            target = Target;
            attribute = Attribute;
        }
    } 

    public class AttributeField<T> : AttributeTarget<FieldInfo, T> where T : Attribute
    {
        internal AttributeField(FieldInfo target, T attribute) : base(target, attribute) {}

        public static IEnumerable<AttributeField<T>> FromTargets(IEnumerable<FieldInfo> targets, bool inherit)
        {
            return targets
                .Where(f => f.IsDefined(typeof(T), inherit))
                .Select(f => new AttributeField<T>(f, f.GetCustomAttribute<T>(inherit)));
        }
    }

    public class AttributeClass<T> : AttributeTarget<Type, T> where T : Attribute
    {
        internal AttributeClass(Type target, T attribute) : base(target, attribute) {}

        public static IEnumerable<AttributeClass<T>> FromTargets(IEnumerable<Type> targets, bool inherit)
        {
            return targets
                .Where(f => f.IsDefined(typeof(T), inherit))
                .Select(f => new AttributeClass<T>(f, f.GetCustomAttribute<T>(inherit)));
        }
    } 
}