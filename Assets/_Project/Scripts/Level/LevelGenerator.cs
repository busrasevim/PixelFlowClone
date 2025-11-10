using UnityEngine;

namespace _Project.Scripts.Level
{
    public class LevelGenerator
    {
        private LevelManager _levelManager;
        private GameObject _levelPrefab;
        public LevelGenerator(LevelManager levelManager, GameObject levelPrefab)
        {
            _levelManager = levelManager;
            _levelPrefab = levelPrefab;
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
            
            levelInstance.Init(data);
            return levelInstance;
        }

        private void ResetLevel()
        {
            if(_levelManager.CurrentLevel == null) return;
            
            Object.Destroy(_levelManager.CurrentLevel.gameObject);
        }
    }
}