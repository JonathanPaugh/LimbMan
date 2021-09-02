using System;
using UnityEngine;

namespace Jape
{
    [AddComponentMenu("")]
    public abstract class EntInfo : PointEntity
    {
        protected override Texture Icon => GetIcon("IconInfo");
    }
}