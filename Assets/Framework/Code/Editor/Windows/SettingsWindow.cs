using System.Linq;
using UnityEditor;
using UnityEngine;
using Jape;

namespace JapeEditor
{
    public class SettingsWindow : Window
    {
        protected override string Title => "Settings";
        protected override Display DisplayMode => Display.Popup;
        protected override bool AutoHeight => true;
        protected override float Width => 512;

        private int index = -1;

        protected override void Draw()
        {
            SettingsData[] settings = Database.GetAssets<SettingsData>().Select(s => s.Load<SettingsData>()).ToArray();

            for (int i = 0; i < settings.Length; i++)
            {
                bool active = i == index;
                active = GUILayout.Toggle(active, settings[i].name, GUI.skin.button, GUILayout.Height(32));
                if (active) { index = i; }
            }

            GUILayout.Space(8);

            if (index >= 0) { ProjectWindowUtil.ShowCreatedAsset(settings[index]); }
        }
    }
}