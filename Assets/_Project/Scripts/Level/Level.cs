using System;
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
        [SerializeField] public ColorCubeGridSystem colorCubeGridSystem;
        [SerializeField] private ReservedSlotGridSystem reservedSlotGridSystem;
        [SerializeField] private Conveyor conveyor;

        [SerializeField] private GameObject shooterPrefab;
        [SerializeField] private GameObject colorCubePrefab;

        public LevelData LevelData { get; private set; }
        public Conveyor Conveyor => conveyor;
        public ShooterGridSystem ShooterGridSystem => shooterGridSystem;
        public ReservedSlotGridSystem ReservedSlotGridSystem => reservedSlotGridSystem;

        private GameSettings _gameSettings;

        public void Init(LevelData data, GameSettings settings)
        {
            LevelData = data;
            _gameSettings = settings;

            shooterGridSystem.Init(data.shooterGridSize);

            var spaceX = colorCubeGridSystem.gridSpaceX *
                         (_gameSettings.defaultTextureWidth / data.colorCubeGridSize.x);
            var spaceZ = colorCubeGridSystem.gridSpaceZ *
                         (_gameSettings.defaultTextureHeight / data.colorCubeGridSize.y);

            spaceX = spaceZ = Mathf.Min(spaceX, spaceZ);

            colorCubeGridSystem.gridSpaceX = spaceX;
            colorCubeGridSystem.gridSpaceZ = spaceZ;

            var cubeScale = 1f * (data.colorCubeGridSize.x > data.colorCubeGridSize.y
                ? _gameSettings.defaultTextureWidth / data.colorCubeGridSize.x
                : _gameSettings.defaultTextureHeight / data.colorCubeGridSize.y);

            colorCubeGridSystem.Init(data.colorCubeGridSize);
            reservedSlotGridSystem.Init();

            CreateShooters(_gameSettings.shooterSpeed);
            CreateColorCubes(cubeScale);
            reservedSlotGridSystem.SetSlotValues(_gameSettings.reservedSlotWarningEffectDuration,
                _gameSettings.reservedSlotWarningEffectCount);

            conveyor.SetShooterLimit(settings.conveyorShooterLimit);
        }

        private void CreateShooters(float shooterFollowSpeed)
        {
            for (int i = 0; i < LevelData.CellsData.GetLength(0); i++)
            {
                for (int j = 0; j < LevelData.CellsData.GetLength(1); j++)
                {
                    var data = LevelData.CellsData[i, j];
                    var node = shooterGridSystem.GetNode(i, shooterGridSystem.gridHeight - j - 1);

                    var shooter = Instantiate(shooterPrefab, node.transform).GetComponent<Shooter>();
                    shooter.Init(data, shooterFollowSpeed);
                    shooter.Initialize(node);
                    node.AssignNodeObject(shooter);
                }
            }
        }

        private void CreateColorCubes(float scale)
        {
            Texture2D texture = LevelData.levelTexture;
            for (int i = 0; i < texture.width; i++)
            {
                for (int j = 0; j < texture.height; j++)
                {
                    if (texture.GetPixel(i, j).a <= 0.1f) continue;

                    var node = colorCubeGridSystem.GetNode(i, j);
                    var cube = Instantiate(colorCubePrefab, node.transform).GetComponent<ColorCube>();

                    cube.transform.localScale = new Vector3(scale, cube.transform.localScale.y, scale);
                    cube.Init(texture.GetPixel(i, j), LevelData.levelColors,
                        LevelData.colorThreshold);
                    cube.Initialize(node);
                    node.AssignNodeObject(cube);
                }
            }
        }

        public ReservedSlot GetAvailableReservedSlot()
        {
            var slot = reservedSlotGridSystem.GetAvailableSlot();
            return slot;
        }
    }
}