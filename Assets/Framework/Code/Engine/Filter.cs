using System;
using System.Linq;
using UnityEngine;

namespace Jape
{
    [Serializable]
    public class Filter
    {
        public enum Mode { Reject, Allow }
        public Mode mode;
            
        [Space(8)]

        public Tag[] tags;
        
        public bool Reject() { return mode == Mode.Reject; }
        public bool Allow() { return mode == Mode.Allow; }

        public bool Evaluate(GameObject gameObject)
        {
            switch (mode)
            {
                case Mode.Reject: return !tags.Any(gameObject.HasTag); 
                case Mode.Allow: return tags.Any(gameObject.HasTag);
            }
            return true;
        }
    }
}