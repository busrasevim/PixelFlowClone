using _Project.Scripts.Data;
using _Project.Scripts.Game;
using _Project.Scripts.Level;
using _Project.Scripts.Level.Signals;
using _Project.Scripts.SaveSystem;
using _Project.Scripts.State_Machine.States;
using _Project.Scripts.UI;
using Zenject;

namespace _Project.Scripts.State_Machine.State_Machines
{
    public class UIStateMachine : StateManager<UIStateMachine.UIState>
    {
        public enum UIState
        {
            Start,
            InGame,
            LevelEnd,
        }

        [Inject] private UIManager _uiManager;
        [Inject] private GameSettings _settings;

        protected override void Init()
        {
            _signalBus.Subscribe<OnLevelEndSignal>(OnLevelEnd);
            _signalBus.Subscribe<OnLevelStartSignal>(OnLevelStart);
        }

        public override void Dispose()
        {
            _signalBus.Unsubscribe<OnLevelEndSignal>(OnLevelEnd);
            _signalBus.Unsubscribe<OnLevelStartSignal>(OnLevelStart);
        }

        protected override void SetStates()
        {
            var startUI = new StartUIState(UIState.Start, UIState.InGame, _uiManager);
            var inGameUI = new InGameUIState(UIState.InGame, UIState.LevelEnd, _uiManager);
            var endUI = new LevelEndUIState(UIState.LevelEnd, UIState.Start, _settings, _uiManager);

            States.Add(UIState.Start, startUI);
            States.Add(UIState.InGame, inGameUI);
            States.Add(UIState.LevelEnd, endUI);

            SetStateWithKey(UIState.Start);
        }

        private void OnLevelEnd(OnLevelEndSignal args)
        {
            if (GetCurrentState() == UIState.LevelEnd) return;

            (States[UIState.LevelEnd] as LevelEndUIState)?.SetWin(args.IsWin);
            SetStateWithKey(UIState.LevelEnd);
        }

        private void OnLevelStart(OnLevelStartSignal args)
        {
            if (GetCurrentState() == UIState.InGame) return;
            SetStateWithKey(UIState.InGame);
        }
    }
}