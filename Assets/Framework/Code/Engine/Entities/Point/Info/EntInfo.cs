using UnityEngine;

namespace Jape
{
    [AddComponentMenu("")]
    public abstract class EntInfo : PointEntity
    {
        protected override Texture2D Icon => GetIcon("IconInfo");
    }
}