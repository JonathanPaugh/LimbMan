using UnityEngine;

namespace Jape
{
    [AddComponentMenu("")]
    public class EntInfoTarget : EntInfo
    {
        protected override Texture2D Icon => GetIcon("IconTarget");
    }
}