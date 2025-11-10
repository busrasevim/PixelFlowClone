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
        [SerializeField] private GameObject colorCubePrefab;
        
        public LevelData LevelData { get; private set; }
        public void Init(LevelData data)
        {
            LevelData = data;
            
            shooterGridSystem.Init(data.shooterGridSize);
            colorCubeGridSystem.Init(data.colorCubeGridSize);
            reservedSlotGridSystem.Init();
            
            CreateShooters();
            CreateColorCubes();
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

        private void CreateColorCubes()
        {
            Texture2D texture = LevelData.levelTexture;
            for (int i = 0; i < texture.width; i++)
            {
                for (int j = 0; j < texture.height; j++)
                {
                    var node = colorCubeGridSystem.GetNode(i, j);
                    var cube = Instantiate(colorCubePrefab, node.transform).GetComponent<ColorCube>();
                    
                    cube.Init(texture.GetPixel(i, j));
                    cube.Initialize(node);
                    node.AssignNodeObject(cube);
                }
            }
        }
    }
}
