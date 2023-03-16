using System;
using System.Collections;
using UnityEngine;

namespace Jape
{
    public static class Wait
    {
        private static EditorSettings editorSettings;
        private static EditorSettings EditorSettings => editorSettings != null ? editorSettings : editorSettings = Framework.Settings<EditorSettings>();

        public static object Frame() { return null; }

        public static object Frames(int count)
        {
            if (count <= 0) { return new Skip(); }
            return Counter(count, Frame());
        }

        public static object Tick() { return new WaitForFixedUpdate(); }

        public static object EditorFrame()
        {
            return new WaitForEditorFrame();
        }

        public static object Seconds(float count)
        {
            if (count <= 0) { return new Skip(); }
            return new WaitForSeconds(count);
        }

        public static object Realtime(float count)
        {
            if (count <= 0) { return new Skip(); }
            return new WaitForSecondsRealtime(count);
        }

        public static object Until(Func<bool> requirement)
        {
            if (requirement()) { return new Skip(); }
            return new WaitUntil(requirement);
        }

        public static object While(Func<bool> requirement)
        {
            if (!requirement()) { return new Skip(); }
            return new WaitWhile(requirement);
        }

        private static IEnumerator Counter(int count, object yield)
        {
            while (count > 0) {
                count--;
                yield return yield;
            }
        }

        internal class Skip {}

        internal class WaitForEditorFrame : CustomYieldInstruction
        {
            private bool isDone;
            public override bool keepWaiting => isDone;

            internal WaitForEditorFrame()
            {
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.update += TickEditor;
                #else
                throw new Exception("Attempted to wait for editor frame without editor");
                #endif
            } 

            private void TickEditor()
            {
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.update -= TickEditor;
                isDone = true;
                #endif
            }
        };
    }
}