using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    [AddComponentMenu("")]
    public class EntWorldTeleport : WorldEntity
    {
        protected override Texture Icon => GetIcon("IconTeleport");

        [Space(16)]

        [SerializeField]
        [SceneObjectsOnly]
        private GameObject target = null;

        protected override void FrameEditor() { DrawLine(); }

        protected override void TouchAction(GameObject gameObject)
        {
            gameObject.transform.position = target.transform.position;
        }

        public void DrawLine()
        {
            if (target == null) { return; }
            Debug.DrawLine(transform.position, target.transform.position);
        }
    }
}