using UnityEngine;

namespace _Project.Scripts.Game
{
    public class ColorCubeGridSystem : GridSystem
    {
        public override void Init(Vector2Int size)
        {
            gridWidth  = size.x;
            gridHeight = size.y;
            
            SpawnNodes(transform);
        }
    }
}
