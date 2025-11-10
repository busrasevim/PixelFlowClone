using _Project.Scripts.Data;
using _Project.Scripts.Game;
using _Project.Scripts.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Scripts.Level
{
    public class Level : MonoBehaviour
    {
        [SerializeField] private ShooterGridSystem shooterGridSystem;
        [SerializeField] private ColorCubeGridSystem colorCubeGridSystem;
        
        public Vector2Int ShooterGridSystemSize = new Vector2Int(3, 5);

        public void Init()
        {
            shooterGridSystem.Init(ShooterGridSystemSize);
            colorCubeGridSystem.Init(new Vector2Int(10,10));
        }
    }
}
