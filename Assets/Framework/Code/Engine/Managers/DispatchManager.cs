using System.Collections;
using UnityEngine;

namespace Jape
{
    [ExecuteAlways]
    public sealed class DispatchManager : Manager<DispatchManager>
    {
        private new static bool InitOnLoad => true;

        protected override void FrameEditor()
        {
            if (Application.isPlaying) { return; }
            DestroyImmediate(gameObject);
        }

        public Coroutine Dispatch(IEnumerator routine)
        {
            return routine == null ? null : StartCoroutine(routine);
        }

        public void Recall(Coroutine coroutine)
        {
            if (coroutine == null) { return; }
            StopCoroutine(coroutine);
        }
    }
}