using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Jape
{
	public partial class Entity
    {
        public partial class Routing
        {
            public static class Selector
            {
                public const string Indicator = "!";

                public static object TargetValue(Routing routing, Entity caller, string input)
                {
                    if (string.IsNullOrEmpty(input)) { return input; }
                    if (!ParameterIndicated(input)) { return input; }

                    int length = SelectorIndicated(input) ? 
                                 input.IndexOf(Jape.Selector.Indicator, StringComparison.Ordinal) - 1 : 
                                 input.Length - 1;

                    string indicated = input.Substring(1, length);

                    object instance = null;

                    switch (indicated)
                    {
                        case "self": instance = routing.entity; break;
                        case "caller": instance = caller; break;
                    }

                    if (instance == null) { return input; }

                    return instance;
                }

                public static object ParameterValue(Routing routing, Entity caller, string input, object[] sends)
                {
                    if (string.IsNullOrEmpty(input)) { return input; }
                    if (!ParameterIndicated(input)) { return input; }

                    if (Global(input)) { return Jape.Selector.GlobalValue(input); }

                    int length = SelectorIndicated(input) ? 
                                 input.IndexOf(Jape.Selector.Indicator, StringComparison.Ordinal) - 1 : 
                                 input.Length - 1;

                    string indicated = input.Substring(1, length);

                    object instance = null;

                    if (Regex.IsMatch(indicated, @"send\d*$"))
                    {
                        Match match = Regex.Match(indicated, @"\d+$");
                        instance = match.Success ? 
                                   sends.ToList().ElementAtOrDefault(int.Parse(match.Value) - 1) :
                                   sends.FirstOrDefault();
                    }
                    
                    if (Regex.IsMatch(indicated, @"target\d*$"))
                    {
                        Match match = Regex.Match(indicated, @"\d+$");
                        instance = match.Success ? 
                                   routing.GetTargets().ToList().ElementAtOrDefault(int.Parse(match.Value) - 1) :
                                   routing.GetTargets().FirstOrDefault();
                    }

                    switch (indicated)
                    {
                        case "self": instance = routing.entity; break;
                        case "caller": instance = caller; break;
                    }

                    if (instance == null) { return input; }

                    return SelectorIndicated(input) ? 
                           Jape.Selector.Value(instance, input.Substring(input.IndexOf(Jape.Selector.Indicator, StringComparison.Ordinal))) : 
                           instance;
                }

                private static bool SelectorIndicated(string input) { return input.Contains(Jape.Selector.Indicator); }
                public static bool TargetIndicated(string input) { return input[0] == Indicator[0]; }
                public static bool ParameterIndicated(string input) { return input[0] == Indicator[0] || input[0] == Jape.Selector.Global[0]; }
                public static bool Global(string input) { return input[0] == Jape.Selector.Global[0]; }
            }
        }
    }
}