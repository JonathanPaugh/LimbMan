using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jape
{
	public partial class Properties
    {
        [Serializable]
        internal class SavedElement
        {
            [HorizontalGroup("ElementSave")]

            [HidePicker]
            [ReadOnly]
            public Element element;

            [HorizontalGroup("ElementSave", Width = 18)]

            [HideLabel]
            public bool save;
        }
    }
}