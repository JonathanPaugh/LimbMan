using System;
using UnityEngine;

namespace Jape
{
    [AddComponentMenu("")]
    public class EntFuncGlobal : EntFunc
    {
        protected override Texture Icon => base.Icon;

        [SerializeField]
        private Global global = null;

        [Route]
        public void SetGlobal(object value)
        {
            global.SetValue(Convert.ChangeType(value, global.GetValue().GetType()) );
        }
    }
}