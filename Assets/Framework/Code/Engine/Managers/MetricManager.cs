using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace Jape
{
    public sealed class MetricManager : Manager<MetricManager> 
    {
        private OrderedDictionary metrics = new OrderedDictionary();

        public static void Set(string metric, string value)
        {
            if (IsQuitting()) { return; }

            if (Instance.metrics.Contains(metric))
            {
                Instance.metrics[metric] = value;
            }
            else
            {
                Instance.metrics.Add(metric, value);
            }
        }

        public static void Remove(string metric)
        {
            if (IsQuitting()) { return; }

            if (!Instance.metrics.Contains(metric))
            {
                Instance.Log().Warning("Metric does not exist");
                return;
            }

            Instance.metrics.Remove(metric);
        }

        private int spaces;
        public static string Space()
        {
            string key = $"Space{Instance.spaces}";
            Set(key, null);
            Instance.spaces++;
            return key;
        }

        private GUIContent label;
        private Vector2 size;
        protected override void Draw()
        {
            int i = -1;
            foreach (DictionaryEntry metric in metrics)
            {
                i++;
                if (metric.Value == null) { continue; }
                label = new GUIContent($"{metric.Key}: {metric.Value}");
                size = GUI.skin.label.CalcSize(label);
                GUI.Label(new Rect(Screen.currentResolution.width - size.x - 8, i * 16, 128, 32), label);
            }
        }
    }
}