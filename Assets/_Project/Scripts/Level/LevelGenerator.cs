using _Project.Scripts.Data;
using _Project.Scripts.Pools;
using UnityEngine;

namespace _Project.Scripts.Level
{
    public class LevelGenerator
    {
        private LevelManager _levelManager;
        private GameObject _levelPrefab;
        private GameSettings _gameSettings;
        private ObjectPool  _objectPool;

        public LevelGenerator(LevelManager levelManager, GameObject levelPrefab, GameSettings gameSettings,
            ObjectPool objectPool)
        {
            _levelManager = levelManager;
            _levelPrefab = levelPrefab;
            _gameSettings = gameSettings;
            _objectPool = objectPool;
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

            levelInstance.Init(data, _gameSettings, _objectPool);
            return levelInstance;
        }

        private void ResetLevel()
        {
            Time.timeScale = 1;
            if (_levelManager.CurrentLevel == null) return;

            Object.Destroy(_levelManager.CurrentLevel.gameObject);
        }
    }
}