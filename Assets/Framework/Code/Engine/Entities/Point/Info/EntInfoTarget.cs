using System;
using UnityEngine;

namespace Jape
{
    [AddComponentMenu("")]
    public class EntInfoTarget : EntInfo
    {
        protected override Texture Icon => GetIcon("IconTarget");
    }
}