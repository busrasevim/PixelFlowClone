namespace _Project.Scripts.Level.Signals
{
    public class OnLevelStartSignal
    {
        public readonly int StartingLevelIndex;
        public readonly Level CurrentLevel;

        public OnLevelStartSignal(int startingLevelIndex, Level currentLevel)
        {
            StartingLevelIndex = startingLevelIndex;
            CurrentLevel = currentLevel;
        }
    }
}