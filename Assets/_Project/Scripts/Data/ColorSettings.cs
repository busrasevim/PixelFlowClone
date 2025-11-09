using System;
using System.Collections.Generic;
using _Project.Scripts.Utils;
using UnityEngine;

namespace _Project.Scripts.Data
{
    [CreateAssetMenu(menuName = "Settings/Color Setting", fileName = "New Color Settings")]
    public class ColorSettings : ScriptableObject
    {
        [Header("PALETTE")] public ColorData[] colorsData;

        [Header("DEFAULTS"), Space(8)] [Tooltip("Aranan ColorType bulunamazsa buraya düşülür.")]
        public ColorType defaultColorType = ColorType.Blue;

        private Dictionary<ColorType, ColorData> _map;

        public ColorData GetColorDataWithType(ColorType type)
        {
            EnsureMap();

            if (_map.TryGetValue(type, out var data)) return data;

            if (_map.TryGetValue(defaultColorType, out var fallback))
                return fallback;

            if (colorsData != null && colorsData.Length > 0)
                return colorsData[0];

            return default; //bunu sorcaz
        }

        private void OnEnable()
        {
            RebuildMap();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            RebuildMap();
        }
#endif

        private void EnsureMap()
        {
            if (_map == null || _map.Count == 0) RebuildMap();
        }

        private void RebuildMap()
        {
            _map = new Dictionary<ColorType, ColorData>(colorsData != null ? colorsData.Length : 0);

            if (colorsData == null) return;

            for (int i = 0; i < colorsData.Length; i++)
            {
                var cd = colorsData[i];

                _map.Add(cd.colorType, cd);
            }

            if (!_map.ContainsKey(defaultColorType))
            {
                foreach (var kv in _map)
                {
                    defaultColorType = kv.Key;
                    break;
                }
            }
        }
    }
}

[System.Serializable]
public struct ColorData
{
    public ColorType colorType;
    public Material material;
}

public enum ColorType
{
    None,
    Red,
    Blue,
    Yellow,
    Green,
    Pink,
    Purple,
    Orange,
    Cyan,
}