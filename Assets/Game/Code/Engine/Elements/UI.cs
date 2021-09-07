using System;
using System.Globalization;
using UnityEngine;
using Jape;
using TMPro;

namespace Game
{
    public class UI : Element
    {
        [SerializeField]
        private Canvas canvas;

        public GameObject IconSettings => transform.Find("SettingsIcon").gameObject;
        public GameObject MenuSettings => transform.Find("SettingsMenu").gameObject;

        public TMP_Text Timer => transform.Find("Timer").gameObject.GetComponent<TMP_Text>();
        public TMP_Text Record => transform.Find("Record").gameObject.GetComponent<TMP_Text>();

        public void ShowSettings()
        { 
            IconSettings.SetActive(false);
            MenuSettings.SetActive(true);
            Game.Pause();
        }

        public void HideSettings()
        {
            Game.Resume();
            IconSettings.SetActive(true);
            MenuSettings.SetActive(false);
        }

        public void ToggleTimer()
        { 
            Timer.gameObject.SetActive(!Timer.gameObject.activeSelf);
        }

        public void RestartGame()
        {
            HideSettings();
            GameManager.Restart();
        }

        public void SetTime(float time)
        {
            Timer.text = TimeSpan.FromSeconds(time).ToString("mm\\:ss\\:fff");
        }

        public void SetRecord(float time)
        {
            Record.text = TimeSpan.FromSeconds(time).ToString("mm\\:ss\\:fff");
        }

        public static UI Create(Jape.Camera camera)
        {
            UI ui = Jape.Game.CloneGameObject(Database.LoadPrefab("UI")).GetComponent<UI>();
            ui.canvas.worldCamera = camera.UnityCamera;
            DontDestroyOnLoad(ui.gameObject);
            return ui;
        }
    }
}