using System;
using UnityEngine;

namespace _Project.Scripts.Level
{
    [Serializable]
    public class CellData
    {
        public Vector2Int coordinates;
        public Color cellColor;
        public int shootCount;

        public CellData(Vector2Int coordinates)
        {
            this.coordinates = coordinates;
            shootCount = 0;
        }
    }
}
