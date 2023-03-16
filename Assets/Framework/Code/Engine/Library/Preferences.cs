using UnityEngine;

namespace Jape
{
	public static class Preferences 
	{
        public static bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public static void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

		public static void GetFloat(string key)
        {
			PlayerPrefs.GetFloat(key);
        }

        public static void GetInt(string key)
        {
            PlayerPrefs.GetInt(key);
        }

        public static void GetString(string key)
        {
            PlayerPrefs.GetString(key);
        }

        public static void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        public static void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public static void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        public static void Save()
        {
            PlayerPrefs.Save();
        }
	}
}