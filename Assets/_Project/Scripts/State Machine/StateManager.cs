using System;
using System.Collections.Generic;
using _Project.Scripts.State_Machine.Interfaces;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.State_Machine
{
    public abstract class StateManager<EState> : IInitializable, System.IDisposable, ITickable, IFixedTickable where EState : Enum
    {
        protected Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();

        private BaseState<EState> CurrentState;

        [Inject] protected SignalBus _signalBus;

        public void Initialize()
        {
            Init();
            SetStates();
        }

        public void Tick()
        {
            if(CurrentState is ITickableState tickableState)
                tickableState.OnUpdate();
        }
        
        public void FixedTick()
        {
            if(CurrentState is IFixedTickableState tickableState)
                tickableState.OnFixedUpdate();
        }
        
        protected virtual void TransitionToState(EState stateKey)
        {
            if (CurrentState != null && CurrentState == States[stateKey])
                return;

            CurrentState?.OnExit();
            CurrentState = States[stateKey];
            CurrentState.OnEnter();
        }

        protected abstract void SetStates();
        protected abstract void Init();

        public void SetStateWithKey(EState stateKey)
        {
            TransitionToState(stateKey);
        }

        public EState GetCurrentState()
        {
            return CurrentState.StateKey;
        }

        public virtual void Dispose()
        {
        }
    }
}