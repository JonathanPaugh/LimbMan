using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jape
{
    public class Team : SystemData
    {
        protected new static string Path => "System/Resources/Teams";
        public static Team Find(string name) { return Find<Team>(name); }
    }
}