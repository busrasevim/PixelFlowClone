using System.Collections.Generic;
using _Project.Scripts.Pools;
using Sirenix.OdinInspector;
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

#if UNITY_EDITOR
        [OnValueChanged(nameof(OnInspectorChanged))]
#endif
        public float centerX;
#if UNITY_EDITOR
        [OnValueChanged(nameof(OnInspectorChanged))]
#endif
        public float centerY;
#if UNITY_EDITOR
        [OnValueChanged(nameof(OnInspectorChanged))]
#endif
        public float centerZ;

#if UNITY_EDITOR
        [OnValueChanged(nameof(OnInspectorChanged))]
#endif
        public float gridSpaceX = 1f;
#if UNITY_EDITOR
        [OnValueChanged(nameof(OnInspectorChanged))]
#endif
        public float gridSpaceY = 1f;
#if UNITY_EDITOR
        [OnValueChanged(nameof(OnInspectorChanged))]
#endif
        public float gridSpaceZ = 1f;

        protected Node[,] _nodes;

        public PoolTags nodePoolTag;

        [Header("Z Position Control")]
#if UNITY_EDITOR
        [OnValueChanged(nameof(OnInspectorChanged))]
#endif
        public bool useCustomZStart; // Özel başlangıç Z değeri
#if UNITY_EDITOR
        [OnValueChanged(nameof(OnInspectorChanged))]
#endif
        public float customZStart;

#if UNITY_EDITOR
        [OnValueChanged(nameof(OnInspectorChanged))]
#endif
        public bool useCustomZEnd; // Özel bitiş Z değeri
#if UNITY_EDITOR
        [OnValueChanged(nameof(OnInspectorChanged))]
#endif
        public float customZEnd;

        public virtual void Init(ObjectPool pool, Vector2Int size = default)
        {
            SpawnNodes(pool, transform);
        }

        protected virtual void SpawnNodes(ObjectPool pool, Transform parent = null, bool editorSpawn = false)
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


                    // If we are spawning in runtime
                    var node = pool.SpawnFromPool(nodePoolTag, position, Quaternion.identity).GetComponent<Node>();
                    node.transform.SetParent(parent);


                    // Set grid coordinates and initialize
                    node.SetCoordination(i, j);
                    node.Initialize(this);

                    // Store the grid object in the array
                    _nodes[i, j] = node;

                    node.gameObject.name = "Node_" + i + "_" + j;
                    
                    
                }
            }
        }

        public Node[] GetAllNodes()
        {
            var nodeList = new List<Node>();
            for (int i = 0; i < _nodes.GetLength(0); i++)
            {
                for (int j = 0; j < _nodes.GetLength(1); j++)
                {
                    nodeList.Add(_nodes[i, j]);
                }
            }

            return nodeList.ToArray();
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
            float halfRowSize = (gridWidth - 1) * (gridSpaceX) / 2f;
            float halfColumnSizeY = (gridHeight - 1) * (gridSpaceY) / 2f;
            float halfColumnSizeZ = (gridHeight - 1) * (gridSpaceZ) / 2f;

            // Calculate positions based on the selected coordinate planes
            float xPosition = coordinateYZ ? centerX : row * (gridSpaceX) - halfRowSize + centerX;
            float yPosition = coordinateXZ ? centerY : column * (gridSpaceY) - halfColumnSizeY + centerY;
            float zPosition;

            if (coordinateXY)
            {
                zPosition = centerZ; // XY düzleminde zaten sabit
            }
            else if (useCustomZStart)
            {
                // Z ekseni verilen değerden başlasın
                zPosition = customZStart + column * (gridSpaceZ);
            }
            else if (useCustomZEnd)
            {
                // Z ekseni verilen değerde bitsin (ters hesaplama)
                zPosition = customZEnd - (gridHeight - 1 - column) * (gridSpaceZ);
            }
            else
            {
                // Varsayılan: ortalanmış grid
                zPosition = column * (gridSpaceZ) - halfColumnSizeZ + centerZ;
            }

            return new Vector3(xPosition, yPosition, zPosition);
        }

#if UNITY_EDITOR
        private void OnInspectorChanged()
        {
            if (_nodes == null || _nodes.Length == 0)
                return;

            UpdateNodePositions();
            SceneView.RepaintAll();
        }
#endif

        protected virtual void UpdateNodePositions()
        {
            for (int i = 0; i < gridWidth; i++)
            {
                for (int j = 0; j < gridHeight; j++)
                {
                    if (_nodes[i, j] == null) continue;
                    _nodes[i, j].transform.position = GetNodePosition(i, j);
#if UNITY_EDITOR
                    EditorUtility.SetDirty(_nodes[i, j]);
#endif
                }
            }
        }
    }
}