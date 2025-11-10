using _Project.Scripts.Data;
using UnityEngine;

namespace _Project.Scripts.Level
{
    public class LevelGenerator
    {
        private LevelManager _levelManager;
        private GameObject _levelPrefab;
        private GameSettings _gameSettings;

        public LevelGenerator(LevelManager levelManager, GameObject levelPrefab, GameSettings gameSettings)
        {
            _levelManager = levelManager;
            _levelPrefab = levelPrefab;
            _gameSettings = gameSettings;
        }

        public Level GenerateLevel(LevelData data)
        {
            ResetLevel();

            if (data == null)
            {
                Debug.LogError("[LevelGenerator] Level prefab null!");
                return null;
            }

            Level levelInstance = Object.Instantiate(_levelPrefab).GetComponent<Level>();

            levelInstance.Init(data, _gameSettings);
            return levelInstance;
        }

        private void ResetLevel()
        {
            if (_levelManager.CurrentLevel == null) return;

            Object.Destroy(_levelManager.CurrentLevel.gameObject);
        }
    }
}