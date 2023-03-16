using UnityEngine;

namespace Jape
{
    [AddComponentMenu("")]
    public abstract class EntFunc : PointEntity
    {
        protected override Texture2D Icon => GetIcon("IconFunc");
    }
}