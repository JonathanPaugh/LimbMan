using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    [AddComponentMenu("")]
    public class EntWorldTransition : WorldEntity
    {
        protected override Texture2D Icon => GetIcon("IconTransition");

        [Space(16)]

        [SerializeField]
        [ValueDropdown(nameof(LoadingScreenDropdown), AppendNextDrawer = true, DisableGUIInAppendedDrawer = true)]
        private Map loadingScreen;

        [Space(16)]

        [SerializeField] 
        private Map map = null;

        [Space(16)]

        [SerializeField] 
        [ValueDropdown(nameof(GetTargets))]
        private string target = null;

        private static GameObject activator;
        private static string activatorTarget;

        private IList<ValueDropdownItem<string>> GetTargets()
        {
            return Ids.GetAll().Where(i => i.Map.ScenePath == map.ScenePath).
                                Select(i => new ValueDropdownItem<string>(i.Name, i.Value)).
                                Prepend(new ValueDropdownItem<string>("None", string.Empty)).
                                ToArray();
        }

        protected override void Default() { loadingScreen = Framework.Settings<SceneSettings>().defaultLoadingScreen; }

        protected override void TouchAction(GameObject gameObject)
        {
            SetActivator(gameObject, target);
            EngineManager.Instance.OnSceneSetup += Move;
            Map.Change(map, loadingScreen);
        }

        private static void SetActivator(GameObject gameObject, string target)
        {
            activator = gameObject;
            activatorTarget = target;
        }

        private static void ResetActivator()
        {
            activator = null;
            activatorTarget = null;
        }

        private static void Move(Map map)
        {
            EngineManager.Instance.OnSceneSetup -= Move;

            GameObject gameObject = activator;
            string target = activatorTarget;

            ResetActivator();

            if (gameObject == null || string.IsNullOrEmpty(target)) { return; }
            
            if (!Ids.Has(target)) { return; }

            gameObject.transform.position = Ids.Get(target).Position;
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            const float MaxDistance = 100;

            if (UnityEngine.Camera.current == null) { return; }

            float distance = Vector3.Distance(UnityEngine.Camera.current.transform.position, gameObject.transform.position);

            if (distance > MaxDistance) { return; }

            GUIStyle style = new GUIStyle(GUI.skin.label).FontSize(8);
            GUIContent labelContent = new($" {map.GetSceneNameEditor()} - {GetTargets().FirstOrDefault(t => t.Value == target).Text}");
            Vector2 size = style.CalcSize(labelContent);
            GUIContent iconContent = string.IsNullOrEmpty(target) ? new GUIContent(Database.GetAsset<Texture>("IconUnlink").Load<Texture>()) : new GUIContent(Database.GetAsset<Texture>("IconLink").Load<Texture>());

            float xOffset = -(size.x / 2);
            float yOffset = Math.Rescale(distance / MaxDistance, 0, 1, -25, -10);

            if (map.IsSet())
            {
                UnityEditor.Handles.Label(transform.position, iconContent, new GUIStyle().Offset(new Vector2(-4, yOffset - 20)).Padding(new RectOffset(-24, -24, -24, -24)));
                UnityEditor.Handles.Label(transform.position, labelContent, style.Offset(new Vector2(xOffset, yOffset)));
            }
        }
        #endif

        private static IList<ValueDropdownItem<Map>> LoadingScreenDropdown() { return Framework.Settings<SceneSettings>().LoadingScreenDropdownEditor(); }
    }
}