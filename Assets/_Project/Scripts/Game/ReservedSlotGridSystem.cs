namespace _Project.Scripts.Game
{
    public class ReservedSlotGridSystem : GridSystem
    {
        public ReservedSlot GetAvailableSlot()
        {
            for (int i = 0; i < _nodes.GetLength(0); i++)
            {
                for (int j = 0; j < _nodes.GetLength(1); j++)
                {
                    if(_nodes[i, j].IsFull) continue;
                    
                    return _nodes[i, j] as ReservedSlot;
                }
            }

            return null;
        }

        public int GetCurrentShooterCount()
        {
            var count = 0;
            for (int i = 0; i < _nodes.GetLength(0); i++)
            {
                for (int j = 0; j < _nodes.GetLength(1); j++)
                {
                    if(_nodes[i,j].IsFull)
                        count++;
                }
            }
            
            return count;
        }
    }
}
