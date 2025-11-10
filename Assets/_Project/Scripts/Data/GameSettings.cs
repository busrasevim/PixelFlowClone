using System.Collections.Generic;
using _Project.Scripts.Level;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Scripts.Data
{
    [CreateAssetMenu(menuName = "Settings/Game Setting", fileName = "New Game Settings")]
    public class GameSettings : ScriptableObject
    {
        [TabGroup("General")] [MinValue(0), LabelText("Default Earn Money")]
        public int defaultLevelEarnMoneyCount = 30;

        [TabGroup("General")] [MinValue(0), LabelText("Random Start Index")]
        public int randomStartIndex = 3;

        [TabGroup("General")] public float levelCompletedUIDelay = 1f;
        [TabGroup("General")]public float levelFailedUIDelay = 2f;

        [TabGroup("Levels")] [ListDrawerSettings(Expanded = true, DraggableItems = false, ShowIndexLabels = true)]
        public LevelData[] levels;

        [ShowInInspector, ReadOnly, TabGroup("Debug")]
        [LabelText("Total Level Count")]
        public int TotalLevelCount => levels != null ? levels.Length : 0;

        private Dictionary<int, LevelData> _levelCache;


        [Button(ButtonSizes.Medium), GUIColor(0.4f, 0.8f, 1f)]
        [TabGroup("Debug")]
        private void RebuildCache() => BuildCache();

        public LevelData GetLevel(int index)
        {
            if (levels == null || levels.Length == 0)
            {
#if UNITY_EDITOR
                Debug.LogWarning("[GameSettings] Level listesi boş!");
#endif
                return null;
            }

            if (index >= 0 && index < levels.Length)
                return levels[index];

            if (randomStartIndex < 0 || randomStartIndex >= levels.Length)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"[GameSettings] RandomStartIndex ({randomStartIndex}) geçersiz, 0 olarak alındı.");
#endif
                randomStartIndex = 0;
            }

            int randomIndex = Random.Range(randomStartIndex, levels.Length);

#if UNITY_EDITOR
            Debug.Log($"[GameSettings] Level {index} -> random seçildi: {randomIndex}");
#endif

            return levels[randomIndex];
        }

        public int GetEarnMoneyForLevel(int index)
        {
            return defaultLevelEarnMoneyCount;
        }

        private void OnEnable() => BuildCache();

        private void BuildCache()
        {
            _levelCache = new Dictionary<int, LevelData>();

            if (levels == null) return;

            for (int i = 0; i < levels.Length; i++)
            {
                var lvl = levels[i];
                if (lvl == null)
                {
#if UNITY_EDITOR
                    Debug.LogWarning($"[GameSettings] levels[{i}] boş (null)!");
#endif
                    continue;
                }

                if (_levelCache.ContainsKey(i))
                {
#if UNITY_EDITOR
                    Debug.LogWarning($"[GameSettings] Duplicate level index {i}!");
#endif
                    continue;
                }

                _levelCache.Add(i, lvl);
            }
        }

#if UNITY_EDITOR
        [FoldoutGroup("Validation"), Button(ButtonSizes.Small), GUIColor(1f, 0.6f, 0.4f)]
        private void ValidateLevels()
        {
            if (levels == null || levels.Length == 0)
            {
                Debug.LogWarning("[GameSettings] Hiç level eklenmemiş.");
                return;
            }

            int nullCount = 0;
            foreach (var lvl in levels)
                if (lvl == null)
                    nullCount++;

            if (nullCount > 0)
                Debug.LogWarning($"[GameSettings] {nullCount} adet boş Level referansı bulundu.");
        }
#endif
    }
}