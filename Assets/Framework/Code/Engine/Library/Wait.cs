using System;
using System.Collections;
using UnityEngine;

namespace Jape
{
    public static class Wait
    {
        private static EditorSettings editorSettings;
        private static EditorSettings EditorSettings => editorSettings ?? (editorSettings = Game.Settings<EditorSettings>());

        public static object Frame() { return null; }

        public static object Frames(int count)
        {
            if (count <= 0) { return new Skip(); }
            return Counter(count, Frame());
        }

        public static object Tick() { return new WaitForFixedUpdate(); }

        public static object Editor()
        {
            return Seconds(EditorSettings.processingRate);
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

        public class Skip {}
    }
}