using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Jape
{
    [Serializable]
    public class ToolChain : SystemData, ToolChain.IChain
    {
        protected new static string Path => IO.JoinPath(SystemPath, "ToolChains");

        [SerializeField, UsedImplicitly]
        [PropertyOrder(3)]
        [ListDrawerSettings(ShowPaging = false, Expanded = true)]
        private IChain[] chain = Array.Empty<IChain>();

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
        [HideReferenceObjectPicker]
        public class Column : IChain
        {
            [SerializeField]
            private Probability size = 100;

            [Space(8)]

            [SerializeField]
            [ListDrawerSettings(ShowPaging = false, Expanded = true)]
            [LabelText("Tools")]
            private List<ToolButton> buttons = new();

            public void Draw(float maxSize)
            {
                GUILayout.BeginVertical();

                foreach (ToolButton button in buttons)
                {
                    button.Draw(maxSize * size);
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