using UnityEngine;

namespace _Project.Scripts.Game
{
    public class ShooterGridSystem : GridSystem
    {
        public override void Init(Vector2Int size)
        {
            gridWidth = size.x;
            gridHeight = size.y;
            
            SpawnNodes(transform);
        }
    }
}
