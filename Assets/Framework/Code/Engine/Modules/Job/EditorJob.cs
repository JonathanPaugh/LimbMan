using System;
using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif

namespace Jape
{
    [ExecuteAlways]
    public class EditorJob : Job
    {
        #if UNITY_EDITOR

        private EditorCoroutine editorCoroutine;

        protected override void Dispatch()
        {
            if (Enumeration() == null) { return; }

            editorCoroutine = EditorCoroutineUtility.StartCoroutine(Enumeration(), this);
        }

        protected override void Recall()
        {
            if (editorCoroutine == null) { return; }

            EditorCoroutineUtility.StopCoroutine(editorCoroutine);
        }

        #endif
    }
}