using _Project.Scripts.Data;
using _Project.Scripts.Level.Signals;
using _Project.Scripts.SaveSystem;
using Zenject;

namespace _Project.Scripts.Level
{
    public class LevelManager : IInitializable
    {
        public int CurrentLevelNo { get; private set; }
        
        private LevelGenerator _generator;
        private DataManager _dataManager;
        private SignalBus _signalBus;
        private GameSettings _gameSettings;
        
        public Level CurrentLevel { get; private set; }
        
        public void Initialize()
        {
            //level number value, generating etc
            SetInitialLevel();
        }

        [Inject]
        private void SpecialInit(DataManager dataManager, 
            GameSettings settings, SignalBus signal)
        {
            _dataManager = dataManager;
            _signalBus = signal;
            _gameSettings = settings;
            
            _generator = new LevelGenerator(this);
        }
        
        public void SetUpLevel()
        {
            var levelPrefab = _gameSettings.GetLevel(CurrentLevelNo);
            CurrentLevel = _generator.GenerateLevel(levelPrefab);
        }

        public void NextLevel()
        {
            OnLevelCompleted(CurrentLevelNo);
            CurrentLevelNo++;
        }
        
        private void SetInitialLevel()
        {
            CurrentLevelNo = _dataManager.GameData.currentLevelNumber;
        }
        
        private void OnLevelCompleted(int completedLevelIndex)
        {
            _signalBus.TryFire(new OnLevelCompletedSignal(completedLevelIndex, _gameSettings.defaultLevelEarnMoneyCount));
        }
    }
}


[System.Serializable]
public class LevelNumberData
{
    public int[] levelNumbers;
}