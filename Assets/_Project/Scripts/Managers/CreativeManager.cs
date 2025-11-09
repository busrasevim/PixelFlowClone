using _Project.Scripts.Audio;
using _Project.Scripts.Level;
using UnityEngine;
using Zenject;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
namespace _Project.Scripts.Managers
{
    public class CreativeManager : ITickable
    {
        //keycodes
        private KeyCode _restartKey = KeyCode.R;
        private KeyCode _winKey = KeyCode.W;
        private KeyCode _failKey = KeyCode.F;

        private GameManager _gameManager;
        private CreativeConfig _creativeConfig;

        [Inject]
        private void Init(GameManager gameManager, CreativeConfig creativeConfig)
        {
            _gameManager = gameManager;
            _creativeConfig = creativeConfig;
            
            SetKeys(creativeConfig.restartKey, creativeConfig.winKey, creativeConfig.failKey);
        }


        public void Tick()
        {
            if (Input.GetKeyDown(_restartKey))
            {
                _gameManager.RestartLevel();
            }
            else if (Input.GetKeyDown(_winKey))
            {
                _gameManager.EndLevel(true);
            }
            else if (Input.GetKeyDown(_failKey))
            {
                _gameManager.EndLevel(false);
            }
        }

        public void SetKeys(KeyCode restart, KeyCode win, KeyCode fail)
        {
            _restartKey = restart;
            _winKey = win;
            _failKey = fail;
        }
    }
}
#else
// Build'de tamamen etkisiz
namespace _Project.Scripts.Managers
{
    public class CreativeManager : ITickable
    {
        public void Tick() { }
    }
}
#endif