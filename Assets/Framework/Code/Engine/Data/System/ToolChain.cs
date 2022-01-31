using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Jape
{
    [Serializable]
    public class ToolChain : SystemData, ToolChain.IChain
    {
        protected new static string Path => "System/Resources/ToolChains";

        [OdinSerialize, ShowInInspector]
        [PropertyOrder(3)]
        [LabelText("Tools")]
        [ListDrawerSettings(ShowPaging = false)]
        private IChain[] chain;

        public IChain[] Chain => chain;

        public void Draw(float maxSize)
        {
            GUILayout.BeginHorizontal();

            foreach (IChain column in chain)
            {
                column.Draw(maxSize);
            }

            GUILayout.EndHorizontal();
        }

        public float GetColumnCount()
        {
            return chain.Sum(chain => chain.GetColumnCount());
        }

        [Serializable]
        public class Column : List<ToolButton>, IChain
        {
            public void Draw(float maxSize)
            {
                GUILayout.BeginVertical();

                foreach (ToolButton button in this)
                {
                    button.Draw(maxSize);
                }

                GUILayout.EndVertical();
            }

            public float GetColumnCount() => 1;
        }

        public interface IChain
        {
            void Draw(float maxSize);
            float GetColumnCount();
        }
    }
}