using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    [DisallowMultipleComponent]
    public class Overlay : Element
    {
        [SerializeField]
        [HideInPlayMode]
        private Canvas canvas = null;

        private UnityEngine.Camera Camera => canvas.worldCamera;

        private Flash flash;

        private UnityEngine.UI.Image fill;
        private UnityEngine.UI.Image Fill
        {
            get
            {
                if (fill != null) { return fill; }
                fill = GetComponentInChildren<UnityEngine.UI.Image>();
                return fill;
            }
        }

        internal void SetCamera(UnityEngine.Camera camera) { canvas.worldCamera = camera; } 
        
        protected override void Init()
        {
            flash = CreateFlash().Set(Fill.color).ReturnAction((c) => Fill.color = c);
        }

        protected override void First()
        {
            if (Camera == null)
            {
                SetCamera(Game.Find<UnityEngine.Camera>().FirstOrDefault());
            }
        }

        protected override void Frame()
        {
            canvas.planeDistance = 1;
        }

        public void SetColor(Color color)
        {
            Fill.color = color;
            flash.Set(Fill.color);
        }

        public void FlashColor(Color color, float time) { flash.CreateFlash(color, time); }
        public void FadeColor(Color color, float time) { flash.CreateFade(color, time); }
    }
}