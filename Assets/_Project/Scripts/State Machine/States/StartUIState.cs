using _Project.Scripts.State_Machine.State_Machines;
using UIManager = _Project.Scripts.UI.UIManager;

namespace _Project.Scripts.State_Machine.States
{
    public class StartUIState : BaseState<UIStateMachine.UIState>
    {
        private UIManager _uiManager;

        public StartUIState(UIStateMachine.UIState key, UIStateMachine.UIState nextStateKey, UIManager uiManager) : base(key)
        {
            NextStateKey = nextStateKey;
            _uiManager = uiManager;
        }

        public override void OnEnter()
        {
            _uiManager.ShowHome();
        }

        public override void OnExit()
        {
            
        }
    }
}