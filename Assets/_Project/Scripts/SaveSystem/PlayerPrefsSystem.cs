using UnityEngine;

namespace _Project.Scripts.SaveSystem
{
    public class PlayerPrefsSystem : ISaveSystem
    {
        public bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public bool Save<T>(string key, T data)
        {
            switch (data)
            {
                case int valueI:
                    PlayerPrefs.SetInt(key, valueI);
                    return true;
                case string valueS:
                    PlayerPrefs.SetString(key, valueS);
                    return true;
                case float valueF:
                    PlayerPrefs.SetFloat(key, valueF);
                    return true;
                default:
                    return false;
            }
        }

        public bool TryGet<T>(string key, out T data)
        {
            if (typeof(T) == typeof(int))
            {
                var value = PlayerPrefs.GetInt(key);
                data = (T)(object)value;
                return true;
            }

            if (typeof(T) == typeof(string))
            {
                var value = PlayerPrefs.GetString(key);
                data = (T)(object)value;
                return true;
            }

            if (typeof(T) == typeof(float))
            {
                var value = PlayerPrefs.GetFloat(key);
                data = (T)(object)value;
                return true;
            }

            data = default(T);
            return false;
        }
    }
}
