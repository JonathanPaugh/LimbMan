using System.Collections;
using System.Collections.Generic;

namespace Jape
{
    public class DenumManager : AssetManager<DenumManager>
    {
        public DenumManager() { Instance = this; }

        private Queue<Denum> queue = new();
        private EditorJob job = (EditorJob)new EditorJob().ChangeMode(Job.Mode.Loop);

        protected override void EnabledEditor()
        {
            if (Module.IsAlive(job)) { return; }
            job.Set(RoutineEditor()).Start();
        }

        protected override void DisabledEditor() { job.Stop(); }

        private IEnumerable RoutineEditor()
        {
            #if UNITY_EDITOR
            while (queue.Count > 0)
            {
                if (UnityEditor.EditorGUIUtility.editingTextField) { break; }
                Denum denum = queue.Dequeue();
                denum.RewriteEditor();
            }
            yield return Wait.EditorFrame();
            #else
            yield return null;
            #endif
        }

        internal static void QueueRewriteEditor(Denum denum)
        {
            #if UNITY_EDITOR
            Instance.queue.Enqueue(denum);
            #endif
        }
    }
}