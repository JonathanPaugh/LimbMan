using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    [AddComponentMenu("")]
    public class EntLogicCase : EntLogic
    {
        protected override Texture Icon => GetIcon("IconCase"); 

        public override Enum Outputs() { return CaseOutputsFlags.OnCase1 | 
                                                CaseOutputsFlags.OnCase2 | 
                                                CaseOutputsFlags.OnCase3 | 
                                                CaseOutputsFlags.OnCase4 |
                                                CaseOutputsFlags.OnCase5 | 
                                                CaseOutputsFlags.OnCase6 | 
                                                CaseOutputsFlags.OnCase7 | 
                                                CaseOutputsFlags.OnCase8 | 
                                                CaseOutputsFlags.OnCase9 | 
                                                CaseOutputsFlags.OnCase10 | 
                                                CaseOutputsFlags.OnCase11 | 
                                                CaseOutputsFlags.OnCase12 | 
                                                CaseOutputsFlags.OnCase13 | 
                                                CaseOutputsFlags.OnCase14 | 
                                                CaseOutputsFlags.OnCase15 | 
                                                CaseOutputsFlags.OnCase16; }

        [SerializeField] [Eject] private Cases cases;

        [Route]
        public virtual void MatchFirst(string value)
        {
            CaseOutputsFlags matches = cases.FindMatches(value);

            if (matches == CaseOutputsFlags.OnNone) { Launch(CaseOutputsFlags.OnNone); return; }

            Launch(CaseOutputsFlags.OnAny);
            Launch(CaseOutputsFlags.OnMatch);

            if (matches.HasFlag(CaseOutputsFlags.OnCase1)) { Launch(CaseOutputsFlags.OnCase1); return; }
            if (matches.HasFlag(CaseOutputsFlags.OnCase2)) { Launch(CaseOutputsFlags.OnCase2); return; }
            if (matches.HasFlag(CaseOutputsFlags.OnCase3)) { Launch(CaseOutputsFlags.OnCase3); return; }
            if (matches.HasFlag(CaseOutputsFlags.OnCase4)) { Launch(CaseOutputsFlags.OnCase4); return; }
            if (matches.HasFlag(CaseOutputsFlags.OnCase5)) { Launch(CaseOutputsFlags.OnCase5); return; }
            if (matches.HasFlag(CaseOutputsFlags.OnCase6)) { Launch(CaseOutputsFlags.OnCase6); return; }
            if (matches.HasFlag(CaseOutputsFlags.OnCase7)) { Launch(CaseOutputsFlags.OnCase7); return; }
            if (matches.HasFlag(CaseOutputsFlags.OnCase8)) { Launch(CaseOutputsFlags.OnCase8); return; }
            if (matches.HasFlag(CaseOutputsFlags.OnCase9)) { Launch(CaseOutputsFlags.OnCase9); return; }
            if (matches.HasFlag(CaseOutputsFlags.OnCase10)) { Launch(CaseOutputsFlags.OnCase10); return; }
            if (matches.HasFlag(CaseOutputsFlags.OnCase11)) { Launch(CaseOutputsFlags.OnCase11); return; }
            if (matches.HasFlag(CaseOutputsFlags.OnCase12)) { Launch(CaseOutputsFlags.OnCase12); return; }
            if (matches.HasFlag(CaseOutputsFlags.OnCase13)) { Launch(CaseOutputsFlags.OnCase13); return; }
            if (matches.HasFlag(CaseOutputsFlags.OnCase14)) { Launch(CaseOutputsFlags.OnCase14); return; }
            if (matches.HasFlag(CaseOutputsFlags.OnCase15)) { Launch(CaseOutputsFlags.OnCase15); return; }
            if (matches.HasFlag(CaseOutputsFlags.OnCase16)) { Launch(CaseOutputsFlags.OnCase16); return; }
        }

        [Route]
        public virtual void MatchAny(string value)
        {
            CaseOutputsFlags matches = cases.FindMatches(value);

            if (matches == CaseOutputsFlags.OnNone) { Launch(CaseOutputsFlags.OnNone); return; }

            Launch(CaseOutputsFlags.OnAny);

            if (matches.HasFlag(CaseOutputsFlags.OnCase1)) { Launch(CaseOutputsFlags.OnMatch); Launch(CaseOutputsFlags.OnCase1); }
            if (matches.HasFlag(CaseOutputsFlags.OnCase2)) { Launch(CaseOutputsFlags.OnMatch); Launch(CaseOutputsFlags.OnCase2); }
            if (matches.HasFlag(CaseOutputsFlags.OnCase3)) { Launch(CaseOutputsFlags.OnMatch); Launch(CaseOutputsFlags.OnCase3); }
            if (matches.HasFlag(CaseOutputsFlags.OnCase4)) { Launch(CaseOutputsFlags.OnMatch); Launch(CaseOutputsFlags.OnCase4); }
            if (matches.HasFlag(CaseOutputsFlags.OnCase5)) { Launch(CaseOutputsFlags.OnMatch); Launch(CaseOutputsFlags.OnCase5); }
            if (matches.HasFlag(CaseOutputsFlags.OnCase6)) { Launch(CaseOutputsFlags.OnMatch); Launch(CaseOutputsFlags.OnCase6); }
            if (matches.HasFlag(CaseOutputsFlags.OnCase7)) { Launch(CaseOutputsFlags.OnMatch); Launch(CaseOutputsFlags.OnCase7); }
            if (matches.HasFlag(CaseOutputsFlags.OnCase8)) { Launch(CaseOutputsFlags.OnMatch); Launch(CaseOutputsFlags.OnCase8); }
            if (matches.HasFlag(CaseOutputsFlags.OnCase9)) { Launch(CaseOutputsFlags.OnMatch); Launch(CaseOutputsFlags.OnCase9); }
            if (matches.HasFlag(CaseOutputsFlags.OnCase10)) { Launch(CaseOutputsFlags.OnMatch); Launch(CaseOutputsFlags.OnCase10); }
            if (matches.HasFlag(CaseOutputsFlags.OnCase11)) { Launch(CaseOutputsFlags.OnMatch); Launch(CaseOutputsFlags.OnCase11); }
            if (matches.HasFlag(CaseOutputsFlags.OnCase12)) { Launch(CaseOutputsFlags.OnMatch); Launch(CaseOutputsFlags.OnCase12); }
            if (matches.HasFlag(CaseOutputsFlags.OnCase13)) { Launch(CaseOutputsFlags.OnMatch); Launch(CaseOutputsFlags.OnCase13); }
            if (matches.HasFlag(CaseOutputsFlags.OnCase14)) { Launch(CaseOutputsFlags.OnMatch); Launch(CaseOutputsFlags.OnCase14); }
            if (matches.HasFlag(CaseOutputsFlags.OnCase15)) { Launch(CaseOutputsFlags.OnMatch); Launch(CaseOutputsFlags.OnCase15); }
            if (matches.HasFlag(CaseOutputsFlags.OnCase16)) { Launch(CaseOutputsFlags.OnMatch); Launch(CaseOutputsFlags.OnCase16); }
        }

        [Serializable]
        public struct Cases
        {
            public string case1;
            [ShowIf(nameof(ValidateCase1))] public string case2;
            [ShowIf(nameof(ValidateCase2))] public string case3;
            [ShowIf(nameof(ValidateCase3))] public string case4;
            [ShowIf(nameof(ValidateCase4))] public string case5;
            [ShowIf(nameof(ValidateCase5))] public string case6;
            [ShowIf(nameof(ValidateCase6))] public string case7;
            [ShowIf(nameof(ValidateCase7))] public string case8;
            [ShowIf(nameof(ValidateCase8))] public string case9;
            [ShowIf(nameof(ValidateCase9))] public string case10;
            [ShowIf(nameof(ValidateCase10))] public string case11;
            [ShowIf(nameof(ValidateCase11))] public string case12;
            [ShowIf(nameof(ValidateCase12))] public string case13;
            [ShowIf(nameof(ValidateCase13))] public string case14;
            [ShowIf(nameof(ValidateCase14))] public string case15;
            [ShowIf(nameof(ValidateCase15))] public string case16;

            public bool ValidateCase1() { return !string.IsNullOrEmpty(case1); }
            public bool ValidateCase2() { return !string.IsNullOrEmpty(case2); }
            public bool ValidateCase3() { return !string.IsNullOrEmpty(case3); }
            public bool ValidateCase4() { return !string.IsNullOrEmpty(case4); }
            public bool ValidateCase5() { return !string.IsNullOrEmpty(case5); }
            public bool ValidateCase6() { return !string.IsNullOrEmpty(case6); }
            public bool ValidateCase7() { return !string.IsNullOrEmpty(case7); }
            public bool ValidateCase8() { return !string.IsNullOrEmpty(case8); }
            public bool ValidateCase9() { return !string.IsNullOrEmpty(case9); }
            public bool ValidateCase10() { return !string.IsNullOrEmpty(case10); }
            public bool ValidateCase11() { return !string.IsNullOrEmpty(case11); }
            public bool ValidateCase12() { return !string.IsNullOrEmpty(case12); }
            public bool ValidateCase13() { return !string.IsNullOrEmpty(case13); }
            public bool ValidateCase14() { return !string.IsNullOrEmpty(case14); }
            public bool ValidateCase15() { return !string.IsNullOrEmpty(case15); }
            public bool ValidateCase16() { return !string.IsNullOrEmpty(case16); }

            public CaseOutputsFlags FindMatches(string value)
            {
                CaseOutputsFlags matches = CaseOutputsFlags.OnNone;

                if (value == case1) { matches |= CaseOutputsFlags.OnCase1; }
                if (value == case2) { matches |= CaseOutputsFlags.OnCase2; }
                if (value == case3) { matches |= CaseOutputsFlags.OnCase3; }
                if (value == case4) { matches |= CaseOutputsFlags.OnCase4; }
                if (value == case5) { matches |= CaseOutputsFlags.OnCase5; }
                if (value == case6) { matches |= CaseOutputsFlags.OnCase6; }
                if (value == case7) { matches |= CaseOutputsFlags.OnCase7; }
                if (value == case8) { matches |= CaseOutputsFlags.OnCase8; }
                if (value == case9) { matches |= CaseOutputsFlags.OnCase9; }
                if (value == case10) { matches |= CaseOutputsFlags.OnCase10; }
                if (value == case11) { matches |= CaseOutputsFlags.OnCase11; }
                if (value == case12) { matches |= CaseOutputsFlags.OnCase12; }
                if (value == case13) { matches |= CaseOutputsFlags.OnCase13; }
                if (value == case14) { matches |= CaseOutputsFlags.OnCase14; }
                if (value == case15) { matches |= CaseOutputsFlags.OnCase15; }
                if (value == case16) { matches |= CaseOutputsFlags.OnCase16; }

                return matches;
            }
        }
    }
}