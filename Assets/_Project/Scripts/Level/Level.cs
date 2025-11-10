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
        [SerializeField] private ReservedSlotGridSystem reservedSlotGridSystem;

        [SerializeField] private GameObject shooterPrefab;
        
        public LevelData LevelData { get; private set; }
        public void Init(LevelData data)
        {
            LevelData = data;
            
            shooterGridSystem.Init(data.shooterGridSize);
            colorCubeGridSystem.Init(data.colorCubeGridSize);
            reservedSlotGridSystem.Init();
            
            CreateShooters();
        }

        private void CreateShooters()
        {
            for (int i = 0; i < LevelData.CellsData.GetLength(0); i++)
            {
                for (int j = 0; j < LevelData.CellsData.GetLength(1); j++)
                {
                    var data = LevelData.CellsData[i, j];
                    var node = shooterGridSystem.GetNode(i, shooterGridSystem.gridHeight - j - 1);
                    
                    var shooter = Instantiate(shooterPrefab, node.transform).GetComponent<Shooter>();
                    shooter.Init(data);
                    shooter.Initialize(node);
                    node.AssignNodeObject(shooter);
                }
            }
        }
    }
}
