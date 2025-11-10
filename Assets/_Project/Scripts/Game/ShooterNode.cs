using UnityEngine;

namespace _Project.Scripts.Game
{
    public class ShooterNode : Node
    {
        public bool IsFrontNode()
        {
            return GridPosition.y == _gridSystem.gridHeight - 1;
        }
    }
}