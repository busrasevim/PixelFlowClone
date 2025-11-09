using _Project.Scripts.Level.Signals;
using _Project.Scripts.State_Machine.States;

namespace _Project.Scripts.State_Machine.State_Machines
{
    public class MainStateMachine : StateManager<MainStateMachine.MainState>
    {
        public enum MainState
        {
            Start,
            Game,
            Finish,
        }

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
            var start = new StartGameState(MainState.Start, MainState.Game);
            var game = new PlayGameState(MainState.Game, MainState.Finish);
            var finish = new FinishGameState(MainState.Finish, MainState.Start);

            States.Add(MainState.Start, start);
            States.Add(MainState.Game, game);
            States.Add(MainState.Finish, finish);

            SetStateWithKey(MainState.Start);
        }

        private void OnLevelEnd(OnLevelEndSignal args)
        {
            if (GetCurrentState() == MainState.Finish) return;
            SetStateWithKey(MainState.Finish);
        }

        private void OnLevelStart(OnLevelStartSignal args)
        {
            if (GetCurrentState() == MainState.Game) return;
            SetStateWithKey(MainState.Game);
        }
    }
}