using System;
using Sirenix.OdinInspector;

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