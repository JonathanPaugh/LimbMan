using UnityEngine;

namespace Jape
{
    [AddComponentMenu("")]
    public abstract class EntLogic : PointEntity
    {
        protected override Texture2D Icon => GetIcon("IconRelay");
    }
}