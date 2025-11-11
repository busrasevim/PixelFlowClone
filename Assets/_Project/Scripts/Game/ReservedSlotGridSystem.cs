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

        public void SetWarningEffect()
        {
            for (int i = 0; i < _nodes.GetLength(0); i++)
            {
                for (int j = 0; j < _nodes.GetLength(1); j++)
                {
                    if (!_nodes[i, j].IsFull)
                    {
                        ActivateWarningEffect(false);
                        return;
                    }
                }
            }
            
            ActivateWarningEffect(true);
        }

        private void ActivateWarningEffect(bool isActive)
        {
            for (int i = 0; i < _nodes.GetLength(0); i++)
            {
                for (int j = 0; j < _nodes.GetLength(1); j++)
                {
                    var node = _nodes[i, j] as ReservedSlot;
                    node.ActivateWarningEffect(isActive);
                }
            }
        }

        public void SetSlotValues(float reservedSlotWarningEffectDuration, int reservedSlotWarningEffectCount)
        {
            for (int i = 0; i < _nodes.GetLength(0); i++)
            {
                for (int j = 0; j < _nodes.GetLength(1); j++)
                {
                    var slot = _nodes[i, j] as ReservedSlot;
                    slot.SetEffectValues(reservedSlotWarningEffectDuration, reservedSlotWarningEffectCount);
                }
            }
        }
    }
}
