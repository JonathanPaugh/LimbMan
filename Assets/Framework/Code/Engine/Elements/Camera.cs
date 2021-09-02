using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    [DisallowMultipleComponent]
    public class Camera : Element
    {
        [SerializeField]
        [HideInPlayMode]
        protected new UnityEngine.Camera camera;

        [SerializeField]
        [HideInPlayMode]
        private Overlay overlayPrefab = null;

        [NonSerialized, HideInInspector]
        public Overlay overlay;

        protected override void Init()
        {
            if (camera == null) { camera = GetComponent<UnityEngine.Camera>(); }
        }

        protected override void First()
        {
            if (camera == null) { return; }
            overlay = Game.CloneGameObject(overlayPrefab.gameObject).GetComponent<Overlay>();
            DontDestroyOnLoad(overlay.gameObject);
            overlay.SetCamera(camera);
        }
    }
}