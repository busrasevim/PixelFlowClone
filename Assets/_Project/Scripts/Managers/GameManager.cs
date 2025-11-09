using System;
using System.Threading;
using _Project.Scripts.Level;
using _Project.Scripts.Level.Signals;
using _Project.Scripts.State_Machine.State_Machines;
using _Project.Scripts.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using UIManager = _Project.Scripts.UI.UIManager;

namespace _Project.Scripts.Managers
{
    public class GameManager : IInitializable, System.IDisposable
    {
        private MainStateMachine _mainStateMachine;
        private UIStateMachine _uIStateMachine;
        private LevelManager _levelManager;
        private UIManager _uiManager;
        private SignalBus _signalBus;
        private FXManager _fxManager;

        private CancellationTokenSource _restartCts;

        [Inject]
        public GameManager(MainStateMachine mainStateMachine, UIStateMachine uiStateMachine, LevelManager levelManager,
            FXManager fxManager, SignalBus signalBus, UIManager uiManager)
        {
            _mainStateMachine = mainStateMachine;
            _uIStateMachine = uiStateMachine;
            _levelManager = levelManager;
            _fxManager = fxManager;
            _signalBus = signalBus;
            _uiManager = uiManager;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<OnPlaySignal>(OnPlay);
            _signalBus.Subscribe<OnRestartSignal>(OnRestart);
            _signalBus.Subscribe<OnLevelSetupRequestedSignal>(HandleSetupRequest);

            _uiManager.ShowHome();
            SetUpLevel();
        }

        private void SetUpLevel(bool fromRestart = false)
        {
            _mainStateMachine.SetStateWithKey(MainStateMachine.MainState.Start);
            _uIStateMachine.SetStateWithKey(UIStateMachine.UIState.Start);

            _levelManager.SetUpLevel();

            if (fromRestart)
            {
                _uIStateMachine.SetStateWithKey(UIStateMachine.UIState.InGame);
                _mainStateMachine.SetStateWithKey(MainStateMachine.MainState.Game);
                StartLevel();
            }
        }

        private void OnPlay()
        {
            StartLevel();
        }

        private void OnRestart(OnRestartSignal arg)
        {
            RestartLevel(arg.Hard);
        }
        private async void RestartLevelAfter(float delay)
        {
            _restartCts?.Cancel();
            _restartCts = new CancellationTokenSource();
            
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: _restartCts.Token);
            RestartLevel();
        }

        public void RestartLevel(bool hard = false)
        {
            _signalBus.TryFire(new OnLevelRestartSignal(_levelManager.CurrentLevelNo));
            SetUpLevel(hard);
        }

        public void Dispose()
        {
            _signalBus.TryUnsubscribe<OnPlaySignal>(OnPlay);
            _signalBus.TryUnsubscribe<OnRestartSignal>(OnRestart);
            _signalBus.TryUnsubscribe<OnLevelSetupRequestedSignal>(HandleSetupRequest);
        }

        private void HandleSetupRequest()
        {
            RestartLevel();
        }
        
        #region Game State Events

        public void StartLevel()
        {
            _signalBus.TryFire(new OnLevelStartSignal(_levelManager.CurrentLevelNo, _levelManager.CurrentLevel));
        }

        public void EndLevel(bool isWin)
        {
            if (_mainStateMachine.GetCurrentState() == MainStateMachine.MainState.Finish) return;

            if (isWin)
                LevelCompleted();
            else
                LevelFailed();

            _signalBus.TryFire(new OnLevelEndSignal(isWin, _levelManager.CurrentLevelNo));
        }

        private void LevelCompleted()
        {
            _levelManager.NextLevel();
            Debug.Log("Level completed.");
        }

        private void LevelFailed()
        {
            Debug.Log("Level failed.");
        }

        #endregion
    }
}