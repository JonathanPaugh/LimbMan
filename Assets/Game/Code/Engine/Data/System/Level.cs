using System;
using UnityEngine;
using Jape;
using Sirenix.Serialization;

namespace Game
{
    public class Level : SystemData
    {
        protected new static string Path => "System/Resources/Levels";

        public string name;

        [SerializeField]
        private Map map = null;
        public Map Map => map;
    }
}