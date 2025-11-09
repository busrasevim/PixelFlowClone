using System.Collections.Generic;
using _Project.Scripts.Level.Signals;
using _Project.Scripts.SaveSystem;
using Zenject;

namespace _Project.Scripts.Managers
{
    public class EconomyManager: IInitializable, System.IDisposable
    {
        private readonly HashSet<IMoneyEconomyListener> _listeners = new();
        
        private DataManager _dataManager;
        private SignalBus _signalBus;

        private int _moneyCount;
        public int MoneyCount => _moneyCount;

        [Inject]
        public EconomyManager(DataManager dataManager, SignalBus signalBus)
        {
            _dataManager = dataManager;
            _signalBus = signalBus;
        }
        
        public void Initialize()
        {
            _moneyCount = _dataManager.GameData.currentMoney;
            NotifyMoneyListeners();
        
            _signalBus.Subscribe<OnLevelCompletedSignal>(OnLevelCompleted);
        }

        public void Dispose()
        {
            _signalBus.TryUnsubscribe<OnLevelCompletedSignal>(OnLevelCompleted);
        }

        private void ModifyMoney(int delta)
        {
            _moneyCount += delta;

            if (_moneyCount < 0) _moneyCount = 0;
            
            UpdateData();
            NotifyMoneyListeners();
        }
        
        public void EarnMoney(int earnedAmount)
        {
            ModifyMoney(earnedAmount);
        }

        public void SpendMoney(int spentAmount)
        {
            ModifyMoney(-spentAmount);
        }

        public bool CanAfford(int cost)
        {
            return cost <= _moneyCount;
        }

        public void AddMoneyListener(IMoneyEconomyListener listener)
        {
            if (_listeners.Add(listener))
                listener.MoneyCountUpdated(_moneyCount);
        }

        public void RemoveMoneyListener(IMoneyEconomyListener listener)
        {
            _listeners.Remove(listener);
        }

        private void NotifyMoneyListeners()
        {
            foreach (var listener in _listeners)
            {
                listener.MoneyCountUpdated(_moneyCount);
            }
        }

        private void UpdateData()
        {
            _dataManager.UpdateMoneyCount(_moneyCount);
        }

        private void OnLevelCompleted(OnLevelCompletedSignal args)
        {
            EarnMoney(args.DefaultLevelEarnMoney);
        }
    }

    public interface IMoneyEconomyListener
    {
        void MoneyCountUpdated(int moneyCount);
    }
}