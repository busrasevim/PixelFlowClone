namespace _Project.Scripts.Level.Signals
{
    public struct OnLevelRestartSignal
    {
        public readonly int RestartingLevelIndex;

        public OnLevelRestartSignal(int restartingLevelIndex)
        {
            RestartingLevelIndex = restartingLevelIndex;
        }
    }
}