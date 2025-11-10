using UnityEngine;

namespace _Project.Scripts.Game
{
    public class ColorCubeGridSystem : GridSystem
    {
        public override void Init(Vector2Int size = default)
        {
            gridWidth  = size.x;
            gridHeight = size.y;
            
            base.Init(size);
        }
    }
}
