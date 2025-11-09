using UnityEngine;

namespace _Project.Scripts.Game
{
    public class Node : MonoBehaviour
    {
        public bool IsFull { get; protected set; }
        public INodeObject NodeObject { get; private set; }
        protected GridSystem _gridSystem;
        
        public Vector2Int GridPosition { get; private set; }
        

        public void Initialize(GridSystem system)
        {
            _gridSystem = system;
        }

        public void SetCoordination(int x, int y)
        {
            GridPosition = new Vector2Int(x, y);
        }

        public virtual void AsiignNodeObject(INodeObject nodeObj)
        {
            NodeObject = nodeObj;
            IsFull = true;
        }
        
        public virtual void SetEmpty(INodeObject nodeObj)
        {
            IsFull = false;
            NodeObject = null;
        }

        public virtual void Reset()
        {
            SetEmpty(null);
        }
    }
}
