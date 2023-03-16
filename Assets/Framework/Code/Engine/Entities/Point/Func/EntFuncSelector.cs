using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Jape
{
    [AddComponentMenu("")]
    public class EntFuncSelector : EntFunc
    {
        public override Enum BaseOutputs() { return BaseOutputsFlags.None | 
                                                    BaseOutputsFlags.OnValue |
                                                    BaseOutputsFlags.OnTrigger | 
                                                    BaseOutputsFlags.OnDestroy |
                                                    BaseOutputsFlags.OnLaunch1 |
                                                    BaseOutputsFlags.OnLaunch2 |
                                                    BaseOutputsFlags.OnLaunch3 |
                                                    BaseOutputsFlags.OnLaunch4; }

        public override IEnumerable<Send> Sends()
        {
            return new [] { new Send(Jape.BaseOutputs.OnValue, typeof(object), "Value") };
        }

        [SerializeField] private MonoBehaviour target = null;
        [SerializeField] private string selector = string.Empty;
        [SerializeField] private string[] selectorArgs = null;

        [Route]
        public void Get()
        {
            object[] args = selectorArgs.Select(a => Selector.Value(target, a)).ToArray();
            Launch(Jape.BaseOutputs.OnValue, Selector.Value(target, selector, args));
        }
    }
}