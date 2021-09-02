using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jape
{
    public class DenumManager : AssetManager<DenumManager>
    {
        public DenumManager() { Instance = this; }

        #if UNITY_EDITOR

        private Queue<Denum> queue = new Queue<Denum>();

        private EditorJob job = (EditorJob)new EditorJob().ChangeMode(Job.Mode.Loop);

        protected override void EnabledEditor()
        {
            if (Module.IsAlive(job)) { return; }
            job.Set(Routine()).Start();
        }

        protected override void DisabledEditor() { job.Stop(); }

        private IEnumerable Routine()
        {
            while (queue.Count > 0)
            {
                if (UnityEditor.EditorGUIUtility.editingTextField) { break; }
                Denum denum = queue.Dequeue();
                denum.Rewrite();
            }
            yield return Wait.Editor();
        }

        #endif

        internal static void QueueRewrite(Denum denum)
        {
            #if UNITY_EDITOR
            Instance.queue.Enqueue(denum);
            #endif
        }
    }
}