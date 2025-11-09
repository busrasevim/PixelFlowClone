using System;
using _Project.Scripts.Game.Constants;
using _Project.Scripts.Level.Signals;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.SaveSystem
{
    public class DataManager : IInitializable, IDisposable
    {
        private GameData _gameData;
        public GameData GameData => _gameData;
        
        private ISaveSystem _saveSystem;
        private SignalBus _signalBus;
    
        public DataManager(ISaveSystem saveSystem, SignalBus signalBus)
        {
            _saveSystem = saveSystem;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            LoadData();
            _signalBus.Subscribe<OnLevelCompletedSignal>(OnLevelCompleted);
        }

        public void Dispose()
        {
            SaveData();
            _signalBus.TryUnsubscribe<OnLevelCompletedSignal>(OnLevelCompleted);
        }

        private void SaveData()
        {
            if (_saveSystem.Save(Constants.GameDataKey, _gameData))
            {
                Debug.Log("Data is saved successfully.");
            }
            else
            {
                Debug.LogWarning("Data cannot be saved.");
            }
        }

        private void LoadData()
        {
            if (!_saveSystem.HasKey(Constants.GameDataKey))
            {
                _gameData = new GameData();
                SaveData();
            }
            else if (_saveSystem.TryGet(Constants.GameDataKey, out _gameData))
            {
                Debug.Log("Data is loaded successfully.");
            }
            else
            {
                Debug.LogError("Data cannot be loaded.");
                Application.Quit();
            }
        }
    
        public void UpdateMoneyCount(int currentMoneyCount)
        {
            _gameData.currentMoney = currentMoneyCount;
            SaveData();
        }
        
        private void OnLevelCompleted(OnLevelCompletedSignal args)
        {
            _gameData.currentLevelNumber = args.LevelIndex+1;
            SaveData();
        }

        public void SetHaptic(bool isOn)
        {
            Taptic.tapticOn = isOn;
            _gameData.isHapticOn = isOn;
            SaveData();
        }

        public void SetSound(bool isOn)
        {
            _gameData.isSoundOn = isOn;
            SaveData();
        }
    }
}