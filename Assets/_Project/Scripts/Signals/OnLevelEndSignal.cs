namespace _Project.Scripts.Level.Signals
{
    public struct OnLevelEndSignal
    {
        public readonly bool IsWin;
        public readonly int LevelIndex;

        public OnLevelEndSignal(bool isWin, int levelIndex)
        {
            IsWin = isWin;
            LevelIndex = levelIndex;
        }
    }
}