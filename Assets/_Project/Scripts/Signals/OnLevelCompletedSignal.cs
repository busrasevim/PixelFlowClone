namespace _Project.Scripts.Level.Signals
{
    public struct OnLevelCompletedSignal
    {
        public readonly int LevelIndex;
        public readonly int DefaultLevelEarnMoney;

        public OnLevelCompletedSignal(int levelIndex, int defaultLevelEarnMoney)
        {
            LevelIndex = levelIndex;
            DefaultLevelEarnMoney = defaultLevelEarnMoney;
        }
    }
}