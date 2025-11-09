using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Game
{
    public abstract class GridSystem : MonoBehaviour
    {
        public int gridWidth;
        public int gridHeight;

        public bool coordinateXY;
        public bool coordinateXZ;
        public bool coordinateYZ;

        public float centerX;
        public float centerY;
        public float centerZ;

        public float gridSpaceX;
        public float gridSpaceY;
        public float gridSpaceZ;

        protected Node[,] _nodes;

        public GameObject nodePrefab;

        [Header("Z Position Control")] public bool useCustomZStart; // Özel başlangıç Z değeri
        public float customZStart;

        public bool useCustomZEnd; // Özel bitiş Z değeri
        public float customZEnd;

        public virtual void Init(Vector2Int size)
        {
            
        }
        
        protected virtual void SpawnNodes(Transform parent = null, bool editorSpawn = false)
        {
            // Initialize the grid array
            _nodes = new Node[gridWidth, gridHeight];

            // Calculate half sizes for centering the grid
            float halfRowSize = (gridWidth - 1) * (1 + gridSpaceX) / 2f;
            float halfColumnSizeY = (gridHeight - 1) * (1 + gridSpaceY) / 2f;
            float halfColumnSizeZ = (gridHeight - 1) * (1 + gridSpaceZ) / 2f;

            // Loop through each grid position
            for (int i = 0; i < gridWidth; i++)
            {
                for (int j = 0; j < gridHeight; j++)
                {
                    var position = GetNodePosition(i, j);
                    Node node;

#if UNITY_EDITOR
                    // If we are spawning in the editor using PrefabUtility
                    if (editorSpawn)
                    {
                        var obj = (GameObject)PrefabUtility.InstantiatePrefab(nodePrefab, parent);

                        obj.transform.position = position;
                        node = obj.GetComponent<Node>();
                    }
                    else
#endif
                    {
                        // If we are spawning in runtime
                        node = Instantiate(nodePrefab, position,
                            Quaternion.identity, parent).GetComponent<Node>();
                    }

                    // Set grid coordinates and initialize
                    node.SetCoordination(i, j);
                    node.Initialize(this);

                    // Store the grid object in the array
                    _nodes[i, j] = node;
                }
            }
        }

        public virtual void ResetSystem()
        {
            for (int i = 0; i < _nodes.GetLength(0); i++)
            {
                for (int j = 0; j < _nodes.GetLength(1); j++)
                {
                    _nodes[i, j].Reset();
                }
            }
        }

        public Node GetNode(int x, int y)
        {
            if (x < 0 || x >= _nodes.GetLength(0) || y < 0 || y >= _nodes.GetLength(1))
            {
                // Debug.LogWarning("Grid coordinates out of range!");
                return null;
            }

            return _nodes[x, y];
        }

        public virtual Vector3 GetNodePosition(int row, int column)
        {
            float halfRowSize = (gridWidth - 1) * (1 + gridSpaceX) / 2f;
            float halfColumnSizeY = (gridHeight - 1) * (1 + gridSpaceY) / 2f;
            float halfColumnSizeZ = (gridHeight - 1) * (1 + gridSpaceZ) / 2f;

            // Calculate positions based on the selected coordinate planes
            float xPosition = coordinateYZ ? centerX : row * (1 + gridSpaceX) - halfRowSize + centerX;
            float yPosition = coordinateXZ ? centerY : column * (1 + gridSpaceY) - halfColumnSizeY + centerY;
            float zPosition;

            if (coordinateXY)
            {
                zPosition = centerZ; // XY düzleminde zaten sabit
            }
            else if (useCustomZStart)
            {
                // Z ekseni verilen değerden başlasın
                zPosition = customZStart + column * (1 + gridSpaceZ);
            }
            else if (useCustomZEnd)
            {
                // Z ekseni verilen değerde bitsin (ters hesaplama)
                zPosition = customZEnd - (gridHeight - 1 - column) * (1 + gridSpaceZ);
            }
            else
            {
                // Varsayılan: ortalanmış grid
                zPosition = column * (1 + gridSpaceZ) - halfColumnSizeZ + centerZ;
            }

            return new Vector3(xPosition, yPosition, zPosition);
        }
    }
}