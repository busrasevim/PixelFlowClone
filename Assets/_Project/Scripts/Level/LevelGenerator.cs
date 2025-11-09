using UnityEngine;

namespace _Project.Scripts.Level
{
    public class LevelGenerator
    {
        private LevelManager _levelManager;
        public LevelGenerator(LevelManager levelManager)
        {
            _levelManager = levelManager;
        }

        public Level GenerateLevel(Level level)
        {
            ResetLevel();

            if (level == null)
            {
                Debug.LogError("[LevelGenerator] Level prefab null!");
                return null;
            }

            Level levelInstance = Object.Instantiate(level);
            
            levelInstance.Init();
            return levelInstance;
        }

        private void ResetLevel()
        {
            if(_levelManager.CurrentLevel == null) return;
            
            Object.Destroy(_levelManager.CurrentLevel.gameObject);
        }
    }
}