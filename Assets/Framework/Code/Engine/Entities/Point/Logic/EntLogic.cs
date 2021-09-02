using System;
using UnityEngine;

namespace Jape
{
    [AddComponentMenu("")]
    public abstract class EntLogic : PointEntity
    {
        protected override Texture Icon => GetIcon("IconRelay");
    }
}