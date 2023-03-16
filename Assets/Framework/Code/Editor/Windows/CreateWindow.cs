using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace JapeEditor
{
    public abstract class CreateWindow : Window
    {
        private const float ButtonHeight = 32;

        protected override Display DisplayMode => Display.Popup;

        protected override bool AutoHeight => true;
        protected override float Width => 256;

        [SerializeField]
        [HideLabel]
        [ValueDropdown(nameof(Selections))]
        protected object Selection { get; private set; }

        protected virtual Action<object> CreateAction => null; 

        protected abstract IList<object> Selections();

        protected void ResetSelection() { Selection = null; }

        protected bool IsSet() { return Selection != null; }

        protected override void Draw() { CreateButton(); }

        private void CreateButton()
        {
            if (CreateAction == null) { return; }
            if (!IsSet()) { GUIHelper.PushGUIEnabled(false); }
            if (GUILayout.Button("Create", GUILayout.Height(ButtonHeight))) { CreateAction.Invoke(Selection); }
            if (!IsSet()) { GUIHelper.PopGUIEnabled(); }
        }

        public void SetSelection(object selection) { Selection = selection; }
    }
}