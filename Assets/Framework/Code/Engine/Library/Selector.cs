using System;
using System.Linq;

namespace Jape
{
    public static class Selector
    {
        public const string Indicator = "$";
        public const string Global = "#";

        public enum Mode { Member, Global }

        public static object Value(object instance, string input, params object[] args)
        {
            if (string.IsNullOrEmpty(input)) { return input; }
            if (!Indicated(input, out Mode mode)) { return input; }

            Member member;
            switch (mode)
            {
                case Mode.Member:
                    member = new Member(instance, GetIndicated(input));
                    break;

                case Mode.Global:
                    member = new Member(DataType.FindAll<Global>().FirstOrDefault(g => g.name == GetIndicated(input)), "Value");
                    break;

                default: return null;
            }

            if (IsAccessor(input)) { return Value(member.Get(), input.Substring(input.Substring(1).IndexOf(Indicator, StringComparison.Ordinal) + 1), args) ?? input; }

            return member.Get(args) ?? input;
        }

        internal static object GlobalValue(string input, params object[] args)
        {
            if (string.IsNullOrEmpty(input)) { return input; }
            if (!Indicated(input, out Mode mode)) { return input; }

            switch (mode)
            {
                case Mode.Member:
                    Log.Write("Global value can only be used for globals");
                    return input;
            }

            Member member = new(DataType.FindAll<Global>().FirstOrDefault(g => g.name == GetIndicated(input)), "value");

            if (IsAccessor(input)) { return Value(member.Get(), input.Substring(input.Substring(1).IndexOf(Indicator, StringComparison.Ordinal) + 1), args) ?? input; }

            return member.Get(args) ?? input;
        }

        public static bool Indicated(string input, out Mode mode)
        {
            if (input[0] == Indicator[0])
            {
                mode = Mode.Member;
                return true;
            }
            if (input[0] == Global[0])
            {
                mode = Mode.Global;
                if (input.Substring(1).Contains(Global)) { Log.Write("Global must be used only at the start of selector"); return false; }
                return true;
            }
            mode = Mode.Member;
            return false;
        }

        public static bool IsAccessor(string input) { return input.Substring(1).Contains(Indicator); }

        public static string GetIndicated(string input)
        {
            return IsAccessor(input) ? 
                   input.Substring(1, input.Substring(1).IndexOf(Indicator, StringComparison.Ordinal)) : 
                   input.Substring(1);
        }
    }
}