using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    [AddComponentMenu("")]
    [ExecuteAlways]
    public class EntInfoTrack : EntInfo
    {
        protected override Texture2D Icon => GetIcon("IconTrack");

        public override Enum Outputs() { return TrackOutputsFlags.OnPass; }

        [PropertyOrder(-1)]
        [Button(ButtonSizes.Large)]
        [HideIf(Game.GameIsRunning)]
        [LabelText("Continue Track")]
        protected void ContinueTrackEditor()
        {
            #if UNITY_EDITOR
            EntInfoTrack next = Game.CloneGameObject(gameObject, transform.position, transform.rotation, transform.parent).GetComponent<EntInfoTrack>();
            target = next;
            UnityEditor.Selection.activeObject = next.gameObject;
            #endif
        }

        [PropertySpace(8)]

        public EntInfoTrack target;

        [Tooltip("-1 uses train speed")] public float speed = -1;
        

        public float delay = 0;

        [Route]
        public void ChangeSpeed(float speed) { this.speed = speed; }

        protected override void FrameEditor() { DrawLine(); }

        internal void Pass() { Launch(TrackOutputs.OnPass); }

        public void DrawLine()
        {
            if (target == null) { return; }
            Debug.DrawLine(transform.position, target.transform.position);
        }
    }
}