using System;
using UnityEngine;

namespace Jape
{
    [AddComponentMenu("")]
    public abstract class EntFunc : PointEntity
    {
        protected override Texture Icon => GetIcon("IconFunc");
    }
}