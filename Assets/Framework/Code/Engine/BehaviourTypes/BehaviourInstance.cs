using System;
using System.Collections.Generic;
using System.Linq;

namespace Jape
{
	public abstract class BehaviourInstance
    {
        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
        protected class ArgAttribute : Attribute {}

        internal void Init(IEnumerable<Behaviour.Arg> args) { SetArgs(args); }

        private void SetArgs(IEnumerable<Behaviour.Arg> args) { foreach (Behaviour.Arg arg in args) { Member.Set(this, arg.member, arg.Value); } }

        public static IEnumerable<string> DeclaredArgs(Type behaviourType) { return Member.AllMembers(behaviourType)
                                                                                          .Where(m => Attribute.IsDefined(m, typeof(ArgAttribute)))
                                                                                          .Select(m => m.Name); }

        public static BehaviourInstance Create(Behaviour behaviour, IEnumerable<Behaviour.Arg> args)
        {
            BehaviourInstance instance = behaviour.CreateInstance();
            instance.Init(args);
            return instance;
        }
    }
}